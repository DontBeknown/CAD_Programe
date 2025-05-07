using UnityEngine;

public static class ColorToString
{
    private static readonly System.Collections.Generic.Dictionary<Color, string> namedColors =
        new System.Collections.Generic.Dictionary<Color, string>
    {
        { Color.red, "Red" },
        { Color.green, "Green" },
        { Color.blue, "Blue" },
        { Color.white, "White" },
        { Color.black, "Black" },
        { Color.yellow, "Yellow" },
        { Color.cyan, "Cyan" },
        { Color.magenta, "Magenta" },
        { Color.gray, "Gray" },
        { new Color(1f, 0.5f, 0f), "Orange" },
        { new Color(0.5f, 0f, 0.5f), "Purple" },
    };

    public static string Convert(Color color, bool useHexFallback = true)
    {
        foreach (var namedColor in namedColors)
        {
            if (ColorsApproximatelyEqual(color, namedColor.Key))
            {
                return namedColor.Value;
            }
        }

        return useHexFallback ? ToHexString(color) : ToRGBAString(color);
    }

    private static bool ColorsApproximatelyEqual(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b) &&
               Mathf.Approximately(a.a, b.a);
    }
    public static string ToHexString(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGBA(color);
    }
    public static string ToHexStringNoAlpha(Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }
    public static string ToRGBAString(Color color)
    {
        return $"RGBA({color.r:F2}, {color.g:F2}, {color.b:F2}, {color.a:F2})";
    }

    public static string ToRGB255String(Color color)
    {
        return $"RGB({(int)(color.r * 255)}, {(int)(color.g * 255)}, {(int)(color.b * 255)})";
    }
}