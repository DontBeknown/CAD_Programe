using UnityEngine;

public class ShapeRotationController
{
    private Shape rotatingShape;
    public float originalRotation;
    public void StartRotation(Shape shape)
    {
        if (shape == null) return;
        rotatingShape = shape;
        originalRotation = shape.GetRotation();
    }

    public void UpdateRotationPreview(Vector2 mousePosition)
    {
        if (rotatingShape == null) return;

        Vector2 center = rotatingShape.GetCenter();
        Vector2 dir = mousePosition - center;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotatingShape.SetRotation(angle);
    }

    public void ConfirmRotation()
    {
        rotatingShape = null;
    }

    public void CancelRotation()
    {
        if (rotatingShape != null)
            rotatingShape.SetRotation(originalRotation);

        rotatingShape = null;
    }

    public bool IsRotating => rotatingShape != null;

    public void ApplyNumericRotation(float angle)
    {
        if (rotatingShape == null) return;

        rotatingShape.SetRotation(angle);
    }
}
