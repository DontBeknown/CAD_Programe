using System.Collections.Generic;
using UnityEngine;

public class BezierNCurve : Shape
{
    private List<Vector2> controlPoints;
    private List<GameObject> controlPointPixels = new List<GameObject>();

    public BezierNCurve(List<Vector2> controlPoints, Color color)
        : base(GetAveragePosition(controlPoints), color)
    {
        this.controlPoints = new List<Vector2>(controlPoints);

        int resolution = CalculateDynamicResolution(this.controlPoints);
        points = GenerateBezierPoints(resolution);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject("BezierNCurve");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";
    }

    private int CalculateDynamicResolution(List<Vector2> cps)
    {
        float length = 0f;
        for (int i = 0; i < cps.Count - 1; i++)
        {
            length += Vector2.Distance(cps[i], cps[i + 1]);
        }
        return Mathf.Clamp(Mathf.CeilToInt(length * 1.5f), 10, 300);
    }

    private List<Vector2> GenerateBezierPoints(int resolution)
    {
        List<Vector2> curve = new List<Vector2>();
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            curve.Add(ComputeBezierPoint(controlPoints, t));
        }
        return curve;
    }

    private Vector2 ComputeBezierPoint(List<Vector2> cps, float t)
    {
        int n = cps.Count - 1;
        Vector2 point = Vector2.zero;

        for (int i = 0; i <= n; i++)
        {
            float binomial = BinomialCoefficient(n, i);
            float term = binomial * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i);
            point += term * cps[i];
        }

        return point;
    }

    private int BinomialCoefficient(int n, int k)
    {
        int result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= n--;
            result /= i;
        }
        return result;
    }

    private static Vector2 GetAveragePosition(List<Vector2> points)
    {
        Vector2 sum = Vector2.zero;
        foreach (var p in points)
            sum += p;
        return sum / points.Count;
    }

    public override string GetDetails()
    {
        return $"Bezier-N Curve with {controlPoints.Count} control points.";
    }

    public List<Vector2> GetControlPoints()
    {
        return new List<Vector2>(controlPoints);
    }

    public override string GetValues()
    {
        string result = "";
        foreach (var p in controlPoints)
            result += $"{p.x:F0} {p.y:F0} ";
        result += ColorToString.Convert(Color);
        return result;
    }

    public override Vector2 GetCenter()
    {
        return GetAveragePosition(controlPoints);
    }

    public void ShowControlPoints(Color highlightColor)
    {
        HideControlPoints();

        foreach (var point in controlPoints)
        {
            GameObject pixel = PixelPool.Instance.GetPixel();
            pixel.transform.position = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), 0);
            pixel.transform.localScale = Vector3.one * 3f;
            pixel.transform.parent = parentObject.transform;
            pixel.GetComponent<Renderer>().material.color = highlightColor;

            controlPointPixels.Add(pixel);
        }
    }

    public void HideControlPoints()
    {
        foreach (var pixel in controlPointPixels)
        {
            if (pixel != null)
                PixelPool.Instance.ReturnPixel(pixel);
        }
        controlPointPixels.Clear();
    }

    public override void Highlight(Color highlightColor)
    {
        base.Highlight(highlightColor);
        ShowControlPoints(Color.red);
    }

    public override void ClearHighlight()
    {
        base.ClearHighlight();
        HideControlPoints();
    }

    public override void Clear()
    {
        base.Clear();
        HideControlPoints();
    }
}
