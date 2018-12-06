using LeePathSearchAlgorithm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//неплохо бы вытащить работу с анимациями в другой класс (GameBoardManager?), да и обработку нажатий на UI-кнопки тоже
public class GameManager : MonoBehaviour
{
    //todo нехорошо так делать (сейчас вытащил исключительно для InfoScreen)
    private static GameState gameState;
    public static GameState GameState
    {
        get
        {
            return gameState;
        }
        set
        {
            gameState = value;
            if (gameState == GameState.Animation)
                Cursor.visible = false;
            else
                Cursor.visible = true;
        }
    }

    public GameObject[] balls;
    public BallBehavior[] ballsBehavior;
    public AudioClip deleteBallsSound;
    public AudioClip wrongPlace;
    PathSearchLee pathSearcher;
    int[,] mapForPathFinding = new int[9, 9];
    BallBehavior currentBall;
    Point currentBallPosition;
    bool ShowPath; //true - идти пешком и показывать следы, false - перемещаться телепортацией

    public Text enemyName;

    //счётчики очков
    public Text enemyScoreText;
    public Text scoreText;

    //шаги
    public GameObject stepToLeft;
    public GameObject stepToRight;
    public GameObject stepToUp;
    public GameObject stepToDown;
    public GameObject background;

    //блок "новые цвета" и всё, что с ним связано
    public GameObject[] PredictionBalls;
    readonly SpriteRenderer[] PredictionBallsSpriteRenderer = new SpriteRenderer[3];
    public Sprite[] PredictionBallsSprites;
    readonly int[] NextBalls = new int[3];

