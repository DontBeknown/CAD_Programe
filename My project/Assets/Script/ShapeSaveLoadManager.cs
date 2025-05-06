using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ShapeSaveLoadManager : MonoBehaviour
{
    private string savePath => Application.persistentDataPath + "/shapes.json";

    public void SaveShapes(List<Shape> shapes)
    {
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
                });
            }
            else if (shape is Circle circle)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "Circle",
                    position1 = circle.CenterPoint,
                    radius = circle.Radius,
                    Color = circle.Color
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
                    Color = ellipse.Color
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
                    Color = hermite.Color
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
                    Color = bezier.Color
                });
            }
        }

        string json = JsonUtility.ToJson(new ShapeDataWrapper { shapes = shapeDatas }, true);
        File.WriteAllText(savePath, json);
    }

    public List<Shape> LoadShapes()
    {
        if (!File.Exists(savePath)) return new List<Shape>();

        string json = File.ReadAllText(savePath);
        var wrapper = JsonUtility.FromJson<ShapeDataWrapper>(json);
        List<Shape> loadedShapes = new List<Shape>();

        foreach (var data in wrapper.shapes)
        {
            switch (data.type)
            {
                case "Line":
                    loadedShapes.Add(new Line(data.position1, data.position2, data.Color));
                    break;
                case "Circle":
                    loadedShapes.Add(new Circle(data.position1, data.radius, data.Color));
                    break;
                case "Ellipse":
                    loadedShapes.Add(new Ellipse(data.position1, data.radiusX, data.radiusY, data.Color));
                    break;
                case "Hermite":
                    loadedShapes.Add(new HermiteCurve(data.position1, data.position2, data.position3, data.position4, data.Color));
                    break;
                case "Bezier":
                    loadedShapes.Add(new BezierCurve(data.position1, data.position2, data.position3, data.position4, data.Color));
                    break;

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
}