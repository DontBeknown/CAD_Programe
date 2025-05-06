using System.Collections.Generic;
using UnityEngine;

public class Ellipse : Shape
{
    public Vector2 CenterPoint { get; private set; }
    public int RadiusX { get; private set; }
    public int RadiusY { get; private set; }

    public Ellipse(Vector2 centerPoint, int radiusX, int radiusY, Color color)
        : base(centerPoint, color)
    {
        CenterPoint = centerPoint;
        RadiusX = radiusX;
        RadiusY = radiusY;

        points = MidpointEllipseAlgorithm(CenterPoint, RadiusX, RadiusY);
        originalPoints = new List<Vector2>(points);

        parentObject = new GameObject($"Ellipse_{CenterPoint}_Rx{RadiusX}_Ry{RadiusY}");
        parentObject.transform.position = GetCenter();
        parentObject.tag = "Selectable";
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

    public override string GetDetails()
    {
        return $"Ellipse Center: {CenterPoint}, RadiusX: {RadiusX}, RadiusY: {RadiusY}";
    }

    public override Vector2 GetCenter()
    {
        return CenterPoint;
    }
}
