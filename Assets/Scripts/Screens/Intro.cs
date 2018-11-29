using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public GameObject background;
    public AudioSource audioSource;
    private bool musicStarted;

    void Awake()
    {
        Helpers.Set2DCameraToObject(background);
    }

    void OnMouseUp()
    {
        audioSource.Stop();
    }

    void Start()
    {
        var soundSystemMode = Config.GetSoundSystemMode();
        if (soundSystemMode == SoundSystemConfig.Music ||
            soundSystemMode == SoundSystemConfig.MusicAndSound)
        {
            musicStarted = true;
            audioSource.Play();
        }
        else
        {
            Invoke("GotoGameplay", 1);
        }
    }

    void GotoGameplay()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    void Update()
    {
        if (musicStarted && !audioSource.isPlaying)
            GotoGameplay();
#if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
#endif
    }
}
