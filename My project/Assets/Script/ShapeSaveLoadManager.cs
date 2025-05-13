using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using SFB;

using UnityEngine.UI;
public class ShapeSaveLoadManager : MonoBehaviour
{
    private GridDraw gridDraw;

    void Start()
    {

        gridDraw = GetComponent<GridDraw>();
    }

    public void SaveShapes(List<Shape> shapes)
    {
        var extensions = new[] {
            new ExtensionFilter("JSON Files", "json")
        };

        string path = StandaloneFileBrowser.SaveFilePanel("Save Shape File", "", "shapes", extensions);

        if (string.IsNullOrEmpty(path))
        {
            DebugLogUI.Instance.Log("Save cancelled.");
            return;
        }

        List<ShapeData> shapeDatas = new List<ShapeData>();

        foreach (var shape in shapes)
        {
            if (shape is Line line)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "Line",
                    position1 = line.StartPoint,
                    position2 = line.EndPoint,
                    Color = line.Color,
                    offsetPosition = shape.Offset,
                    rotation = shape.GetRotation()
                });
            }
            else if (shape is Circle circle)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "Circle",
                    position1 = circle.CenterPoint,
                    radius = circle.Radius,
                    Color = circle.Color,
                    isFill = circle.Fill,
                    offsetPosition = shape.Offset,
                    rotation = shape.GetRotation()
                });
            }
            else if (shape is Ellipse ellipse)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "Ellipse",
                    position1 = ellipse.CenterPoint,
                    radiusX = ellipse.RadiusX,
                    radiusY = ellipse.RadiusY,
                    Color = ellipse.Color,
                    offsetPosition = shape.Offset,
                    rotation = shape.GetRotation()
                });
            }
            else if (shape is HermiteCurve hermite)
            {
                shapeDatas.Add(new ShapeData { 
                    type = "Hermite",
                    position1 = hermite.P0,
                    position2 = hermite.P1,
                    position3 = hermite.T0,
                    position4 = hermite.T1,
                    Color = hermite.Color,
                    offsetPosition = shape.Offset,
                    rotation = shape.GetRotation()
                });
            }
            else if (shape is BezierCurve bezier)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "Bezier",
                    position1 = bezier.P0,
                    position2 = bezier.P1,
                    position3 = bezier.P2,
                    position4 = bezier.P3,
                    Color = bezier.Color,
                    offsetPosition = shape.Offset,
                    rotation = shape.GetRotation()
                });
            }
            else if (shape is BezierNCurve bezierN)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "BezierN",
                    controlPoints = new List<Vector2>(bezierN.GetControlPoints()),
                    Color = bezierN.Color,
                    offsetPosition = shape.Offset,
                    rotation = shape.GetRotation()
                });
            }

        }

        ShapeDataWrapper wrapper = new ShapeDataWrapper
        {
            shapes = shapeDatas,
            gridData = new GridData
            {
                gridSize = gridDraw.GridSize,
                gridMin = gridDraw.GridAreaMin,
                gridMax = gridDraw.GridAreaMax
            }
        };

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);

    }

    public List<Shape> LoadShapes()
    {

        var extensions = new[] {
        new ExtensionFilter("JSON Files", "json")
    };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Load Shape File", "", extensions, false);

        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            DebugLogUI.Instance.Log("Load cancelled.");
            return null;
        }

        string path = paths[0];
        string json = File.ReadAllText(path);

        var wrapper = JsonUtility.FromJson<ShapeDataWrapper>(json);

        if (wrapper.gridData != null)
        {
            gridDraw.SetGridSettings(wrapper.gridData.gridSize, wrapper.gridData.gridMin, wrapper.gridData.gridMax);
        }

        List<Shape> loadedShapes = new List<Shape>();

        foreach (var data in wrapper.shapes)
        {
            Shape shape = null;

            switch (data.type)
            {
                case "Line":
                    shape = new Line(data.position1, data.position2, data.Color);
                    break;
                case "Circle":
                    shape = new Circle(data.position1, data.radius, data.Color, data.isFill);
                    break;
                case "Ellipse":
                    shape = new Ellipse(data.position1, data.radiusX, data.radiusY, data.Color, data.isFill);
                    break;
                case "Hermite":
                    shape = new HermiteCurve(data.position1, data.position2, data.position3, data.position4, data.Color);
                    break;
                case "Bezier":
                    shape = new BezierCurve(data.position1, data.position2, data.position3, data.position4, data.Color);
                    break;
                case "BezierN":
                    shape = new BezierNCurve(data.controlPoints, data.Color);
                    break;
            }

            if (shape != null)
            {
                //shape.MoveOffset(data.offsetPosition);
                //shape.SetRotation(data.rotation);
                loadedShapes.Add(shape);
            }
        }

        foreach (var shape in loadedShapes)
        {
            SelectionManager.Instance.RegisterShape(shape.parentObject, shape);
            shape.Draw();
        }

        return loadedShapes;
    }

    [System.Serializable]
    private class ShapeDataWrapper
    {
        public List<ShapeData> shapes;
        public GridData gridData;
    }

    [System.Serializable]
    public class ShapeData
    {
        public string type;
        public Vector2 position1;
        public Vector2 position2;
        public Vector2 position3;
        public Vector2 position4;
        public int radius;
        public int radiusX;
        public int radiusY;

        public List<Vector2> controlPoints;

        public Vector2 offsetPosition;
        public float rotation;

        public bool isFill;

        public string colorHex;

        public Color Color
        {
            get => ColorFromHex(colorHex);
            set => colorHex = ColorToHex(value);
        }

        private static string ColorToHex(Color color)
        {
            Color32 c32 = color;
            return $"#{c32.r:X2}{c32.g:X2}{c32.b:X2}{c32.a:X2}";
        }

        private static Color ColorFromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length < 7) return Color.white;

            byte r = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = hex.Length >= 9 ? byte.Parse(hex.Substring(7, 2), System.Globalization.NumberStyles.HexNumber) : (byte)255;

            return new Color32(r, g, b, a);
        }
    }

    [System.Serializable]
    public class GridData
    {
        public float gridSize;
        public Vector2Int gridMin;
        public Vector2Int gridMax;
    }

}