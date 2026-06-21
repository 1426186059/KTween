using UnityEngine;

/// <summary>
/// 示例脚本 03 - 缩放脉冲 Tween
/// 演示：使用循环让物体做呼吸式缩放动画
/// </summary>
public class Example_03_ScalePulse : MonoBehaviour
{
    public float duration = 0.6f;
    public float minScale = 0.8f;
    public float maxScale = 1.3f;
    public bool autoPlay = true;

    private Vector3 m_OriginalScale;

    private void Start()
    {
        m_OriginalScale = transform.localScale;
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        transform.localScale = m_OriginalScale * minScale;

        KTween.AddTween(gameObject, duration, (t) =>
        {
            float s = KTweenFunc.linear(minScale, maxScale, t);
            transform.localScale = m_OriginalScale * s;
        }).SetLoopPingPong(-1); // 无限 PingPong 循环
    }
}