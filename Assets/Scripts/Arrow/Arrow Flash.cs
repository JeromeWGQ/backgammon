using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFlash : MonoBehaviour
{
    public Renderer targetRenderer;
    public float minAlpha = 0.5f;
    public float maxAlpha = 0.75f;
    public float flashSpeed = 2.0f;

    private float targetAlpha;
    private bool isIncreasing = true;

    void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
        targetAlpha = minAlpha;
    }

    void Update()
    {
        if (isIncreasing)
        {
            targetAlpha += Time.deltaTime * flashSpeed;
            if (targetAlpha >= maxAlpha)
            {
                isIncreasing = false;
            }
        }
        else
        {
            targetAlpha -= Time.deltaTime * flashSpeed;
            if (targetAlpha <= minAlpha)
            {
                isIncreasing = true;
            }
        }

        Color currentColor = targetRenderer.material.color;
        targetRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
}
