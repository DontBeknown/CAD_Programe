using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : Shape
{
    public Vector2 P0 { get; private set; }
    public Vector2 P1 { get; private set; }
    public Vector2 P2 { get; private set; }
    public Vector2 P3 { get; private set; }

    public BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        : base(p0, color)
    {
        P0 = p0;
        P1 = p1;
        P2 = p2;
        P3 = p3;

        int resolution = CalculateDynamicResolution(P0, P1, P2, P3);
        points = GenerateBezierCurve(resolution);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject($"BezierCurve_{P0}_to_{P3}");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";
    }

    private int CalculateDynamicResolution(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float approxLength =
            Vector2.Distance(p0, p1) +
            Vector2.Distance(p1, p2) +
            Vector2.Distance(p2, p3);

        int resolution = Mathf.Clamp(Mathf.CeilToInt(approxLength * 1.5f), 10, 300);
        return resolution;
    }

    private List<Vector2> GenerateBezierCurve(int resolution)
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            result.Add(ComputeBezier(t));
        }
        return result;
    }

    private Vector2 ComputeBezier(float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        return uuu * P0 + 3 * uu * t * P1 + 3 * u * tt * P2 + ttt * P3;
    }

    public override string GetDetails()
    {
        return $"Bezier Curve from {P0} to {P3} with controls {P1}, {P2}";
    }

    public override Vector2 GetCenter()
    {
        return (P0 + P1 + P2 + P3) / 4f;
    }
}