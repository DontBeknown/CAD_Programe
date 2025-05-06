using UnityEngine;
using System.Collections.Generic;

public class HermiteCurve : Shape
{
    public Vector2 P0 { get; private set; }
    public Vector2 P1 { get; private set; }
    public Vector2 T0 { get; private set; }
    public Vector2 T1 { get; private set; }

    public HermiteCurve(Vector2 p0, Vector2 p1, Vector2 t0, Vector2 t1, Color color)
        : base(p0, color)
    {
        P0 = p0;
        P1 = p1;
        T0 = t0;
        T1 = t1;

        int resolution = CalculateDynamicResolution(P0, P1, T0, T1);
        points = GenerateHermiteCurve(resolution);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject($"HermiteCurve_{P0}_to_{P1}");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";
    }

    private int CalculateDynamicResolution(Vector2 p0, Vector2 p1, Vector2 t0, Vector2 t1)
    {
        float length = Vector2.Distance(p0, p1);
        float tangentInfluence = (t0.magnitude + t1.magnitude) * 0.25f;
        float curveComplexity = length + tangentInfluence;

        int resolution = Mathf.Clamp(Mathf.CeilToInt(curveComplexity * 2f), 10, 300);
        return resolution;
    }

    private List<Vector2> GenerateHermiteCurve(int resolution)
    {
        List<Vector2> result = new List<Vector2>();
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            result.Add(ComputeHermite(t));
        }
        return result;
    }

    private Vector2 ComputeHermite(float t)
    {
        float h00 = 2 * t * t * t - 3 * t * t + 1;
        float h10 = t * t * t - 2 * t * t + t;
        float h01 = -2 * t * t * t + 3 * t * t;
        float h11 = t * t * t - t * t;

        return h00 * P0 + h10 * T0 + h01 * P1 + h11 * T1;
    }

    public override string GetDetails()
    {
        return $"Hermite Curve from {P0} to {P1} with tangents {T0}, {T1}";
    }

    public override Vector2 GetCenter()
    {
        return (P0 + P1) / 2f;
    }
}
