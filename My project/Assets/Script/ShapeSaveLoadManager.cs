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
                    position2 = line.EndPoint
                });
            }
            else if (shape is Circle circle)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "Circle",
                    position1 = circle.CenterPoint,
                    radius = circle.Radius
                });
            }
            else if (shape is Ellipse ellipse)
            {
                shapeDatas.Add(new ShapeData
                {
                    type = "Ellipse",
                    position1 = ellipse.CenterPoint,
                    radiusX = ellipse.RadiusX,
                    radiusY = ellipse.RadiusY
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
                    loadedShapes.Add(new Line(data.position1, data.position2, Color.black));
                    break;
                case "Circle":
                    loadedShapes.Add(new Circle(data.position1, data.radius, Color.black));
                    break;
                case "Ellipse":
                    loadedShapes.Add(new Ellipse(data.position1, data.radiusX, data.radiusY, Color.black));
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
        public int radius;
        public int radiusX;
        public int radiusY;
    }
}