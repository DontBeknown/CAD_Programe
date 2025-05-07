using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float ZoomSpeed = 10f;
    public float PanSpeed = 0.5f;
    public float MaxZoom = 100f;
    public float MinZoom = 2f;
    public Rect PanLimits = new Rect(-50, -50, 100, 100);

    GridDraw gridDraw;

    private Vector3 lastMousePosition;

    void Start()
    {
        gridDraw = GetComponent<GridDraw>();
        if (gridDraw != null)
        {
            PanLimits = new Rect(gridDraw.GridAreaMin.x, gridDraw.GridAreaMin.y,
                gridDraw.GridAreaMax.x - gridDraw.GridAreaMin.x, gridDraw.GridAreaMax.y - gridDraw.GridAreaMin.y);
        }
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll * ZoomSpeed * Time.deltaTime * 100f;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinZoom, MaxZoom);

        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0) * PanSpeed * Time.deltaTime;
            Camera.main.transform.Translate(move, Space.Self);
            lastMousePosition = Input.mousePosition;
        }

        Vector3 pos = Camera.main.transform.position;
        pos.x = Mathf.Clamp(pos.x, PanLimits.xMin, PanLimits.xMax);
        pos.y = Mathf.Clamp(pos.y, PanLimits.yMin, PanLimits.yMax);
        Camera.main.transform.position = pos;
    }
}
