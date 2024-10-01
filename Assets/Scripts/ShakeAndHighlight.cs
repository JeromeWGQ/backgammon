using UnityEngine;
using System.Collections;

public class ShakeAndHighlight : MonoBehaviour
{
    // Variables to control shake and highlight  
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
    public Color highlightColor = Color.yellow;
    public float highlightDuration = 0.2f;

    private Renderer rendererComp;
    private Color originalColor;
    private Vector3 originalPosition;

    void Start()
    {
        // Get the renderer component and store the original color  
        rendererComp = GetComponent<Renderer>();
        if (rendererComp != null)
        {
            originalColor = rendererComp.material.color;
        }
    }

    // Function to be called to trigger the shake and highlight  
    public void TriggerShakeAndHighlight()
    {
        // Store the original position  
        originalPosition = transform.position;
        StartCoroutine(ShakeRoutine());
        StartCoroutine(HighlightRoutine());
    }

    // Coroutine for shaking the object  
    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        Vector3 offset = Vector3.zero;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = Random.Range(-shakeMagnitude, shakeMagnitude);
            float z = Random.Range(-shakeMagnitude, shakeMagnitude);

            offset = new Vector3(x, y, z);
            transform.position = originalPosition + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    // Coroutine for highlighting the object  
    private IEnumerator HighlightRoutine()
    {
        float elapsed = 0f;

        while (elapsed < highlightDuration)
        {
            if (rendererComp != null)
            {
                rendererComp.material.color = Color.Lerp(originalColor, highlightColor, elapsed / highlightDuration);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (rendererComp != null)
        {
            rendererComp.material.color = originalColor;
        }
    }
}