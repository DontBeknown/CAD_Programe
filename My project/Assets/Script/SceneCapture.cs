using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;
using System.Collections;

public class SceneCapture : MonoBehaviour
{
    public Camera mainCamera;              // Your MAIN camera
    public Toggle gridToggle;              // Toggle to include/exclude grid
    public TMP_InputField inputWidth;      // CanvasInputX
    public TMP_InputField inputHeight;     // CanvasInputY
    public GridDraw gridDraw;              // Reference to GridDraw script

    private GameObject GridLinesObject => gridDraw != null ? gridDraw.GridParent : null;

    public void SaveSceneAsPNG()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save Screenshot", "", "scene", "png");
        if (!string.IsNullOrEmpty(path))
            StartCoroutine(CaptureScreenshot(path));
    }

    IEnumerator CaptureScreenshot(string path)
    {
        int width = int.TryParse(inputWidth.text, out var w) ? w : 1920;
        int height = int.TryParse(inputHeight.text, out var h) ? h : 1080;

        if (GridLinesObject != null)
            GridLinesObject.SetActive(gridToggle == null || gridToggle.isOn);

        yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(width, height, 24);
        mainCamera.targetTexture = rt;
        RenderTexture.active = rt;

        mainCamera.Render();

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] pngBytes = tex.EncodeToPNG();
        File.WriteAllBytes(path, pngBytes);
        Debug.Log("Saved PNG to: " + path);

        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        Destroy(tex);

        if (GridLinesObject != null)
            GridLinesObject.SetActive(true);
    }
}
