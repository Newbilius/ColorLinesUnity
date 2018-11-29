using UnityEngine;
using UnityEngine.UI;

public class ButtonState : MonoBehaviour
{
    Text text;
    Color deactivatedColor;
    Color activatedColor;
    public bool Activated { get; private set; }

    void Awake()
    {
        text = GetComponent<Button>().GetComponentInChildren<Text>();
        ColorUtility.TryParseHtmlString("#555555", out deactivatedColor);
        ColorUtility.TryParseHtmlString("#00AA00", out activatedColor);
    }

    public void SetActivation(bool value)
    {
        Activated = value;
        text.color = Activated ? activatedColor : deactivatedColor;
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
