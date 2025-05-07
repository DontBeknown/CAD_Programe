using System.Collections.Generic;
using UnityEngine;

public class PixelPool : MonoBehaviour
{
    public static PixelPool Instance;
    public GameObject pixelPrefab;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetPixel()
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(pixelPrefab);

            GameObject colliderChild = new GameObject("PixelCollider");
            colliderChild.transform.parent = obj.transform;
            colliderChild.transform.localPosition = Vector3.zero;

            var box = colliderChild.AddComponent<BoxCollider2D>();
            box.size = Vector2.one;

        }

        return obj;
    }

    public void ReturnPixel(GameObject pixel)
    {
        pixel.transform.SetParent(null);
        pixel.SetActive(false);
        pool.Enqueue(pixel);
    }
}
