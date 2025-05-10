using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShapeListUIManager : MonoBehaviour
{
    public GameObject listItemPrefab;
    public Transform listContentParent;
    public GameObject listPanel;
    public Button layerButton;

    private bool isPanelOpen = false;

    private void Start()
    {
        layerButton.onClick.AddListener(() => ToggleLayerList());
        listPanel.SetActive(isPanelOpen);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleLayerList();
        }
    }
    public void RefreshList(Dictionary<GameObject, Shape> shapeRegistry)
    {
        foreach (Transform child in listContentParent)
        {
            Destroy(child.gameObject);
        }

        int i = 0;
        foreach (var kvp in shapeRegistry)
        {
            GameObject listItem = Instantiate(listItemPrefab, listContentParent);
            var ui = listItem.GetComponent<ShapeListItemUI>();
            ui.Setup(kvp.Key, kvp.Value.GetType().Name + $" {i}");
            i++;
        }
    }

    private void ToggleLayerList()
    {
        isPanelOpen = !isPanelOpen;
        listPanel.SetActive(isPanelOpen);
    }
}