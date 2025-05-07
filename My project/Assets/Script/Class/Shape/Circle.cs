using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Circle : Shape
{
    public Vector2 CenterPoint { get; private set; }
    public int Radius { get; private set; }

    public Circle(Vector2 centerPoint, int radius, Color color)
        : base(centerPoint, color)
    {
        CenterPoint = centerPoint;
        Radius = radius;

        points = MidpointCircleAlgorithm(CenterPoint, Radius);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject($"Circle_{CenterPoint}_R{Radius}");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";

    }

    private List<Vector2> MidpointCircleAlgorithm(Vector2 center, int radius)
    {
        List<Vector2> circlePoints = new List<Vector2>();

        int x = 0, y = radius;
        int d = 1 - radius;

        while (x <= y)
        {
            circlePoints.AddRange(new Vector2[]
            {
                new Vector2(center.x + x, center.y + y),
                new Vector2(center.x - x, center.y + y),
                new Vector2(center.x + x, center.y - y),
                new Vector2(center.x - x, center.y - y),
                new Vector2(center.x + y, center.y + x),
                new Vector2(center.x - y, center.y + x),
                new Vector2(center.x + y, center.y - x),
                new Vector2(center.x - y, center.y - x)
            });

            if (d < 0)
                d += 2 * x + 3;
            else
            {
                d += 2 * (x - y) + 5;
                y--;
            }

            x++;
        }

        return circlePoints;
    }

    public void SetPoints(Vector2 newCenter, int newRadius)
    {
        CenterPoint = newCenter;
        Radius = newRadius;

        points = MidpointCircleAlgorithm(CenterPoint, Radius);
        originalPoints = new List<Vector2>(points);

        Position = GetCenter();

        if (parentObject != null)
        {
            parentObject.transform.position = Position;
        }

        Redraw();
    }

    public override string GetDetails()
    {
        return $"Circle Center: {CenterPoint:F0}, Radius: {Radius:F0}";
    }

    public override string GetValues()
    {
        return $"{CenterPoint.x:F0} {CenterPoint.y:F0} {Radius:F0} {ColorToString.Convert(Color)}";
    }

    public override Vector2 GetCenter()
    {
        return CenterPoint;
    }

}
