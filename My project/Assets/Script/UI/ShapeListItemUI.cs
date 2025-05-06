using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShapeListItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    private GameObject shapeObject;

    public void Setup(GameObject obj, string shapeName)
    {
        shapeObject = obj;
        label.text = shapeName;
    }

    public void OnClick()
    {
        SelectionManager.Instance.Select(shapeObject);
    }
}
