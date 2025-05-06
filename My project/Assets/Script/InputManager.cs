using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    [Header("Key Bindings")]
    public KeyCode deleteKey = KeyCode.Delete;
    public KeyCode selectModeKey = KeyCode.Alpha1;
    public KeyCode lineKey = KeyCode.Alpha2;
    public KeyCode circleKey = KeyCode.Alpha3;
    public KeyCode ellipseKey = KeyCode.Alpha4;
    public KeyCode hermitKey = KeyCode.Alpha5;
    public KeyCode bezierKey = KeyCode.Alpha6;
    public KeyCode toggleGridKey = KeyCode.G;
    public KeyCode rotateKey = KeyCode.R;
    public KeyCode moveKey = KeyCode.F;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI coordinateText;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private ShapeListUIManager shapeListUIManager;
    private InputMode currentMode = InputMode.Select;
    private string currentInput = "";

    private ShapeDrawer shapeDrawer;
    private ShapeCommandParser commandParser;
    private GridDraw grid;
    private SelectionManager selectionManager;
    private ShapeSaveLoadManager shapeSaveLoadManager;
    private ShapeRotationController rotationController;
    private ShapeMover shapeMover;
    void Start()
    {
        grid = GetComponent<GridDraw>();
        shapeSaveLoadManager = GetComponent<ShapeSaveLoadManager>();

        selectionManager = new SelectionManager(shapeSaveLoadManager, shapeListUIManager);
        shapeDrawer = new ShapeDrawer();
        rotationController = new ShapeRotationController();
        shapeMover = new ShapeMover();
        commandParser = new ShapeCommandParser(shapeDrawer, selectionManager, rotationController);

        selectionManager.LoadFromFile();
    }

    void Update()
    {
        HandleModeSwitch();
        HandleKeyInput();
        HandleMouseInput();
        HandleTextInput();
        UpdateMouseCoordinateDisplay();

        shapeDrawer.UpdatePreview(GetClampedMousePosition(), currentMode);
        shapeDrawer.DrawPreview();

        HandleRotationInput();
    }

    void HandleModeSwitch()
    {
        InputMode previousMode = currentMode;

        if (Input.GetKeyDown(selectModeKey)) currentMode = InputMode.Select;
        else if (Input.GetKeyDown(lineKey)) currentMode = InputMode.DrawLine;
        else if (Input.GetKeyDown(circleKey)) currentMode = InputMode.DrawCircle;
        else if (Input.GetKeyDown(ellipseKey)) currentMode = InputMode.DrawEllipse;
        else if (Input.GetKeyDown(hermitKey)) currentMode = InputMode.DrawHermit;
        else if (Input.GetKeyDown(bezierKey)) currentMode = InputMode.DrawBezier;

        if (previousMode != currentMode)
        {
            shapeDrawer.CancelDrawing();
            DebugLogUI.Instance.Log($"Switched to {currentMode} mode");
        }
            

        if(currentMode != InputMode.Select && currentMode != InputMode.RotatePreview && currentMode != InputMode.Move)
            selectionManager.Deselect();
    }

    void HandleKeyInput()
    {
        if (Input.GetKeyDown(toggleGridKey) && grid != null)
        {
            grid.ToggleGrid();
            DebugLogUI.Instance.Log($"Grid toggled: {grid.isGridVisible}");
        }

        if (Input.GetKeyDown(deleteKey) && currentMode == InputMode.Select)
        {
            selectionManager.DeleteSelected();
        }

        bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        bool commandPressed = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
        bool sPressed = Input.GetKeyDown(KeyCode.S);

        if ((ctrlPressed || commandPressed) && sPressed)
        {
            selectionManager.SaveToFile();
            DebugLogUI.Instance.Log($"Save completed");
        }

        if (Input.GetKeyDown(rotateKey) && currentMode == InputMode.Select)
        {
            if (selectionManager.HasSelection())
            {
                currentMode = InputMode.RotatePreview;
                rotationController.StartRotation(selectionManager.GetSelectedShape());
                DebugLogUI.Instance.Log("Rotate mode started. Move mouse to rotate, click to confirm, ESC to cancel.");
            }
        }

        if (Input.GetKeyDown(moveKey) && currentMode == InputMode.Select)
        {
            if (selectionManager.HasSelection())
            {
                currentMode = InputMode.Move;
                Vector2 mousePos = GetClampedMousePosition();
                shapeMover.StartMove(selectionManager.GetSelectedObject(), mousePos);
                DebugLogUI.Instance.Log("Move mode started. Move mouse to reposition shape. Click to confirm, ESC to cancel.");
            }
        }
    }

    void HandleMouseInput()
    {
        Vector2 mousePos = GetClampedMousePosition();
        if (Input.GetMouseButtonDown(0))
        {
            if (currentMode == InputMode.Select)
            {
                TrySelectObject();
            }
            else if (currentMode == InputMode.Move && shapeMover.IsMoving)
            {
                shapeMover.ConfirmMove();
                currentMode = InputMode.Select;
                DebugLogUI.Instance.Log("Move confirmed.");
            }
            else
            {
                shapeDrawer.OnMouseClick(currentMode, mousePos);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMode == InputMode.Move && shapeMover.IsMoving)
            {
                shapeMover.CancelMove();
                currentMode = InputMode.Select;
                DebugLogUI.Instance.Log("Move cancelled.");
            }

            if (currentMode == InputMode.DrawLine ||
                currentMode == InputMode.DrawCircle ||
                currentMode == InputMode.DrawEllipse || 
                currentMode == InputMode.DrawHermit ||
                currentMode == InputMode.DrawBezier)
            {
                shapeDrawer.CancelDrawing();
            }
        }

        if (currentMode == InputMode.Move && shapeMover.IsMoving)
        {
            shapeMover.UpdateMove(mousePos);
        }
    }

    void HandleTextInput()
    {
        if (currentMode == InputMode.Select)
        {
            currentInput = "";
            displayText.text = "";
            return;
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key) && key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9)
            {
                currentInput += key.ToString().Replace("Keypad", "");
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            currentInput += "-";
        }

        if (Input.GetKeyDown(KeyCode.Space)) currentInput += " ";
        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
            currentInput = currentInput.Substring(0, currentInput.Length - 1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            commandParser.ParseCommand(currentMode, currentInput);
            currentInput = "";

            if (currentMode == InputMode.RotatePreview)
                currentMode = InputMode.Select;
        }

        displayText.text = currentInput;
    }

    void UpdateMouseCoordinateDisplay()
    {
        if (coordinateText == null) return;
        Vector2 world = GetClampedMousePosition();
        Vector2 snapped = shapeDrawer.SnapToGrid(world);
        coordinateText.text = $"Mouse: ({snapped.x}, {snapped.y})";
    }

    void TrySelectObject()
    {
        Vector2 mousePos = GetClampedMousePosition();
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null && hit.transform.parent?.parent != null)
        {
            GameObject parent = hit.transform.parent.parent.gameObject;
            if (parent.CompareTag("Selectable"))
            {
                selectionManager.Select(parent);
                return;
            }
        }

        selectionManager.Deselect();
    }

    void HandleRotationInput()
    {
        if (currentMode == InputMode.RotatePreview && rotationController.IsRotating)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                rotationController.CancelRotation();
                currentMode = InputMode.Select;
                DebugLogUI.Instance.Log("Rotation canceled.");
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                rotationController.ConfirmRotation();
                currentMode = InputMode.Select;
                DebugLogUI.Instance.Log("Rotation confirmed.");
                return;
            }

            Vector2 mouseWorld = GetClampedMousePosition();
            rotationController.UpdateRotationPreview(mouseWorld);
        }
    }

    Vector2 GetClampedMousePosition()
    {

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GridDraw grid = GetComponent<GridDraw>();
        mousePos.x = Mathf.Clamp(mousePos.x, grid.GridAreaMin.x, grid.GridAreaMax.x);
        mousePos.y = Mathf.Clamp(mousePos.y, grid.GridAreaMin.y, grid.GridAreaMax.y);

        return mousePos;
    }
}

public enum InputMode
{
    Select,
    DrawLine,
    DrawCircle,
    DrawEllipse,
    DrawHermit,
    DrawBezier,
    RotatePreview,
    Move,
}

