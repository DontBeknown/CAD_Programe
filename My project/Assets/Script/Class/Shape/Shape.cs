using System.Collections.Generic;
using UnityEngine;

public abstract class Shape
{
    public Vector2 Position { get; protected set; }
    public Color Color { get; protected set; }
    public GameObject parentObject;

    protected List<Vector2> points;
    protected List<Vector2> originalPoints;

    protected List<GameObject> drawnPixels;

    protected static Material sharedMaterial;

    protected bool isHighlighted = false;
    protected Color highlightColor;


    static Shape()
    {
        sharedMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    public Shape(Vector2 position, Color color)
    {
        Position = position;
        Color = color;
        points = new List<Vector2>();
        originalPoints = new List<Vector2>();
        drawnPixels = new List<GameObject>();
    }

    public void Draw()
    {
        foreach (var point in points)
        {
            DrawPoint(point);
        }

    }

    protected void DrawPoint(Vector2 position)
    {
        Vector2 snapped = new Vector2(Mathf.Round(position.x), Mathf.Round(position.y));
        GameObject pixel = PixelPool.Instance.GetPixel();
        pixel.transform.position = new Vector3(snapped.x, snapped.y, 0);
        pixel.transform.localScale = Vector3.one;
        pixel.transform.parent = parentObject.transform;
        pixel.GetComponent<Renderer>().material.color = Color;
        drawnPixels.Add(pixel);
    }

    public virtual void Redraw()
    {
        foreach (GameObject pixel in drawnPixels)
        {
            PixelPool.Instance.ReturnPixel(pixel);
        }
        drawnPixels.Clear();

        Draw();

        if (parentObject != null && isHighlighted)
        {
            UpdateAllPixelsColor(highlightColor);
        }
    }

    public abstract string GetDetails();

    public abstract string GetValues();

    public virtual void Clear()
    {
        foreach (GameObject pixel in drawnPixels)
        {
            PixelPool.Instance.ReturnPixel(pixel);
        }
        drawnPixels.Clear();

        if (parentObject != null)
        {
            GameObject.Destroy(parentObject);
            parentObject = null;
        }
    }

    public virtual void MoveOffset(Vector2 offset)
    {
        Position += offset;

        if (parentObject != null)
        {
            parentObject.transform.position += new Vector3(offset.x, offset.y, 0f);
        }

        for (int i = 0; i < points.Count; i++)
        {
            points[i] += offset;
        }
    }

    public virtual void MoveToPoint(Vector2 destination)
    {
        Vector2 offset = destination - GetCenter();
        Position = destination;

        if (parentObject != null)
        {
            parentObject.transform.position = new Vector3(destination.x, destination.y, 0f);
        }

        for (int i = 0; i < points.Count; i++)
        {
            points[i] += offset;
        }
    }


    public virtual void SetRotation(float angle)
    {
        if (parentObject != null)
        {
            parentObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        float radians = angle * Mathf.Deg2Rad;
        var sin = Mathf.Sin(radians);
        var cos = Mathf.Cos(radians);

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p = points[i] - GetCenter();
            float xNew = p.x * cos - p.y * sin;
            float yNew = p.x * sin + p.y * cos;
            points[i] = new Vector2(xNew, yNew) + GetCenter();
        }
    }

    public virtual float GetRotation()
    {
        return parentObject != null ? parentObject.transform.eulerAngles.z : 0f;
    }

    public virtual Vector2 GetCenter()
    {
        return Position;
    }

    public virtual void Recolor(Color newColor)
    {
        Color = newColor;

        if (!isHighlighted)
        {
            UpdateAllPixelsColor(newColor);
        }
    }

    protected virtual void UpdateAllPixelsColor(Color color)
    {
        foreach (GameObject pixel in drawnPixels)
        {
            if (pixel != null)
            {
                pixel.GetComponent<Renderer>().material.color = color;
            }
        }
    }

    public virtual void Highlight(Color highlightColor)
    {
        isHighlighted = true;
        this.highlightColor = highlightColor;
        UpdateAllPixelsColor(highlightColor);
    }

    public virtual void ClearHighlight()
    {
        isHighlighted = false;
        UpdateAllPixelsColor(Color);
    }

}

