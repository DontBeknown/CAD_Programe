using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [Header("Key Bindings")]
    public KeyCode deleteKey = KeyCode.Delete;
    public KeyCode toggleGridKey = KeyCode.G;
    public KeyCode rotateKey = KeyCode.R;
    public KeyCode moveKey = KeyCode.F;
    public KeyCode colorKey = KeyCode.C;

    [Header("Mode Keys")]
    public KeyCode selectModeKey = KeyCode.Alpha1;
    public KeyCode lineKey = KeyCode.Alpha2;
    public KeyCode circleKey = KeyCode.Alpha3;
    public KeyCode ellipseKey = KeyCode.Alpha4;
    public KeyCode hermitKey = KeyCode.Alpha5;
    public KeyCode bezierNKey = KeyCode.Alpha6;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI coordinateText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject uiManager;
    
    public InputMode currentMode { get; set; } = InputMode.Select;
    public Color currentColor = Color.black;
    private bool isActiveInputCommand = false;

    private ShapeDrawer shapeDrawer;
    private ShapeCommandParser commandParser;
    private GridDraw grid;
    private SelectionManager selectionManager;
    private ShapeSaveLoadManager shapeSaveLoadManager;
    private ShapeRotationController rotationController;
    private ShapeMover shapeMover;
    private ShapeListUIManager shapeListUIManager;
    private ButtonManager buttonManager;

    void Start()
    {
        grid = GetComponent<GridDraw>();
        shapeSaveLoadManager = GetComponent<ShapeSaveLoadManager>();

        shapeListUIManager = uiManager.GetComponent<ShapeListUIManager>();
        buttonManager = uiManager.GetComponent<ButtonManager>();

        selectionManager = new SelectionManager(shapeSaveLoadManager, shapeListUIManager, this);
        shapeDrawer = new ShapeDrawer();
        rotationController = new ShapeRotationController();
        shapeMover = new ShapeMover();
        commandParser = new ShapeCommandParser(shapeDrawer, selectionManager, rotationController);

        buttonManager.saveButton.onClick.AddListener(() => selectionManager.SaveToFile());

    }

    void Update()
    {
        HandleModeSwitch();
        HandleGeneralKeys();
        HandleMouseClicks();
        HandleTextCommand();
        HandleRotationPreview();
        UpdateMouseDisplay();

        shapeDrawer.UpdatePreview(GetMousePosition(), currentMode);
        shapeDrawer.DrawPreview();
    }

    #region Mode Switching
    public void SetMode(InputMode mode)
    {
        if (currentMode == mode) return;

        InputMode prevMode = currentMode;
        currentMode = mode;

        if ((mode == InputMode.RotatePreview || mode == InputMode.Move) && selectionManager.GetSelectedShape() == null)
        {
            currentMode = prevMode;
        }

        shapeDrawer.CancelDrawing();
        inputField.text = "";
        DebugLogUI.Instance.Log($"Switched to {currentMode} mode");
        buttonManager.SetButtonMode(currentMode);

        if (!IsEditMode(currentMode))
            selectionManager.Deselect();
    }

    void HandleModeSwitch()
    {
        if (Input.GetKeyDown(selectModeKey)) SetMode(InputMode.Select);
        else if (Input.GetKeyDown(lineKey)) SetMode(InputMode.DrawLine);
        else if (Input.GetKeyDown(circleKey)) SetMode(InputMode.DrawCircle);
        else if (Input.GetKeyDown(ellipseKey)) SetMode(InputMode.DrawEllipse);
        else if (Input.GetKeyDown(hermitKey)) SetMode(InputMode.DrawHermit);
        else if (Input.GetKeyDown(bezierNKey)) SetMode(InputMode.DrawNBezier);
        else if (Input.GetKeyDown(colorKey)) SetMode(InputMode.ColorPick);
    }

    bool IsEditMode(InputMode mode) =>
        mode == InputMode.Select || mode == InputMode.Move || mode == InputMode.RotatePreview;
    #endregion

    #region General Keys
    void HandleGeneralKeys()
    {
        if (Input.GetKeyDown(toggleGridKey) && grid != null && !isActiveInputCommand)
        {
            grid.ToggleGrid();
            DebugLogUI.Instance.Log($"Grid toggled: {grid.isGridVisible}");
        }

        if ((Input.GetKeyDown(deleteKey) || Input.GetKeyDown(KeyCode.Backspace)) 
            && currentMode == InputMode.Select && !isActiveInputCommand)
        {
            selectionManager.DeleteSelected();
            inputField.text = "";
        }
            

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(KeyCode.S))
        {
            selectionManager.SaveToFile();
            DebugLogUI.Instance.Log("Save completed");
        }

        if (Input.GetKeyDown(rotateKey) && currentMode == InputMode.Select && !isActiveInputCommand)
        {
            currentMode = InputMode.RotatePreview;
            rotationController.StartRotation(selectionManager.GetSelectedShape());
            DebugLogUI.Instance.Log("Rotate mode started. Move mouse to rotate, click to confirm, ESC to cancel.");
            inputField.text = "";
        }

        if (Input.GetKeyDown(moveKey) && currentMode == InputMode.Select && !isActiveInputCommand)
        {
            currentMode = InputMode.Move;
            shapeMover.StartMove(selectionManager.GetSelectedObject(), GetMousePosition());
            DebugLogUI.Instance.Log("Move mode started. Click to confirm, ESC to cancel.");
            inputField.text = "";
        }

        if (Input.GetKeyDown(KeyCode.Return) && currentMode == InputMode.DrawNBezier)
        {
            shapeDrawer.FinalizeBezierN(currentColor);
        }
    }
    #endregion

    #region Mouse Input
    void HandleMouseClicks()
    {
        Vector2 mouse = GetMousePosition();

        if (Input.GetMouseButtonDown(0))
        {
            switch (currentMode)
            {
                case InputMode.Select:
                    TrySelect(mouse);
                    break;
                case InputMode.Move when shapeMover.IsMoving:
                    shapeMover.ConfirmMove();
                    currentMode = InputMode.Select;
                    DebugLogUI.Instance.Log("Move confirmed.");
                    break;
                default:
                    shapeDrawer.OnMouseClick(currentMode, mouse, currentColor);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentMode == InputMode.Move && shapeMover.IsMoving)
            {
                shapeMover.CancelMove();
                currentMode = InputMode.Select;
                DebugLogUI.Instance.Log("Move canceled.");
            }
            else if (!IsEditMode(currentMode))
            {
                shapeDrawer.CancelDrawing();
                DebugLogUI.Instance.Log("Drawing canceled.");
            }
        }

        if (currentMode == InputMode.Move && shapeMover.IsMoving)
        {
            shapeMover.UpdateMove(mouse);
        }
    }

    void TrySelect(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapPoint(pos);
        if (hit && hit.transform.parent?.parent?.CompareTag("Selectable") == true)
            selectionManager.Select(hit.transform.parent.parent.gameObject);
        else
        {
            selectionManager.Deselect();
            inputField.text = "";
        }
    }
    #endregion

    #region Text Commands
    void HandleTextCommand()
    {
        if (Input.GetMouseButtonDown(1))
            ToggleInputField();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            commandParser.ParseCommand(currentMode, inputField.text, selectionManager.GetSelectedShape());
            inputField.text = "";

            if (currentMode == InputMode.Move || currentMode == InputMode.RotatePreview)
                currentMode = InputMode.Select;

            if (selectionManager.HasSelection())
                GetShapeValue(selectionManager.GetSelectedShape());
        }

        if (inputField.placeholder is TextMeshProUGUI placeholder)
        {
            placeholder.text = GetPlaceholderText(currentMode);
        }
    }

    string GetPlaceholderText(InputMode mode) => mode switch
    {
        InputMode.DrawLine => "<X0> <Y0> <X1> <Y1> <Color>",
        InputMode.DrawCircle => "<X0> <Y0> <R> <isFill> <Color>",
        InputMode.DrawEllipse => "<X0> <Y0> <Rx> <Ry> <isFill> <Color>",
        InputMode.DrawHermit => "<P0> <P1> <T0> <T1> <Color>",
        InputMode.RotatePreview => "<Angle>",
        InputMode.Move => "<X> <Y>",
        _ => ""
    };
    #endregion

    #region Rotation Preview
    void HandleRotationPreview()
    {
        if (currentMode != InputMode.RotatePreview || !rotationController.IsRotating)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            rotationController.CancelRotation();
            currentMode = InputMode.Select;
            DebugLogUI.Instance.Log("Rotation canceled.");
        }
        else if (Input.GetMouseButtonDown(0))
        {
            rotationController.ConfirmRotation();
            currentMode = InputMode.Select;
            DebugLogUI.Instance.Log("Rotation confirmed.");
        }
        else
        {
            rotationController.UpdateRotationPreview(GetMousePosition());
        }
    }
    #endregion

    #region Utility
    Vector2 GetMousePosition()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float x = Mathf.Clamp(mouse.x, grid.GridAreaMin.x, grid.GridAreaMax.x);
        float y = Mathf.Clamp(mouse.y, grid.GridAreaMin.y, grid.GridAreaMax.y);

        if (currentMode != InputMode.DrawLine && currentMode != InputMode.DrawCircle && currentMode != InputMode.DrawEllipse &&
            currentMode != InputMode.DrawHermit && currentMode != InputMode.DrawNBezier)
            return new Vector2(x, y);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            return SnapToGrid(mouse);
        }

        return new Vector2(x, y);
    }

    Vector2 SnapToGrid(Vector2 position)
    {
       
        float gridSize = grid.GridSize;
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;

        return new Vector2(x, y);
    }

    void UpdateMouseDisplay()
    {
        if (coordinateText != null)
        {
            Vector2 world = GetMousePosition();
            Vector2 snapped = shapeDrawer.SnapToGrid(world);
            coordinateText.text = $"Mouse: ({snapped.x}, {snapped.y})";
        }
    }

    public void ToggleInputField()
    {
        if (isActiveInputCommand) DeactivateInputField();
        else ActivateInputField();
    }

    void ActivateInputField()
    {
        if (inputField == null) return;

        DebugLogUI.Instance.Log("Activate command");

        inputField.interactable = true;
        inputField.Select();
        inputField.ActivateInputField();
        isActiveInputCommand = true;
    }

    void DeactivateInputField()
    {
        if (inputField == null) return;

        DebugLogUI.Instance.Log("Deactivate command");

        inputField.DeactivateInputField();
        EventSystem.current?.SetSelectedGameObject(null);

        if (inputField.placeholder is TextMeshProUGUI placeholder)
            placeholder.text = "";

        isActiveInputCommand = false;
    }

    public void GetShapeValue(Shape shape)
    {
        inputField.text = shape.GetValues();
    }

    public bool isSelected()
    {
        return selectionManager.GetSelectedShape() != null;
    }

    public void Newfile()
    {
        selectionManager.ClearAllShapes();
    }
    public void LoadFile()
    {
        selectionManager.ClearAllShapes();
        selectionManager.LoadFromFile();
    }

    #endregion
}




public enum InputMode
{
    Select,
    DrawLine,
    DrawCircle,
    DrawEllipse,
    DrawHermit,
    DrawNBezier,
    RotatePreview,
    Move,
    ColorPick,
}

