using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelFlash : MonoBehaviour
{

    private float frequency = 2f; // 调整纹理振幅的频率
    private float amplitude = 0.1f; // 调整纹理振幅的幅度
    private float speed = 1.5f; // 调整正弦波的速度

    private Renderer _renderer;
    private float _time = 0.0f;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        _time += Time.deltaTime * speed;
        float wave = amplitude * Mathf.Sin(frequency * _time);
        Color color = _renderer.material.color;
        color.r = color.g = color.b = 0.6f - wave; // 使用正弦波的值反转颜色，接近白色
        _renderer.material.color = color;
    }

    //private float duration = 1.0f; // 闪白持续时间
    //private float interval = 0.05f; // 闪亮和灭的间隔时间
    //private Renderer _renderer;
    //private Color _originalColor;
    //private float _timer;

    //void Start()
    //{
    //    _renderer = GetComponent<Renderer>();
    //    _originalColor = _renderer.material.color;
    //}

    //void Update()
    //{
    //    if (_timer < duration)
    //    {
    //        _renderer.material.color = Color.white;
    //        _timer += Time.deltaTime;
    //    }
    //    else if (_timer < duration + interval)
    //    {
    //        _renderer.material.color = _originalColor;
    //        _timer += Time.deltaTime;
    //    }
    //    else
    //    {
    //        _timer = 0.0f; // 重置计时器
    //    }
    //}
}
