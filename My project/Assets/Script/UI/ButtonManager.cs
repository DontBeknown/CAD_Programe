using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [System.Serializable]
    public struct ModeButton
    {
        public InputMode mode;
        public Button button;
    }

    [SerializeField] InputManager inputManager;
    public List<ModeButton> modeButtons;

    void Start()
    {
        foreach (var mb in modeButtons)
        {
            InputMode modeCopy = mb.mode;
            mb.button.onClick.AddListener(() => OnModeButtonClicked(modeCopy));
        }

        SetButtonMode(InputMode.Select);
    }

    public void OnModeButtonClicked(InputMode mode)
    {
        inputManager.SetMode(mode);
    }

    public void SetButtonMode(InputMode newMode)
    {
        
        foreach (var mb in modeButtons)
        {
            bool isActive = mb.mode == newMode;
            SetButtonState(mb.button, isActive);
        }
    }

    private void SetButtonState(Button button, bool active)
    {
        var colors = button.colors;

        if (active)
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        }
        else
        {
            Color dimColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            colors.normalColor = dimColor;
            colors.highlightedColor = dimColor;
            colors.pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        button.colors = colors;
    }

}
