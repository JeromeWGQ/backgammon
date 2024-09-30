using UnityEngine;

public class PieceController : MonoBehaviour
{
    // 目标位置
    private Vector3 targetPosition;
    // 移动速度
    private float moveSpeed = 10.0f;

    // 设置目标位置的公共方法
    public void SetTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
    }

    public void pickup()
    {
        targetPosition += new Vector3(0, 0.5f, 0);
    }

    public void unPickup()
    {
        targetPosition -= new Vector3(0, 0.5f, 0);
    }

    private void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // 检查当前位置与目标位置是否一致  
        if (Vector3.Distance(transform.position, targetPosition) > 0.01f) // 使用一个小的阈值以避免浮点精度问题  
        {
            // 计算移动方向  
            Vector3 direction = (targetPosition - transform.position).normalized;

            // 计算移动距离（基于速度和时间）  
            float step = moveSpeed * Time.deltaTime;

            // 移动物体  
            transform.position += direction * step;
        }
        if (transform.position.x > 11 && !hasRotated)
        {
            transform.Rotate(-15, 0, 0);
            hasRotated = true;
        }
    }

    // 标志位，用于确保旋转只发生一次  
    private bool hasRotated = false;
}
