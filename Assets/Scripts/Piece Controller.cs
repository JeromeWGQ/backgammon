using UnityEngine;

public class PieceController : MonoBehaviour
{
    // 目标位置  
    private Vector3 targetPosition;
    // 移动持续时间（秒）  
    private float duration;
    // 当前是否正在移动  
    private bool isMoving = false;

    // 调用此函数以开始移动到目标位置  
    public void MoveToTarget(Vector3 position, bool withCurve, float speed = 1.0f, bool playSound = true)
    {
        duration = speed;
        if (!isMoving)
        {
            StartCoroutine(MoveWithEaseCoroutine(position, duration, withCurve, playSound));
        }
    }

    // 协程实现移动和缓动效果  
    private System.Collections.IEnumerator MoveWithEaseCoroutine(Vector3 position, float time, bool withCurve, bool playSound)
    {
        isMoving = true;
        targetPosition = position;
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;
            float easeInOutQuad = t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

            Vector3 yV = new Vector3(0, 0, 0);

            if (withCurve)
            {
                // 实现抛物线效果
                float yInc = (1 - (t - 0.5f) * (t - 0.5f) * 4) * 0.6f;
                yV = new Vector3(0, yInc, 0);
            }

            transform.position = Vector3.Lerp(startPosition, targetPosition, easeInOutQuad) + yV;

            yield return null;
        }

        if(playSound) sc.PlayDropSound();

        transform.position = targetPosition; // 确保最终位置精确到达  
        isMoving = false;
    }

    public void pickup()
    {
        MoveToTarget(targetPosition + new Vector3(0, 0.5f, 0),false, 0.1f,false);
        ChangeColor();
        sc.PlaySelectSound();
    }

    public void unPickup()
    {
        MoveToTarget(targetPosition + new Vector3(0, -0.5f, 0),false, 0.1f,false);
        ChangeColor();
        sc.PlayCancelSound();
    }

    private SoundController sc;

    private void Start()
    {
        targetPosition = transform.position;
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
        sc = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    void Update()
    {
        if (transform.position.x > 11 && !hasRotated)
        {
            transform.Rotate(-15, 0, 0);
            hasRotated = true;
        }
    }

    // 标志位，用于确保旋转只发生一次  
    private bool hasRotated = false;

    private new Renderer renderer; // 用于非UI物体   
    private Color originalColor;
    private bool isYellow = false;

    public void ChangeColor()
    {
        Material material = renderer.material;
        Color currentColor = material.color;
        material.color = isYellow ? originalColor : GetYellowColor(currentColor);
        StartCoroutine(LerpColor(material, isYellow ? originalColor : GetYellowColor(currentColor), 0.3f));
        isYellow = !isYellow;
    }

    public void ChangeBackColor()
    {
        if (isYellow) ChangeColor();
    }

    private Color GetYellowColor(Color original)
    {
        // 调整颜色分量以更接近蓝色  
        float blueIntensityIncrease = 0.5f; // 增加蓝色的强度
        float redGreenDecrease = 0.4f;    // 减少红色和绿色的强度
        return new Color(
            Mathf.Clamp01(originalColor.r - redGreenDecrease), // 保持红色在0到1之间  
            Mathf.Clamp01(originalColor.g - redGreenDecrease), // 保持绿色在0到1之间  
            Mathf.Clamp01(originalColor.b + blueIntensityIncrease) // 保持蓝色在0到1之间  
        );
    }

    private System.Collections.IEnumerator LerpColor(Material material, Color targetColor, float duration)
    {
        float elapsed = 0f;
        Color startColor = material.color;

        while (elapsed < duration)
        {
            material.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        material.color = targetColor;
    }
}