    int EnemyScore;
    int score { get; set; }
    int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = score.ToString();
            CharactersManager.Instance.UpdateEnemyAndUser(Score, EnemyScore);
        }
    }

    public void OnRestartButtonClick()
    {
        if (GameState == GameState.InfoMenu)
            return;
        RestartGame();
    }

    public void OnExitButtonClick()
    {
        if (GameState == GameState.InfoMenu)
            return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("http://www.old-hard.ru/");
#else
        Application.Quit();
#endif
    }

    public void OnInfoButtonClick()
    {
        if (GameState == GameState.InfoMenu)
            return;
        SceneManager.LoadScene("InfoScene", LoadSceneMode.Additive);
    }

    public void OnPathButtonClick()
    {
        if (GameState == GameState.InfoMenu)
            return;
        ShowPath = Config.SwitchPath();
        ButtonUIStateManager.Instance.ReloadStatesFromConfig();
    }

    public void OnSoundButtonClick()
    {
        if (GameState == GameState.InfoMenu)
            return;
        Config.SwitchSoundSystemMode();
        AudioManager.Instance.ReloadStatesFromConfig();
        ButtonUIStateManager.Instance.ReloadStatesFromConfig();
    }

    public void OnNextColorsButtonClick()
    {
        if (GameState == GameState.InfoMenu)
            return;
        Config.SwitchNextColors();
        ButtonUIStateManager.Instance.ReloadStatesFromConfig();
        UpdateNextBalls();
    }

    private void UpdateNextBalls()
    {
        for (int i = 0; i < 3; i++)
        {
            var showNextColors = Config.GetNextColorsValue();
            PredictionBalls[i].SetActive(showNextColors);
            PredictionBallsSpriteRenderer[i].sprite = PredictionBallsSprites[NextBalls[i]];
        }
    }

    void Awake()
    {
        ShowPath = Config.GetPathValue();

        pathSearcher = new PathSearchLee(PathSearchLee.SearchMethod.Path4);

        for (int i = 0; i < 3; i++)
            PredictionBallsSpriteRenderer[i] = PredictionBalls[i].GetComponent<SpriteRenderer>();

        ballsBehavior = new BallBehavior[balls.Length];
        for (int i = 0; i < balls.Length; i++)
            ballsBehavior[i] = balls[i].GetComponent<BallBehavior>();

        Helpers.Set2DCameraToObject(background);
    }

    void Start()
    {
        Helpers.ForceLandscapeOrientation();

        for (int i = 0; i < 3; i++)
            PredictionBalls[i].SetActive(false);
        RestartGame();
    }

    void RestartGame()
    {
        if (GameState == GameState.RestartAnimation)
            return;
        GameState = GameState.RestartAnimation;

        var highScore = ScoreManager.GetHighScore();
        EnemyScore = highScore.Value;
        enemyScoreText.text = highScore.Value.ToString();
        enemyName.text = highScore.Name;

        GameCompletingManager.Instance.Deactivate();
        Score = 0;
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 9; y++)
            {
                if (GameBoardManager.Instance.field[x, y] != null)
                    GameBoardManager.Instance.field[x, y].Die();
                GameBoardManager.Instance.field[x, y] = null;
            }
        StartCoroutine(RestartGameComplete());
    }

    IEnumerator RestartGameComplete()
    {
        //todo тут по хорошему бы подписаться на событие "все шарики закончили анимацию исчезновения"
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 5; i++)
            CreateBallInRandomPlace();

        InitNextBalls();

        GameState = GameState.Gameplay;
    }

    private void InitNextBalls()
    {
        for (int i = 0; i < 3; i++)
            NextBalls[i] = ballsBehavior[Random.Range(0, ballsBehavior.Length)].BallCode - 1;
        UpdateNextBalls();
    }

    private void CreateBallInRandomPlace(int nextBalls = -1)
    {
        var x = Random.Range(0, 8);
        var y = Random.Range(0, 8);
        int counter = 0;
        while (GameBoardManager.Instance.field[x, y] != null)
        {
            counter++;
            x = Random.Range(0, 8);
            y = Random.Range(0, 8);
            if (counter == 5)
                break;
        }
        if (GameBoardManager.Instance.field[x, y] == null)
        {
            GameBoardManager.Instance.field[x, y] = CreateBall(x, y, nextBalls);
            return;
        }
        for (x = 0; x < 9; x++)
            for (y = 0; y < 9; y++)
            {
                if (GameBoardManager.Instance.field[x, y] == null)
                {
                    GameBoardManager.Instance.field[x, y] = CreateBall(x, y, nextBalls);
                    return;
                }
            }
    }

    private BallBehavior CreateBall(int x, int y, int ballValue = -1)
    {
        var pos = GameBoardManager.Instance.GetPos(x, y);
        if (ballValue == -1)
            ballValue = Random.Range(0, balls.Length);
        var ball = Instantiate(balls[ballValue], pos, Quaternion.identity);
        var component = ball.GetComponent<BallBehavior>();
        return component;
    }

    void Update()
    {
#if UNITY_ANDROID
        if (GameState != GameState.InfoMenu
            && Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
#endif
    }

    void OnMouseDown()
    {
        if (GameState != GameState.Gameplay)
            return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        var oldBall = currentBall;
        var oldBallPosition = currentBallPosition;
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            var position = hit.collider.gameObject.transform.position;
            currentBallPosition = GameBoardManager.Instance.ScreenCoordToBoardCoords(position);

            currentBall = GameBoardManager.Instance.field[currentBallPosition.X, currentBallPosition.Y];

            if (currentBall != null)
                currentBall.StartJumping();

            if (currentBall != null && oldBall != null)
                oldBall.StopJumping();

            if (currentBall == null && oldBall != null)
            {
                var successMoveToNewPostion = MoveBallToNewPositionIfCan(oldBall, oldBallPosition, currentBallPosition);
                if (!successMoveToNewPostion)
                {
                    currentBall = oldBall;
                    currentBallPosition = oldBallPosition;
                }
            }
        }
    }

    private bool MoveBallToNewPositionIfCan(BallBehavior oldBall, Point start, Point end)
    {
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 9; y++)
                mapForPathFinding[x, y] = GameBoardManager.Instance.field[x, y] == null ? 0 : 1;

        mapForPathFinding[start.X, start.Y] = 0;
        var resultPath = pathSearcher.Search(mapForPathFinding, start, end);
        if (resultPath.Any())
        {
            var oldPosition = GameBoardManager.Instance.GetPos(start.X, start.Y);
            Queue<GameObject> stepImages = new Queue<GameObject>();

            if (ShowPath)
            {
                var firstStepImage = GetNextImage(new Vector3Combined
                {
                    Start = oldPosition,
                    End = GameBoardManager.Instance.GetPos(resultPath[0].X, resultPath[0].Y)
                });
                stepImages.Enqueue(firstStepImage);
                GameState = GameState.Animation;

                oldBall.MoveByPath(resultPath.Select(pathStep => GameBoardManager.Instance.GetPos(pathStep.X, pathStep.Y)).ToArray(),
                () =>
                {
                    StartCoroutine(AfterBallMovedToNewPosition(stepImages, oldBall, start, end));
                },
                (movedPosition) =>
                {
                    GameObject stepImage = GetNextImage(movedPosition);

                    if (stepImage != null)
                        stepImages.Enqueue(stepImage);
                });
            }
            else
            {
                GameState = GameState.Animation;
                oldBall.StartTeleport(() =>
                {
                    oldBall.transform.position = GameBoardManager.Instance.GetPos(end.X, end.Y);

                    oldBall.StartShowingAnimation(() =>
                    {
                        StartCoroutine(AfterBallMovedToNewPosition(stepImages, oldBall, start, end));
                    });
                });
            }
            return true;
        }
        else
        {
            AudioManager.Instance.PlaySound(wrongPlace);
            return false;
        }
    }

    private GameObject GetNextImage(Vector3Combined vector)
    {
        if (vector.End.x > vector.Start.x)
            return Instantiate(stepToRight, vector.Start, Quaternion.identity);
        if (vector.End.x < vector.Start.x)
            return Instantiate(stepToLeft, vector.Start, Quaternion.identity);
        if (vector.End.y > vector.Start.y)
            return Instantiate(stepToUp, vector.Start, Quaternion.identity);
        if (vector.End.y < vector.Start.y)
            return Instantiate(stepToDown, vector.Start, Quaternion.identity);

        return null;
    }

    private IEnumerator AfterBallMovedToNewPosition(Queue<GameObject> stepImages, BallBehavior oldBall, Point start, Point end)
    {
        var oldScore = Score;
        oldBall.StopJumping();
        while (stepImages.Count > 0)
        {
            var stepToRemove = stepImages.Dequeue();
            Destroy(stepToRemove);
            if (stepImages.Count > 0)
                yield return new WaitForSeconds(0.05f);
        }
        GameBoardManager.Instance.field[end.X, end.Y] = GameBoardManager.Instance.field[start.X, start.Y];
        GameBoardManager.Instance.field[start.X, start.Y] = null;
        var deletedCount = RemoveCompletedLinesAndAddScore();
        if (deletedCount > 0)
            yield return new WaitForSeconds(0.10f);
        else
        {
            for (int i = 0; i < 3; i++)
                CreateBallInRandomPlace(NextBalls[i]);
            InitNextBalls();
            RemoveCompletedLinesAndAddScore();
        }
        GameState = GameState.Gameplay;
        CheckGameover();
        if (GameState == GameState.Gameplay
            && Score > EnemyScore
             && oldScore <= EnemyScore)
        {
            GameState = GameState.Gameplay;
            AudioManager.Instance.PlayWinMusicIfCan(() => { });
        }
    }

    private void CheckGameover()
    {
        bool haveEmptySpaces = false;
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 9; y++)
            {
                if (GameBoardManager.Instance.field[x, y] == null)
                {
                    haveEmptySpaces = true;
                    break;
                }
            }
        if (!haveEmptySpaces)
        {
            GameState = GameState.GameOver;
            GameCompletingManager.Instance.Activate(Score, () =>
            {
                RestartGame();
            });
        }
    }

    private int RemoveCompletedLinesAndAddScore()
    {
        var result = LinesSearcher.SearchBy(GameBoardManager.Instance.field);
        foreach (var point in result.Points)
        {
            var ball = GameBoardManager.Instance.field[point.X, point.Y];
            if (ball != null)
                ball.Die();
            GameBoardManager.Instance.field[point.X, point.Y] = null;
            AudioManager.Instance.PlaySound(deleteBallsSound);
        }
        Score += result.Score;
        return result.Points.Count;
    }
}