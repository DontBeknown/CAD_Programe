using UnityEngine;

public class ShapeMover
{
    private GameObject targetShape;
    private bool isMoving;
    private Vector2 offset;
    private Vector2 originalPosition;

    public bool IsMoving => isMoving;

    public void StartMove(GameObject shape, Vector2 mouseWorldPos)
    {
        targetShape = shape;
        originalPosition = targetShape.transform.position;
        offset = originalPosition - mouseWorldPos;
        isMoving = true;
    }

    public void UpdateMove(Vector2 mouseWorldPos)
    {
        if (targetShape == null || !isMoving) return;

        Vector2 newCenter = mouseWorldPos + offset;
        targetShape.transform.position = newCenter;
    }

    public void ConfirmMove()
    {
        isMoving = false;
        DebugLogUI.Instance.Log($"Moved shape to {targetShape.transform.position}");
        targetShape = null;
    }

    public void CancelMove()
    {
        if (targetShape != null)
        {
            targetShape.transform.position = originalPosition;
        }

        isMoving = false;
        targetShape = null;
    }
}
