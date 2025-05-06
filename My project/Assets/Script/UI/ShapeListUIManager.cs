using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShapeListUIManager : MonoBehaviour
{
    public GameObject listItemPrefab;
    public Transform listContentParent;

    private void Update()
    {
        
    }
    public void RefreshList(Dictionary<GameObject, Shape> shapeRegistry)
    {
        foreach (Transform child in listContentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var kvp in shapeRegistry)
        {
            GameObject listItem = Instantiate(listItemPrefab, listContentParent);
            var ui = listItem.GetComponent<ShapeListItemUI>();
            ui.Setup(kvp.Key, kvp.Value.GetDetails());
        }
    }
}