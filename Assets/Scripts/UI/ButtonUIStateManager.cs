using UnityEngine;
using UnityEngine.UI;

public class ButtonUIStateManager : MonoBehaviour
{
    public static ButtonUIStateManager Instance;

    public Button PathButton;
    public Button SoundSystemButton;
    public Button NextColorsButton;
    public Button InfoButton;
    public Button RestartButton;
    public Button ExitButton;

    private ButtonState NextColorsButtonState;
    private ButtonState PathButtonState;
    private ButtonState SoundSystemButtonState;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        NextColorsButtonState = NextColorsButton.GetComponent<ButtonState>();
        PathButtonState = PathButton.GetComponent<ButtonState>();
        SoundSystemButtonState = SoundSystemButton.GetComponent<ButtonState>();

        //в оригинале буквы серые, но насколько это очевидный UI-паттерн?
        //RestartButton.GetComponent<ButtonState>().SetActivation(false);
        //ExitButton.GetComponent<ButtonState>().SetActivation(false);
        //InfoButton.GetComponent<ButtonState>().SetActivation(false);

        ReloadStatesFromConfig();
    }

    public void ReloadStatesFromConfig()
    {
        var newMusicValue = Config.GetSoundSystemMode();

        switch (newMusicValue)
        {
            case SoundSystemConfig.MusicAndSound:
            case SoundSystemConfig.None:
                SoundSystemButtonState.SetText("Звук/Муз");
                break;

            case SoundSystemConfig.Music:
                SoundSystemButtonState.SetText("Музыка");
                break;

            case SoundSystemConfig.Sound:
                SoundSystemButtonState.SetText("Звук");
                break;
        }

        SoundSystemButtonState.SetActivation(newMusicValue != SoundSystemConfig.None);
        NextColorsButtonState.SetActivation(Config.GetNextColorsValue());
        PathButtonState.SetActivation(Config.GetPathValue());
    }
}
