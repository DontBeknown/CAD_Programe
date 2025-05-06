using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ShapeDrawer
{
    private Vector2? startPoint = null;

    private ShapePreviewManager previewManager = new ShapePreviewManager();

    private List<Vector2> hermitePoints = new List<Vector2>();
    private List<Vector2> bezierPoints = new List<Vector2>();
    public void OnMouseClick(InputMode mode, Vector2 position)
    {
        Vector2 snapped = SnapToGrid(position);

        switch (mode)
        {
            case InputMode.DrawLine:
            case InputMode.DrawCircle:
            case InputMode.DrawEllipse:
                HandleDrawing((start, end) =>
                {
                    DrawShape(mode, start, end);
                }, mode, snapped);
                break;

            case InputMode.DrawHermit:
                HandleHermiteInput(snapped);
                break;
            case InputMode.DrawBezier:
                HandleBezierInput(snapped);
                break;
        }
    }

    public Vector2 SnapToGrid(Vector2 position)
    {
        float x = Mathf.Round(position.x);
        float y = Mathf.Round(position.y);
        return new Vector2(x, y);
    }

    public void DrawLine(Vector2 start, Vector2 end)
    {
        var line = new Line(start, end, Color.black);
        SelectionManager.Instance.RegisterShape(line.parentObject, line);
        line.Draw();
        DebugLogUI.Instance.Log($"Created line from {start} to {end}");
    }

    public void DrawCircle(Vector2 center, int radius)
    {
        var circle = new Circle(center, radius, Color.black);
        SelectionManager.Instance.RegisterShape(circle.parentObject, circle);
        circle.Draw();
        DebugLogUI.Instance.Log($"Created circle at {center} with radius {radius}");
    }

    public void DrawEllipse(Vector2 center, int radiusX, int radiusY)
    {
        var ellipse = new Ellipse(center, radiusX, radiusY, Color.black);
        SelectionManager.Instance.RegisterShape(ellipse.parentObject, ellipse);
        ellipse.Draw();
        DebugLogUI.Instance.Log($"Created ellipse at {center} with radiusX={radiusX}, radiusY={radiusY}");
    }

    public void DrawHermite(Vector2 p0, Vector2 p1, Vector2 t0, Vector2 t1)
    {
        var curve = new HermiteCurve(p0, p1, t0, t1, Color.black);
        SelectionManager.Instance.RegisterShape(curve.parentObject, curve);
        curve.Draw();
        DebugLogUI.Instance.Log($"Created Hermite curve: P0={p0}, P1={p1}, T0={t0}, T1={t1}");
    }

    public void DrawBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var curve = new BezierCurve(p0, p1, p2, p3, Color.black);
        SelectionManager.Instance.RegisterShape(curve.parentObject, curve);
        curve.Draw();
        DebugLogUI.Instance.Log($"Created Bézier curve: P0={p0}, P1={p1}, P2={p2}, P3={p3}");
    }
    private void HandleDrawing(System.Action<Vector2, Vector2> drawAction, InputMode mode, Vector2 snapped)
    {
        if (startPoint == null)
        {
            startPoint = snapped;
        }
        else
        {
            drawAction.Invoke(startPoint.Value, snapped);
            previewManager.ClearPreview();
            startPoint = null;
        }
    }

    private void DrawShape(InputMode mode, Vector2 start, Vector2 end)
    {
        switch (mode)
        {
            case InputMode.DrawLine:
                DrawLine(start, end);
                break;
            case InputMode.DrawCircle:
                int radius = Mathf.RoundToInt(Vector2.Distance(start, end));
                DrawCircle(start, radius);
                break;
            case InputMode.DrawEllipse:
                int radiusX = Mathf.Abs(Mathf.RoundToInt(end.x - start.x));
                int radiusY = Mathf.Abs(Mathf.RoundToInt(end.y - start.y));
                DrawEllipse(start, radiusX, radiusY);
                break;
        }
    }

    public void UpdatePreview(Vector2 mouseWorldPos, InputMode mode)
    {
        Vector2 snapped = SnapToGrid(mouseWorldPos);
        if (mode == InputMode.DrawHermit)
        {
            UpdateHermitePreview(snapped);
        }
        else if (startPoint.HasValue)
        {
            previewManager.UpdatePreview(mode, startPoint.Value, snapped);
        }
        else if (mode == InputMode.DrawBezier)
        {
            UpdateBezierPreview(snapped);
        }

    }

    public void DrawPreview()
    {
        previewManager.DrawPreview();
    }

    public void CancelDrawing()
    {
        startPoint = null;
        hermitePoints.Clear();
        previewManager.ClearPreview();
        DebugLogUI.Instance.Log("Drawing canceled.");
    }

    private void HandleHermiteInput(Vector2 point)
    {
        hermitePoints.Add(point);

        if (hermitePoints.Count == 4)
        {
            Vector2 p0 = hermitePoints[0];
            Vector2 p1 = hermitePoints[1];
            Vector2 t0 = hermitePoints[2] - p0;
            Vector2 t1 = hermitePoints[3] - p1;

            DrawHermite(p0, p1, t0, t1);

            hermitePoints.Clear();
            previewManager.ClearPreview();
        }
    }

    private void UpdateHermitePreview(Vector2 current)
    {
        var points = new List<Vector2>(hermitePoints) { current };

        switch (points.Count)
        {
            case 2:
                previewManager.UpdateCustomPreview(new Line(points[0], points[1], Color.gray));
                break;

            case 3:
                Vector2 t0 = points[2] - points[0];
                previewManager.UpdateCustomPreview(new HermiteCurve(points[0], points[1], t0, Vector2.zero, Color.gray));
                break;

            case 4:
                Vector2 t0full = points[2] - points[0];
                Vector2 t1full = points[3] - points[1];
                previewManager.UpdateCustomPreview(new HermiteCurve(points[0], points[1], t0full, t1full, Color.gray));
                break;

            default:
                previewManager.ClearPreview();
                break;
        }
    }

    private void HandleBezierInput(Vector2 point)
    {
        bezierPoints.Add(point);

        if (bezierPoints.Count == 4)
        {
            Vector2 p0 = bezierPoints[0];
            Vector2 p1 = bezierPoints[1];
            Vector2 p2 = bezierPoints[2];
            Vector2 p3 = bezierPoints[3];

            DrawBezier(p0, p1, p2, p3);

            bezierPoints.Clear();
            previewManager.ClearPreview();
        }
    }

    private void UpdateBezierPreview(Vector2 current)
    {
        var points = new List<Vector2>(bezierPoints) { current };

        switch (points.Count)
        {
            case 2:
            case 3:
                previewManager.UpdateCustomPreview(new Line(points[0], points[1], Color.gray));
                break;

            case 4:
                previewManager.UpdateCustomPreview(new BezierCurve(points[0], points[1], points[2], points[3], Color.gray));
                break;

            default:
                previewManager.ClearPreview();
                break;
        }
    }
}