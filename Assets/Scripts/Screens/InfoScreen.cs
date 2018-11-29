using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour {

    public GameObject background;
    public Text scoreNames;
    public Text scoreValues;

    void Awake()
    {
        GameManager.GameState = GameState.InfoMenu;
        scoreNames.text = ScoreManager.GetNamesListForUI();
        scoreValues.text = ScoreManager.GetValuesListForUI();
        Helpers.Set2DCameraToObject(background);
    }

    public void OnCloseButtonClick()
    {
        SceneManager.UnloadSceneAsync("InfoScene");
        GameManager.GameState = GameState.Gameplay;
    }

    public void OnGotoSiteButtonClick()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://search?q=pub:%D0%94%D0%BC%D0%B8%D1%82%D1%80%D0%B8%D0%B9+%D0%9C%D0%BE%D0%B8%D1%81%D0%B5%D0%B5%D0%B2");
#else
        Application.OpenURL("http://www.old-hard.ru/");
#endif
    }

    void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
            OnCloseButtonClick();
#endif
    }
}
