using UnityEngine;

public class ShapePreviewManager
{
    private Shape previewShape;

    public void UpdatePreview(InputMode mode, Vector2 start, Vector2 current)
    {
        ClearPreview();

        switch (mode)
        {
            case InputMode.DrawLine:
                previewShape = new Line(start, current, Color.gray);
                break;

            case InputMode.DrawCircle:
                int radius = Mathf.RoundToInt(Vector2.Distance(start, current));
                previewShape = new Circle(start, radius, Color.gray);
                break;

            case InputMode.DrawEllipse:
                int radiusX = Mathf.Abs(Mathf.RoundToInt(current.x - start.x));
                int radiusY = Mathf.Abs(Mathf.RoundToInt(current.y - start.y));
                previewShape = new Ellipse(start, radiusX, radiusY, Color.gray);
                break;
        }

        previewShape?.Draw();
    }

    public void UpdateCustomPreview(Shape shape)
    {
        ClearPreview();
        previewShape = shape;
        previewShape?.Draw();
    }

    public void DrawPreview()
    {
        previewShape?.Draw();
    }

    public void ClearPreview()
    {
        previewShape?.Clear();
        previewShape = null;
    }
}
