using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class InputFieldManager : MonoBehaviour
{
    public ShapeDrawer shapeDrawer;

    [Header("Line")]
    public TMP_InputField startLine;
    public TMP_InputField endLine;

    [Header("Circle")]
    public TMP_InputField centerCircle;
    public TMP_InputField radiusCircle;
    public Toggle isFillCircle;

    [Header("Ellipse")]
    public TMP_InputField centerEllipse;
    public TMP_InputField radiusXEllipse;
    public TMP_InputField radiusYEllipse;
    public Toggle isFillEllipse;

    [Header("Hermite")]
    public TMP_InputField startHermite;
    public TMP_InputField endHermite;
    public TMP_InputField startTangent;
    public TMP_InputField endTangent;

    [Header("Bezier")]
    public TMP_InputField startControlPoint1;
    public TMP_InputField startControlPoint2;
    public GameObject listItemPrefab;
    public Transform listContentParent;
    private List<GameObject> controlPointInputs = new List<GameObject>();

    public void DrawShapeFromInput(InputMode input, Color color)
    {
        switch (input)
        {
            case InputMode.DrawLine:
                DrawLine(color);
                break;
            case InputMode.DrawCircle:
                DrawCircle(color);
                break;
            case InputMode.DrawEllipse:
                DrawEllipse(color);
                break;
            case InputMode.DrawHermit:
                DrawHermite(color);
                break;
            case InputMode.DrawNBezier:
                DrawNBezier(color);
                break;
        }
    }

    private void DrawLine(Color color)
    {
        if (!AreInputsValid(startLine, endLine)) return;

        shapeDrawer.DrawLine(GetVector2Int(startLine), GetVector2Int(endLine), color);
        ClearFields(startLine, endLine);
    }

    private void DrawCircle(Color color)
    {
        if (!AreInputsValid(centerCircle, radiusCircle)) return;

        shapeDrawer.DrawCircle(
            GetVector2Int(centerCircle),
            GetInt(radiusCircle),
            isFillCircle.isOn,
            color
        );
        ClearFields(centerCircle, radiusCircle);
    }

    private void DrawEllipse(Color color)
    {
        if (!AreInputsValid(centerEllipse, radiusXEllipse, radiusYEllipse)) return;

        shapeDrawer.DrawEllipse(
            GetVector2Int(centerEllipse),
            GetInt(radiusXEllipse),
            GetInt(radiusYEllipse),
            isFillEllipse.isOn,
            color
        );
        ClearFields(centerEllipse, radiusXEllipse, radiusYEllipse);
    }

    private void DrawHermite(Color color)
    {
        if (!AreInputsValid(startHermite, endHermite, startTangent, endTangent)) return;

        shapeDrawer.DrawHermite(
            GetVector2Int(startHermite),
            GetVector2Int(endHermite),
            GetVector2Int(startTangent),
            GetVector2Int(endTangent),
            color
        );
        ClearFields(startHermite, endHermite, startTangent, endTangent);
    }

    private void DrawNBezier(Color color)
    {
        if(!AreInputsValid(startControlPoint1, startControlPoint2)) return;

        List<Vector2> controlPoints = new List<Vector2>();
        controlPoints.Add(GetVector2Int(startControlPoint1));
        controlPoints.Add(GetVector2Int(startControlPoint2));

        foreach (GameObject obj in controlPointInputs)
        {
            TMP_InputField inputField = obj.GetComponent<TMP_InputField>();
            controlPoints.Add(GetVector2Int(inputField));
        }

        shapeDrawer.DrawNBezier(controlPoints, color);

        foreach (GameObject obj in controlPointInputs)
        {
            Destroy(obj);
        }
        ClearFields(startControlPoint1, startControlPoint2);
    }

    public void AddControlPointInput()
    {
        GameObject listItem = Instantiate(listItemPrefab, listContentParent);
        controlPointInputs.Add(listItem);
    }

    #region Helper Methods

    private bool AreInputsValid(params TMP_InputField[] inputs)
    {
        foreach (var input in inputs)
        {
            if (string.IsNullOrWhiteSpace(input.text))
                return false;
        }
        return true;
    }

    private void ClearFields(params TMP_InputField[] fields)
    {
        foreach (var field in fields)
        {
            field.text = "";
        }
    }

    private Vector2Int GetVector2Int(TMP_InputField inputField)
    {
        string[] parts = inputField.text.Trim().Split(' ');

        if (parts.Length == 2 &&
            int.TryParse(parts[0], out int x) &&
            int.TryParse(parts[1], out int y))
        {
            return new Vector2Int(x, y);
        }

        return Vector2Int.zero;
    }

    private int GetInt(TMP_InputField inputField)
    {
        return int.TryParse(inputField.text, out int result) ? result : 0;
    }
    #endregion
}
