using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float ZoomSpeed = 0.1f;
    public float PanSpeed = 0.2f;
    public float MaxZoom = 100f;
    public float MinZoom = 2f;
    public Rect PanLimits = new Rect(-50, -50, 100, 100);

    GridDraw gridDraw;

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
        Camera.main.orthographicSize -= scroll * ZoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinZoom, MaxZoom);

        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow)) move += Vector3.up * PanSpeed;
        if (Input.GetKey(KeyCode.DownArrow)) move += Vector3.down * PanSpeed;
        if (Input.GetKey(KeyCode.LeftArrow)) move += Vector3.left * PanSpeed;
        if (Input.GetKey(KeyCode.RightArrow)) move += Vector3.right * PanSpeed;

        Camera.main.transform.Translate(move, Space.World);

        Vector3 pos = Camera.main.transform.position;
        pos.x = Mathf.Clamp(pos.x, PanLimits.xMin, PanLimits.xMax);
        pos.y = Mathf.Clamp(pos.y, PanLimits.yMin, PanLimits.yMax);
        Camera.main.transform.position = pos;
    }

}

