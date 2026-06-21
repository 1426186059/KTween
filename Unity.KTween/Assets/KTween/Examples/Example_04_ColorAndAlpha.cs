using UnityEngine;

/// <summary>
/// 示例脚本 04 - 颜色 + 透明度 Tween
/// 演示：用 Tween 渐变 SpriteRenderer 的颜色和透明度
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Example_04_ColorAndAlpha : MonoBehaviour
{
    public float duration = 2.0f;
    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    public bool autoPlay = true;

    private SpriteRenderer m_Renderer;

    private void Start()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        m_Renderer.color = color1;

        // 在两种颜色之间 PingPong 循环
        KTween.AddTween(gameObject, duration, (t) =>
        {
            m_Renderer.color = Color.Lerp(color1, color2, t);
        }).SetLoopPingPong(-1);
    }
}