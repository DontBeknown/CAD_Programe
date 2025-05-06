using UnityEngine;
using System.Collections.Generic;

public class GridDraw : MonoBehaviour
{
    public float GridSize = 1f;
    public float LineWidth = 0.05f;
    public Material GridMaterial;
    public Vector2Int GridAreaMin = new Vector2Int(-50, -50);
    public Vector2Int GridAreaMax = new Vector2Int(50, 50);

    private Camera mainCamera;
    private GameObject gridParent;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    public bool isGridVisible = true;

    private Queue<LineRenderer> linePool = new Queue<LineRenderer>();

    void Start()
    {
        mainCamera = Camera.main;
        gridParent = new GameObject("GridLines");
        RenderGrid();
        
        if(!isGridVisible) gridParent.SetActive(isGridVisible);
    }

    LineRenderer GetLineRenderer()
    {
        if (linePool.Count > 0)
            return linePool.Dequeue();

        GameObject lineObject = new GameObject("GridLine");
        lineObject.transform.parent = gridParent.transform;
        LineRenderer lr = lineObject.AddComponent<LineRenderer>();
        lr.material = GridMaterial;
        lr.startWidth = LineWidth;
        lr.endWidth = LineWidth;
        lr.useWorldSpace = true;
        return lr;
    }
    void RenderGrid()
    {
        for (float x = GridAreaMin.x + 0.5f; x <= GridAreaMax.x; x += GridSize)
        {
            Vector3 start = new Vector3(x, GridAreaMin.y + 0.5f, 0);
            Vector3 end = new Vector3(x, GridAreaMax.y + 0.5f, 0);
            CreateLine(start, end);
        }

        for (float y = GridAreaMin.y + 0.5f; y <= GridAreaMax.y; y += GridSize)
        {
            Vector3 start = new Vector3(GridAreaMin.x + 0.5f, y, 0);
            Vector3 end = new Vector3(GridAreaMax.x + 0.5f, y, 0);
            CreateLine(start, end);
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        LineRenderer lr = GetLineRenderer();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });
        lr.gameObject.SetActive(true);
        lineRenderers.Add(lr);
    }

    public void ToggleGrid()
    {
        isGridVisible = !isGridVisible;
        gridParent.SetActive(isGridVisible);
    }
}