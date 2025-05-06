using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class DebugLogUI : MonoBehaviour
{
    public static DebugLogUI Instance { get; private set; }

    [Header("UI Settings")]
    public GameObject logTextPrefab;  
    public Transform logContainer; 
    public float messageDuration = 3f;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Log(string message)
    {
        GameObject logGO = Instantiate(logTextPrefab, logContainer);
        TextMeshProUGUI text = logGO.GetComponent<TextMeshProUGUI>();
        text.text = message;
        StartCoroutine(FadeAndDestroy(logGO, messageDuration, fadeDuration));
    }

    private IEnumerator FadeAndDestroy(GameObject obj, float duration, float fadeTime)
    {
        yield return new WaitForSeconds(duration);

        TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
        Color originalColor = text.color;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(obj);
    }

    
}
