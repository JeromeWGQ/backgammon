using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighlightableImage : MonoBehaviour
{
    private Image imageComponent;
    private Vector2 originalSize;
    private float scaleFactor = 2f; // 放大倍数  
    private float transitionDuration = 0.3f; // 过渡持续时间（秒）  

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent != null && imageComponent.rectTransform != null)
        {
            originalSize = imageComponent.rectTransform.sizeDelta;
        }
        else
        {
            Debug.LogError("Image component or its rectTransform is missing!");
        }
    }

    // 协程：放大图像  
    public IEnumerator ScaleImageUpCoroutine()
    {
        Vector2 startSize = imageComponent.rectTransform.sizeDelta;
        Vector2 endSize = startSize * scaleFactor;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            // 使用EaseInOutQuad缓动函数  
            float ease = EaseInOutQuad(t);
            imageComponent.rectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, ease);

            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧  
        }

        imageComponent.rectTransform.sizeDelta = endSize; // 确保最终大小正确  
    }

    // 协程：恢复图像原始大小  
    public IEnumerator RestoreOriginalSizeCoroutine()
    {
        Vector2 startSize = imageComponent.rectTransform.sizeDelta;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            // 使用EaseInOutQuad缓动函数  
            float ease = EaseInOutQuad(t);
            imageComponent.rectTransform.sizeDelta = Vector2.Lerp(startSize, originalSize, ease);

            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧  
        }

        imageComponent.rectTransform.sizeDelta = originalSize; // 确保最终大小正确  
    }

    // 缓动函数：EaseInOutQuad  
    private float EaseInOutQuad(float t)
    {
        t = t * 2f;
        if (t < 1)
            return 0.5f * t * t;
        return -0.5f * ((--t) * (t - 2) - 1);
    }

    // 公开方法：启动放大协程  
    public void ScaleImageUp()
    {
        StartCoroutine(ScaleImageUpCoroutine());
    }

    // 公开方法：启动恢复大小协程  
    public void RestoreOriginalSize()
    {
        StartCoroutine(RestoreOriginalSizeCoroutine());
    }
}