using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Line : Shape
{
    public Vector2 StartPoint { get; private set; }
    public Vector2 EndPoint { get; private set; }

    public Line(Vector2 startPoint, Vector2 endPoint, Color color)
        : base(startPoint, color)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;

        points = BresenhamLine(StartPoint, EndPoint);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject($"Line_{StartPoint}_to_{EndPoint}");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";
    }

    private List<Vector2> BresenhamLine(Vector2 start, Vector2 end)
    {
        List<Vector2> result = new List<Vector2>();

        int x0 = Mathf.RoundToInt(start.x);
        int y0 = Mathf.RoundToInt(start.y);
        int x1 = Mathf.RoundToInt(end.x);
        int y1 = Mathf.RoundToInt(end.y);

        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            result.Add(new Vector2(x0, y0));
            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }

        return result;
    }

    public override string GetDetails()
    {
        return $"Line from {StartPoint:F0} to {EndPoint:F0}";
    }

    public override Vector2 GetCenter()
    {
        return (StartPoint + EndPoint) / 2f;
    }
}
