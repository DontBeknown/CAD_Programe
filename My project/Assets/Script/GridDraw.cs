using UnityEngine;
using System.Collections.Generic;
using TMPro;

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

    public TMP_InputField gridSizeInput;
    public TMP_InputField gridWidthInput;
    public TMP_InputField gridHeightInput;

    private Queue<LineRenderer> linePool = new Queue<LineRenderer>();

    public GameObject GridParent => gridParent; // NEW: Expose gridParent

    void Start()
    {
        mainCamera = Camera.main;
        gridParent = new GameObject("GridLines");
        RenderGrid();

        gridParent.SetActive(isGridVisible);

        if (gridSizeInput != null)
        {
            gridSizeInput.text = GridSize.ToString();
            gridSizeInput.onValueChanged.AddListener(OnGridSizeChanged);
        }
        if (gridWidthInput != null)
            gridWidthInput.text = (GridAreaMax.x - GridAreaMin.x).ToString();
        if (gridHeightInput != null)
            gridHeightInput.text = (GridAreaMax.y - GridAreaMin.y).ToString();
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
        if (gridParent != null)
            gridParent.SetActive(isGridVisible);
    }

    public void RegenerateGrid()
    {
        foreach (var line in lineRenderers)
        {
            line.gameObject.SetActive(false);
            linePool.Enqueue(line);
        }

        lineRenderers.Clear();
        RenderGrid();
    }

    public void ApplyGridSettingsFromUI()
    {
        if (float.TryParse(gridSizeInput.text, out float newGridSize))
            GridSize = Mathf.Max(0.1f, newGridSize);

        if (int.TryParse(gridWidthInput.text, out int width))
        {
            int halfWidth = Mathf.Max(1, width / 2);
            GridAreaMin.x = -halfWidth;
            GridAreaMax.x = halfWidth;
        }

        if (int.TryParse(gridHeightInput.text, out int height))
        {
            int halfHeight = Mathf.Max(1, height / 2);
            GridAreaMin.y = -halfHeight;
            GridAreaMax.y = halfHeight;
        }

        RegenerateGrid();
    }

    public void SetGridSettings(float gridSize, Vector2Int gridMin, Vector2Int gridMax)
    {
        GridSize = gridSize;
        GridAreaMin = gridMin;
        GridAreaMax = gridMax;

        if (gridSizeInput != null)
            gridSizeInput.text = GridSize.ToString();

        if (gridWidthInput != null)
            gridWidthInput.text = (GridAreaMax.x - GridAreaMin.x).ToString();

        if (gridHeightInput != null)
            gridHeightInput.text = (GridAreaMax.y - GridAreaMin.y).ToString();

        RegenerateGrid();
    }

    private void OnGridSizeChanged(string newSize)
    {
        if (float.TryParse(newSize, out float parsedSize))
        {
            GridSize = Mathf.Max(0.1f, parsedSize);
            RegenerateGrid();
        }
    }
}
