using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ColorPickerController : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField hexInputField;
    public TMP_InputField rgbInputField;
    public RawImage colorPreview;

    [SerializeField] private InputManager inputManager;

    private bool isUpdating = false;

    void Start()
    {
        hexInputField.onValueChanged.AddListener(OnHexInputChanged);
        rgbInputField.onValueChanged.AddListener(OnRGBInputChanged);

        OnHexInputChanged("#000000");
        hexInputField.text = "#000000";
    }

    void OnHexInputChanged(string hex)
    {
        if (isUpdating) return;

        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            isUpdating = true;

            rgbInputField.text = ColorToString.ToRGB255String(color);
            UpdateColorPreview(color);

            inputManager.currentColor = color;

            isUpdating = false;
        }
    }

    void OnRGBInputChanged(string rgb)
    {
        if (isUpdating) return;

        if (TryParseRGBString(rgb, out Color color))
        {
            isUpdating = true;

            hexInputField.text = ColorToString.ToHexString(color);
            UpdateColorPreview(color);

            inputManager.currentColor = color;

            isUpdating = false;
        }
    }

    void UpdateColorPreview(Color color)
    {
        colorPreview.color = color;
    }

    bool TryParseRGBString(string input, out Color color)
    {
        color = Color.black;
        var parts = input.Split(' ');

        if (parts.Length != 3) return false;

        if (int.TryParse(parts[0], out int r) &&
            int.TryParse(parts[1], out int g) &&
            int.TryParse(parts[2], out int b))
        {
            if (IsByte(r) && IsByte(g) && IsByte(b))
            {
                color = new Color(r / 255f, g / 255f, b / 255f, 1f);
                return true;
            }
        }

        return false;
    }

    bool IsByte(int value) => value >= 0 && value <= 255;
}
