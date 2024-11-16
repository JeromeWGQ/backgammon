using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    private int targetFace; // 目标面向上的值，例如1-6  
    private Rigidbody rb;

    // 用于存储物体的初始位置
    private Vector3 initialPosition;

    void Start()
    {
        // 记录物体的初始位置，场景里拖拽到的位置
        initialPosition = transform.position;

        // 开局直接挪到地下
        transform.position = new Vector3(transform.position.x, -1, transform.position.z);

        rb = GetComponent<Rigidbody>();

        // 测试下
        //RotateAndDown(Random.Range(1, 7));
    }

    private float rotateForce = 3000f;

    public void RotateAndDown(int target)
    {
        transform.position = initialPosition;

        this.targetFace = target;

        // 确保目标面在1到6之间  
        if (targetFace < 1 || targetFace > 6)
        {
            Debug.LogError("targetFace 必须在1到6之间");
            return;
        }

        // 计算旋转角度  
        Vector3 targetRotation = CalculateTargetRotation(targetFace);

        // 设置初始旋转力  
        rb.angularVelocity = Vector3.zero; // 先停止所有现有的角速度  
        rb.AddTorque(Random.insideUnitSphere * rotateForce, ForceMode.Impulse); // 添加随机扭矩以开始旋转  

        // 使用协程在几秒后设置目标旋转  
        StartCoroutine(StopAndSetRotation(targetRotation, 1f)); // 3秒后停止并设置旋转  
    }

    private float disappearDuration = 0.3f; // 物体消失的时间

    public void Disappear()
    {
        rb.useGravity = false;
        StartCoroutine(ShrinkAndDisappear());
    }

    IEnumerator ShrinkAndDisappear()
    {
        // 缩小物体
        float elapsedTime = 0f;
        // 记录物体的原始大小和旋转
        Vector3 originalScale = transform.localScale;
        while (elapsedTime < disappearDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / disappearDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 瞬间移动物体到地下并恢复原始大小和旋转
        transform.position = new Vector3(transform.position.x, -1, transform.position.z);
        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;
    }

    Vector3 CalculateTargetRotation(int face)
    {
        // 创建一个目标旋转
        if (face == 1)
        {
            return new Vector3(0, 0, 0);
        }
        else if (face == 2)
        {
            return new Vector3(90, 0, 0);
        }
        else if (face == 3)
        {
            return new Vector3(0, 0, 90);
        }
        else if (face == 4)
        {
            return new Vector3(0, 0, -90);
        }
        else if (face == 5)
        {
            return new Vector3(-90, 0, 0);
        }
        else if (face == 6)
        {
            return new Vector3(0, 0, -180);
        }
        return new Vector3(0, 0, 0);
    }

    IEnumerator StopAndSetRotation(Vector3 targetRotation, float duration)
    {
        // 等待一段时间  
        yield return new WaitForSeconds(duration);

        // 停止物理旋转（将物体设置为运动学）  
        rb.isKinematic = true;

        // 禁用碰撞和刚体更新，以避免物理干扰  
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete; // 或者设置为 None，如果不需要碰撞检测  

        // 插值到目标旋转（可选，但为了平滑过渡，这里加上）  
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), t);
            yield return null;
        }

        // 直接设置最终旋转（如果不使用插值，则只需这一行）  
        // transform.rotation = Quaternion.Euler(targetEulerAngles);  

        // 恢复物理属性  
        rb.useGravity = true;
        //rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 恢复为连续碰撞检测，如果需要的话  
        rb.isKinematic = false;

        // 清除角速度  
        rb.angularVelocity = Vector3.zero;

        // test
        //Disappear();
    }

    //Rigidbody rb; // 如果你需要物理组件，可以保留这个变量
    private bool hasStarted = false; // 防止协程被重复启动

    public void appear(int target)
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        // 计算旋转角度  
        Vector3 targetRotation = CalculateTargetRotation(target);
        transform.rotation = Quaternion.Euler(targetRotation);
        // 如果还没有启动协程，则在一秒后启动它
        if (!hasStarted)
        {
            StartCoroutine(ScaleUpAfterDelay());
            hasStarted = true; // 标记为已启动，防止重复
        }
    }

    IEnumerator ScaleUpAfterDelay()
    {
        // 等待一秒
        yield return new WaitForSeconds(2f);

        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        // 使用Lerp在指定的持续时间内平滑过渡到目标缩放比例
        float elapsed = 0.0f;
        while (elapsed < 0.5f)
        {
            transform.localScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.1f), new Vector3(1f, 1f, 1f), elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保最终缩放比例精确为目标值
        transform.localScale = new Vector3(1f, 1f, 1f);

        hasStarted = false; // 标记为已启动，防止重复
        //rb.useGravity = true;
    }
}
