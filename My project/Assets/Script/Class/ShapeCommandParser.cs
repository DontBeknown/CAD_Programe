using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ShapeCommandParser
{
    private ShapeDrawer shapeDrawer;
    private SelectionManager selectionManager;
    private ShapeRotationController rotationController;

    public ShapeCommandParser(ShapeDrawer drawer, SelectionManager selection, ShapeRotationController rotation)
    {
        shapeDrawer = drawer;
        selectionManager = selection;
        rotationController = rotation;
    }

    public void ParseCommand(InputMode mode, string input, Shape selectedShape = null)
    {
        List<float> numbers = ParseNumbers(input);
        string colorName = ExtractLastWord(input);
        Color shapeColor = TryParseColor(colorName);

        if (numbers == null)
        {
            DebugLogUI.Instance.Log("Invalid input: " + input);
            return;
        }

        switch (mode)
        {
            case InputMode.DrawLine:
            case InputMode.DrawCircle:
            case InputMode.DrawEllipse:
            case InputMode.DrawHermit:
            case InputMode.DrawBezier:
                HandleDrawCommand(mode, numbers, shapeColor);
                break;

            case InputMode.Select:
                if (selectedShape != null)
                    HandleEditCommand(selectedShape, numbers, shapeColor);
                break;

            case InputMode.RotatePreview:
                HandleRotation(numbers);
                break;

            case InputMode.Move:
                HandleMove(numbers);
                break;

            default:
                
                break;
        }
    }

    private void HandleDrawCommand(InputMode mode, List<float> numbers, Color color)
    {
        switch (mode)
        {
            case InputMode.DrawLine:
                if (numbers.Count != 4)
                {
                    LogInvalid("Line requires 4 numbers: x1 y1 x2 y2");
                    return;
                }
                shapeDrawer.DrawLine(ToV2(numbers[0], numbers[1]), ToV2(numbers[2], numbers[3]), color);
                break;

            case InputMode.DrawCircle:
                if (!ValidatePositive(numbers, 2, "Circle requires 3 numbers: x y radius", "Circle requires positive radius"))
                    return;
                shapeDrawer.DrawCircle(ToV2(numbers[0], numbers[1]), (int)numbers[2], color);
                break;

            case InputMode.DrawEllipse:
                if (!ValidatePositive(numbers, 3, "Ellipse requires 4 numbers: x y radiusX radiusY", "Ellipse requires positive radius"))
                    return;
                shapeDrawer.DrawEllipse(ToV2(numbers[0], numbers[1]), (int)numbers[2], (int)numbers[3], color);
                break;

            case InputMode.DrawHermit:
                if (numbers.Count != 8)
                {
                    LogInvalid("Hermite requires 4 points: p0 p1 t0 t1");
                    return;
                }
                shapeDrawer.DrawHermite(ToV2(numbers[0], numbers[1]), ToV2(numbers[2], numbers[3]),
                                        ToV2(numbers[4], numbers[5]), ToV2(numbers[6], numbers[7]), color);
                break;

            case InputMode.DrawBezier:
                if (numbers.Count != 8)
                {
                    LogInvalid("Bezier requires 4 points: p0 p1 p2 p3");
                    return;
                }
                shapeDrawer.DrawBezier(ToV2(numbers[0], numbers[1]), ToV2(numbers[2], numbers[3]),
                                       ToV2(numbers[4], numbers[5]), ToV2(numbers[6], numbers[7]), color);
                break;
        }
    }

    private void HandleEditCommand(Shape shape, List<float> numbers, Color color)
    {
        switch (shape)
        {
            case Line line:
                if (numbers.Count != 4)
                {
                    LogInvalid("Line requires 4 numbers: x1 y1 x2 y2");
                    return;
                }
                    
                line.SetPoints(ToV2(numbers[0], numbers[1]), ToV2(numbers[2], numbers[3]));
                line.Recolor(color);
                break;

            case Circle circle:
                if (!ValidatePositive(numbers, 2, "Circle requires 3 numbers: x y radius", "Circle requires positive radius")) return;
                circle.SetPoints(ToV2(numbers[0], numbers[1]), (int)numbers[2]);
                circle.Recolor(color);
                break;

            case Ellipse ellipse:
                if (!ValidatePositive(numbers, 3, "Ellipse requires 4 numbers: x y radiusX radiusY", "Ellipse requires positive radius")) return;
                ellipse.SetValues(ToV2(numbers[0], numbers[1]), (int)numbers[2], (int)numbers[3]);
                ellipse.Recolor(color);
                break;

            case HermiteCurve hermite:
                if (numbers.Count != 8)
                {
                    LogInvalid("Hermite requires 4 points: p0 p1 t0 t1");
                    return;
                }
                hermite.SetValues(ToV2(numbers[0], numbers[1]), ToV2(numbers[2], numbers[3]),
                                  ToV2(numbers[4], numbers[5]), ToV2(numbers[6], numbers[7]));
                hermite.Recolor(color);
                break;

            case BezierCurve bezier:
                if (numbers.Count != 8)
                {
                    LogInvalid("Bezier requires 4 points: p0 p1 p2 p3");
                    return;
                }
                bezier.SetValues(ToV2(numbers[0], numbers[1]), ToV2(numbers[2], numbers[3]),
                                 ToV2(numbers[4], numbers[5]), ToV2(numbers[6], numbers[7]));
                bezier.Recolor(color);
                break;
        }
    }

    private void HandleRotation(List<float> numbers)
    {
        if (numbers.Count != 1)
        {
            LogInvalid("Rotate requires exactly 1 number");
            return;
        }

        float angle = numbers[0];
        if (angle < -360 || angle > 360)
        {
            LogInvalid("Rotation must be between -360 and 360");
            return;
        }

        Shape selected = selectionManager.GetSelectedShape();
        if (selected == null)
        {
            LogInvalid("No shape selected for rotation");
            return;
        }

        rotationController.StartRotation(selected);
        rotationController.ApplyNumericRotation(angle);
        rotationController.ConfirmRotation();

        DebugLogUI.Instance.Log($"Rotated selected shape to {angle} degrees");
    }

    private void HandleMove(List<float> numbers)
    {
        if (numbers.Count != 2)
        {
            LogInvalid("Move requires 2 numbers: dx dy");
            return;
        }

        Shape selected = selectionManager.GetSelectedShape();
        if (selected == null)
        {
            LogInvalid("No shape selected to move");
            return;
        }

        Vector2 offset = ToV2(numbers[0], numbers[1]);
        selected.MoveOffset(offset);
        DebugLogUI.Instance.Log($"Moved shape by {offset}");
    }

    private List<float> ParseNumbers(string input)
    {
        return input.Split(' ')
                    .Where(p => float.TryParse(p, out _))
                    .Select(float.Parse)
                    .ToList();
    }

    private string ExtractLastWord(string input)
    {
        return string.IsNullOrWhiteSpace(input) ? "" : input.Split(' ').Last();
    }

    private Color TryParseColor(string colorName)
    {
        return ColorUtility.TryParseHtmlString(colorName, out var color) ? color : Color.black;
    }

    private bool ValidatePositive(List<float> numbers, int minIndex, string countError, string valueError)
    {
        if (numbers.Count <= minIndex)
        {
            LogInvalid(countError);
            return false;
        }

        if (numbers.Skip(minIndex).Any(n => n <= 0))
        {
            LogInvalid(valueError);
            return false;
        }

        return true;
    }

    private Vector2 ToV2(float x, float y) => new Vector2(x, y);

    private bool LogInvalid(string message)
    {
        DebugLogUI.Instance.Log(message);
        return false;
    }
}
