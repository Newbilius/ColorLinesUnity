using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    public static CharactersManager Instance;
    public float MinimumCharacterY;
    public float MaximumCharacterY;
    public GameObject Enemy;
    public GameObject Player;
    private SpriteRenderer EnemySpriteRenderer;
    private SpriteRenderer PlayerSpriteRenderer;
    public Sprite EnemyOk;
    public Sprite EnemyDie;
    public Sprite PlayerOk;
    public Sprite PlayerWin;

    void Awake()
    {
        Instance = this;
        EnemySpriteRenderer = Enemy.GetComponent<SpriteRenderer>();
        PlayerSpriteRenderer = Player.GetComponent<SpriteRenderer>();
    }

    public void UpdateEnemyAndUser(int score, int enemyScore)
    {
        if (score <= enemyScore)
        {
            if (EnemySpriteRenderer.sprite != EnemyOk)
                EnemySpriteRenderer.sprite = EnemyOk;
            if (PlayerSpriteRenderer.sprite != PlayerOk)
                PlayerSpriteRenderer.sprite = PlayerOk;
        }
        else
        {
            if (EnemySpriteRenderer.sprite != EnemyDie)
                EnemySpriteRenderer.sprite = EnemyDie;
            if (PlayerSpriteRenderer.sprite != PlayerWin)
                PlayerSpriteRenderer.sprite = PlayerWin;
        }

        var tempForUserY = MaximumCharacterY - MinimumCharacterY;
        var userY = score * tempForUserY / ((float)enemyScore * 2);
        userY += MinimumCharacterY;
        if (userY > MaximumCharacterY)
            userY = MaximumCharacterY;
        Player.transform.position = new Vector3(Player.transform.position.x, userY);

        var tempForEnemyY = MinimumCharacterY - MaximumCharacterY;
        var enemyY = score * tempForEnemyY / ((float)enemyScore * 2);
        enemyY += MaximumCharacterY;
        if (enemyY < MinimumCharacterY)
            enemyY = MinimumCharacterY;
        Enemy.transform.position = new Vector3(Enemy.transform.position.x, enemyY);
    }
}
