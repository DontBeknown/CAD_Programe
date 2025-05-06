using UnityEngine;
using System.Collections.Generic;

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

    public void ParseCommand(InputMode mode, string input)
    {
        List<float> numbers = ParseNumbers(input);
        if (numbers == null)
        {
            DebugLogUI.Instance.Log("Invalid input: " + input);
            return;
        }

        switch (mode)
        {
            case InputMode.DrawLine:
                if (numbers.Count != 4)
                {
                    DebugLogUI.Instance.Log("Line requires 4 numbers: x1 y1 x2 y2");
                    return;
                }
                shapeDrawer.DrawLine(new Vector2(numbers[0], numbers[1]), new Vector2(numbers[2], numbers[3]));
                break;

            case InputMode.DrawCircle:
                if (numbers.Count != 3)
                {
                    DebugLogUI.Instance.Log("Circle requires 3 numbers: x y radius");
                    return;
                }
                if(numbers[2] <= 0)
                {
                    DebugLogUI.Instance.Log("Circle requires positive radius");
                    return;
                }
                shapeDrawer.DrawCircle(new Vector2(numbers[0], numbers[1]), (int)numbers[2]);
                break;

            case InputMode.DrawEllipse:
                if (numbers.Count != 4)
                {
                    DebugLogUI.Instance.Log("Ellipse requires 4 numbers: x y radiusX radiusY");
                    return;
                }
                if (numbers[2] <= 0 || numbers[3] <= 0)
                {
                    DebugLogUI.Instance.Log("Ellipse requires positive radius");
                    return;
                }
                shapeDrawer.DrawEllipse(new Vector2(numbers[0], numbers[1]), (int)numbers[2], (int)numbers[3]);
                break;

            case InputMode.DrawHermit:
                if (numbers.Count != 8)
                {
                    DebugLogUI.Instance.Log("Hermite requires 4 points: p0 p1 t0 t1");
                    return;
                }
                shapeDrawer.DrawHermite(new Vector2(numbers[0], numbers[1]), new Vector2(numbers[2], numbers[3])
                    ,new Vector2(numbers[4], numbers[5])
                    ,new Vector2(numbers[6], numbers[7])
                    );
                break;

            case InputMode.DrawBezier:
                if (numbers.Count != 8)
                {
                    DebugLogUI.Instance.Log("Bezier requires 4 points: p0 p1 p2 p3");
                    return;
                }
                shapeDrawer.DrawBezier(new Vector2(numbers[0], numbers[1]), new Vector2(numbers[2], numbers[3])
                    , new Vector2(numbers[4], numbers[5])
                    , new Vector2(numbers[6], numbers[7])
                    );
                break;

            case InputMode.RotatePreview:
                if (numbers.Count != 1)
                {
                    DebugLogUI.Instance.Log("Rotate requires exactly 1 number");
                    return;
                }
                float angle = numbers[0];
                if (angle < -360 || angle > 360)
                {
                    DebugLogUI.Instance.Log("Rotation must be between -360 and 360");
                    return;
                }

                Shape selected = selectionManager.GetSelectedShape();
                if (selected == null)
                {
                    DebugLogUI.Instance.Log("No shape selected for rotation");
                    return;
                }

                rotationController.StartRotation(selected);
                rotationController.ApplyNumericRotation(angle);
                rotationController.ConfirmRotation();

                DebugLogUI.Instance.Log($"Rotated selected shape by {angle} degrees");
                break;

            case InputMode.Move:
                if (numbers.Count != 2)
                {
                    DebugLogUI.Instance.Log("Move requires 2 numbers: dx dy");
                    return;
                }

                Shape selectedMove = selectionManager.GetSelectedShape();
                if (selectedMove == null)
                {
                    DebugLogUI.Instance.Log("No shape selected to move");
                    return;
                }

                Vector2 offset = new Vector2(numbers[0], numbers[1]);
                selectedMove.MoveOffset(offset);

                DebugLogUI.Instance.Log($"Moved shape by {offset}");
                break;
            default:
                DebugLogUI.Instance.Log("Invalid mode for command input");
                break;
        }
    }

    private List<float> ParseNumbers(string input)
    {
        string[] parts = input.Split(' ');
        List<float> numbers = new List<float>();
        foreach (var part in parts)
        {
            if (float.TryParse(part, out float result))
                numbers.Add(result);
        }
        return numbers.Count > 0 ? numbers : null;
    }
}
