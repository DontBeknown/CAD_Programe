using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ShapeSaveLoadManager;

public class SelectionManager
{
    public static SelectionManager Instance { get; private set; }

    private GameObject selectedObject;
    private Shape selectedShape;
    private Dictionary<GameObject, Shape> shapeRegistry = new Dictionary<GameObject, Shape>();
    private ShapeSaveLoadManager saveLoadManager;

    private ShapeListUIManager shapeListUIManager;
    public SelectionManager(ShapeSaveLoadManager saveLoadManager,ShapeListUIManager shapeListUIManager)
    {
        Instance = this;
        this.saveLoadManager = saveLoadManager;
        this.shapeListUIManager = shapeListUIManager;
        }
    public void RegisterShape(GameObject obj, Shape shape)
    {
        if (!shapeRegistry.ContainsKey(obj))
        {
            shapeRegistry.Add(obj, shape);
            shapeListUIManager.RefreshList(shapeRegistry);
        }
    }
    public void Select(GameObject obj)
    {
        if (selectedObject == obj) return;

        Deselect();

        selectedObject = obj;
        if (shapeRegistry.TryGetValue(obj, out Shape shape))
        {
            string detail = shape.GetDetails();
            selectedShape = shape;
            shape.Highlight(Color.darkOrange);
            DebugLogUI.Instance.Log("Select " + detail);
        }
    }

    public void Deselect()
    {
        if (selectedObject != null)
        {
            selectedShape.ClearHighlight();
            selectedObject = null;
        }
    }

    public void DeleteSelected()
    {
        if (selectedObject != null)
        {
            if (shapeRegistry.ContainsKey(selectedObject))
            {
                shapeRegistry.Remove(selectedObject);
                shapeListUIManager.RefreshList(shapeRegistry);
            }

            DebugLogUI.Instance.Log($"Delete {selectedObject.name}");
            Object.Destroy(selectedObject);
            selectedObject = null;

        }
    }

    public void SaveToFile()
    {
        List<Shape> shapesToSave = new List<Shape>(shapeRegistry.Values);
        saveLoadManager.SaveShapes(shapesToSave);
    }

    public void LoadFromFile()
    {
        List<Shape> loadedShape = saveLoadManager.LoadShapes();
        if (loadedShape == null || loadedShape.Count == 0)
        {
            DebugLogUI.Instance.Log("No shapes loaded.");
            return;
        }
        DebugLogUI.Instance.Log($"Loaded {loadedShape.Count} shapes.");
        shapeListUIManager.RefreshList(shapeRegistry);
    }

    public bool HasSelection()
    {
        return selectedShape != null;
    }

    public Shape GetSelectedShape()
    {
        return selectedShape;
    }

    public GameObject GetSelectedObject()
    {
        return selectedObject;
    }

}

