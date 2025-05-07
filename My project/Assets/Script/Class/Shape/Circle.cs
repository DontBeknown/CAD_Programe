using System.Collections.Generic;
using UnityEngine;

public class Circle : Shape
{
    public Vector2 CenterPoint { get; private set; }
    public int Radius { get; private set; }
    public bool Fill { get; private set; }

    private GameObject fillObject;
    private MeshFilter fillMeshFilter;
    private MeshRenderer fillMeshRenderer;

    private const float FillZOffset = 0.5f;
    private const int MeshSegments = 32;

    public Circle(Vector2 centerPoint, int radius, Color color, bool fill = false)
        : base(centerPoint, color)
    {
        CenterPoint = centerPoint;
        Radius = radius;
        Fill = fill;

        points = MidpointCircleAlgorithm(centerPoint, radius);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject($"Circle_{centerPoint}_R{radius}");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";

        if (Fill)
        {
            SetupFillMesh();
        }
    }

    private List<Vector2> MidpointCircleAlgorithm(Vector2 center, int radius)
    {
        var result = new List<Vector2>();
        int x = 0, y = radius;
        int d = 1 - radius;

        while (x <= y)
        {
            result.AddRange(new Vector2[]
            {
                new(center.x + x, center.y + y),
                new(center.x - x, center.y + y),
                new(center.x + x, center.y - y),
                new(center.x - x, center.y - y),
                new(center.x + y, center.y + x),
                new(center.x - y, center.y + x),
                new(center.x + y, center.y - x),
                new(center.x - y, center.y - x)
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

        return result;
    }

    private void SetupFillMesh()
    {
        if (fillObject == null)
        {
            fillObject = new GameObject("FillMesh");
            fillObject.transform.SetParent(parentObject.transform);
            fillObject.transform.localPosition = new Vector3(0, 0, FillZOffset);

            fillMeshFilter = fillObject.AddComponent<MeshFilter>();
            fillMeshRenderer = fillObject.AddComponent<MeshRenderer>();
            fillMeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        fillMeshFilter.mesh = GenerateFilledCircleMesh(Radius, MeshSegments);
        UpdateFillColor();
    }

    private Mesh GenerateFilledCircleMesh(float radius, int segments)
    {
        var mesh = new Mesh();
        var vertices = new List<Vector3> { Vector3.zero };
        var triangles = new List<int>();

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            vertices.Add(new Vector3(x, y, 0));
        }

        for (int i = 1; i < vertices.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private void UpdateFillColor()
    {
        if (fillMeshRenderer != null)
        {
            Color displayColor = isHighlighted ? highlightColor : Color.Lerp(Color, Color.white, 0.5f);
            fillMeshRenderer.material.color = displayColor;
        }
    }

    public void SetPoints(Vector2 newCenter, int newRadius, bool fill = false)
    {
        CenterPoint = newCenter;
        Radius = newRadius;
        Fill = fill;

        points = MidpointCircleAlgorithm(CenterPoint, Radius);
        originalPoints = new List<Vector2>(points);
        Position = GetCenter();

        if (parentObject != null)
        {
            parentObject.transform.position = Position;
        }

        if (Fill)
        {
            SetupFillMesh();
        }
        else
        {
            if (fillObject != null)
            {
                GameObject.Destroy(fillObject);
                fillObject = null;
            }
        }

        Redraw();
    }

    public override void Clear()
    {
        base.Clear();

        if (fillObject != null)
        {
            GameObject.Destroy(fillObject);
            fillObject = null;
        }
    }

    protected override void UpdateAllPixelsColor(Color color)
    {
        base.UpdateAllPixelsColor(color);
        UpdateFillColor();
    }

    public override string GetDetails()
    {
        return $"Circle Center: {CenterPoint:F0}, Radius: {Radius:F0}, Fill: {Fill}";
    }

    public override string GetValues()
    {
        return $"{CenterPoint.x:F0} {CenterPoint.y:F0} {Radius:F0} {(Fill ? "true" : "false")} {ColorToString.Convert(Color)}";
    }

    public override Vector2 GetCenter()
    {
        return CenterPoint;
    }
}
