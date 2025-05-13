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
        public GameObject panel;
    }

    public GameObject filePanel;
    public Button saveButton;
    public Button loadButton;
    public Sprite selectImage, defaultImage;

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
            if(mb.panel != null && !isActive)
            {
                mb.panel.SetActive(false);
            }

            if (isActive)
            {
                mb.button.image.sprite = selectImage;
                if(mb.mode == InputMode.ColorPick)
                    mb.panel.SetActive(true);
            }
            else
            {
                mb.button.image.sprite = defaultImage;
            }

        }
    }

    public void TogglePanel(InputMode mode)
    {
        foreach (var mb in modeButtons)
        {
            if(mb.panel != null && mb.mode == mode)
            {
                mb.panel.SetActive(!mb.panel.activeSelf);
            }
        }
    }

    public void ToggleFilePanel()
    {
        filePanel.SetActive(!filePanel.activeSelf);
    }
    
}
