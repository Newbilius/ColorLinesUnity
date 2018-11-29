using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//работа с блоком "сохраните свой рекорд"
public class GameCompletingManager : MonoBehaviour
{
    public static GameCompletingManager Instance;

    public InputField userNameInputField;
    public GameObject gameCompleteBlock;
    public GameObject saveScoreBlock;
    public GameObject cantSaveScoreBlock;
    public Text scoreNames;
    public Text scoreValues;
    string newUserName;
    private Action OnComplete;
    private int Score;

    void Awake()
    {
        Instance = this;
    }

    public void Activate(int score, Action onComplete)
    {
        OnComplete = onComplete;
        Score = score;
        gameCompleteBlock.SetActive(true);

        if (Score > ScoreManager.GetMinimumValue())
        {
            saveScoreBlock.SetActive(true);
            cantSaveScoreBlock.SetActive(false);
        }
        else
        {
            saveScoreBlock.SetActive(false);
            cantSaveScoreBlock.SetActive(true);
        }

        UpdateScore();
    }

    public void Deactivate()
    {
        gameCompleteBlock.SetActive(false);
    }

    private void UpdateScore()
    {
        scoreNames.text = ScoreManager.GetNamesListForUI();
        scoreValues.text = ScoreManager.GetValuesListForUI();
    }

    public void OnUserNameChanged()
    {
        newUserName = userNameInputField.text;
    }

    public void OnSetUserNameCompleted()
    {
        newUserName = userNameInputField.text;
        OnSaveScoresButtonClick();
    }

    public void OnSaveScoresButtonClick()
    {
        var scoreResults = ScoreManager.GetScores();
        scoreResults.Data.Add(new Score
        {
            Name = newUserName,
            Value = Score
        });

        scoreResults.Data = scoreResults.Data.OrderByDescending(x => x.Value).ToList();
        scoreResults.Data.RemoveAt(scoreResults.Data.Count - 1);
        Config.SaveScores(scoreResults);

        userNameInputField.text = "";
        if (OnComplete != null)
            OnComplete();
        OnComplete = null;
    }
}
