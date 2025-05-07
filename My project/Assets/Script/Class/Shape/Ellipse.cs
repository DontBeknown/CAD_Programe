using System.Collections.Generic;
using UnityEngine;

public class Ellipse : Shape
{
    public Vector2 CenterPoint { get; private set; }
    public int RadiusX { get; private set; }
    public int RadiusY { get; private set; }

    public bool Fill { get; private set; }

    private GameObject fillObject;
    private MeshRenderer fillMeshRenderer;
    private MeshFilter fillMeshFilter;
    public Ellipse(Vector2 centerPoint, int radiusX, int radiusY, Color color, bool fill = false)
        : base(centerPoint, color)
    {
        CenterPoint = centerPoint;
        RadiusX = radiusX;
        RadiusY = radiusY;
        Fill = fill;

        points = MidpointEllipseAlgorithm(CenterPoint, RadiusX, RadiusY);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject($"Ellipse_{CenterPoint}_Rx{RadiusX}_Ry{RadiusY}");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";

        if (Fill)
        {
            CreateFillMesh();
        }

    }

    private List<Vector2> MidpointEllipseAlgorithm(Vector2 center, int rx, int ry)
    {
        List<Vector2> ellipsePoints = new List<Vector2>();
        float x = 0, y = ry;

        float rxSq = rx * rx;
        float rySq = ry * ry;
        float dx = 2 * rySq * x;
        float dy = 2 * rxSq * y;

        float p1 = rySq - (rxSq * ry) + (0.25f * rxSq);

        while (dx < dy)
        {
            ellipsePoints.AddRange(GetSymmetricPoints(center, x, y));
            if (p1 < 0)
            {
                x++;
                dx += 2 * rySq;
                p1 += dx + rySq;
            }
            else
            {
                x++;
                y--;
                dx += 2 * rySq;
                dy -= 2 * rxSq;
                p1 += dx - dy + rySq;
            }
        }

        float p2 = rySq * (x + 0.5f) * (x + 0.5f) + rxSq * (y - 1) * (y - 1) - rxSq * rySq;

        while (y >= 0)
        {
            ellipsePoints.AddRange(GetSymmetricPoints(center, x, y));
            if (p2 > 0)
            {
                y--;
                dy -= 2 * rxSq;
                p2 += rxSq - dy;
            }
            else
            {
                y--;
                x++;
                dx += 2 * rySq;
                dy -= 2 * rxSq;
                p2 += dx - dy + rxSq;
            }
        }

        return ellipsePoints;
    }

    private List<Vector2> GetSymmetricPoints(Vector2 center, float x, float y)
    {
        return new List<Vector2>
        {
            new Vector2(center.x + x, center.y + y),
            new Vector2(center.x - x, center.y + y),
            new Vector2(center.x + x, center.y - y),
            new Vector2(center.x - x, center.y - y)
        };
    }

    private void CreateFillMesh()
    {
        fillObject = new GameObject("EllipseFill");
        fillObject.transform.SetParent(parentObject.transform);
        fillObject.transform.localPosition = new Vector3(0, 0, 0.5f);

        fillMeshFilter = fillObject.AddComponent<MeshFilter>();
        fillMeshRenderer = fillObject.AddComponent<MeshRenderer>();

        fillMeshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        fillMeshRenderer.material.color = isHighlighted ? highlightColor : Color.Lerp(Color, Color.white, 0.5f);

        fillMeshFilter.mesh = GenerateEllipseMesh(RadiusX, RadiusY, 64);
    }

    private Mesh GenerateEllipseMesh(float rx, float ry, int segments)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3> { Vector3.zero };
        List<int> triangles = new List<int>();

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * rx;
            float y = Mathf.Sin(angle) * ry;
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

    public void SetValues(Vector2 newCenter, int newRadiusX, int newRadiusY, bool fill = false)
    {
        CenterPoint = newCenter;
        RadiusX = newRadiusX;
        RadiusY = newRadiusY;
        Fill = fill;

        points = MidpointEllipseAlgorithm(CenterPoint, RadiusX, RadiusY);
        originalPoints = new List<Vector2>(points);

        Position = GetCenter();

        if (parentObject != null)
        {
            parentObject.transform.position = Position;
        }

        if (Fill)
        {
            if (fillObject == null)
            {
                CreateFillMesh();
            }
            else
            {
                fillMeshRenderer.material.color = isHighlighted ? highlightColor : Color.Lerp(Color, Color.white, 0.5f);
                fillMeshFilter.mesh = GenerateEllipseMesh(RadiusX, RadiusY, 64);
            }
        }
        else if (fillObject != null)
        {
            GameObject.Destroy(fillObject);
            fillObject = null;
        }

        Redraw();
    }

    protected override void UpdateAllPixelsColor(Color color)
    {
        base.UpdateAllPixelsColor(color);
        if (Fill && fillMeshRenderer != null)
        {
            fillMeshRenderer.material.color = Color.Lerp(color, Color.white, 0.5f);
        }
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

    public override string GetDetails()
    {
        return $"Ellipse Center: {CenterPoint:F0}, RadiusX: {RadiusX:F0}, RadiusY: {RadiusY:F0}";
    }

    public override string GetValues()
    {
        return $"{CenterPoint.x:F0} {CenterPoint.y:F0} {RadiusX:F0} {RadiusY:F0} {ColorToString.Convert(Color)}";
    }

    public override Vector2 GetCenter()
    {
        return CenterPoint;
    }
}
