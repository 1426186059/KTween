using UnityEngine;

/// <summary>
/// 示例脚本 11 - UI CanvasGroup 淡入淡出
/// 演示：用 Tween 控制 CanvasGroup.alpha 实现 UI 渐隐渐现
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Example_11_UI_Fade : MonoBehaviour
{
    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 1.0f;
    public float holdDuration = 1.0f;
    public bool autoPlay = true;

    private CanvasGroup m_CanvasGroup;

    private void Start()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        // 创建完整的淡入 → 保持 → 淡出序列
        var fadeIn = KTween.AddTween(gameObject, fadeInDuration, (t) =>
        {
            m_CanvasGroup.alpha = t;
        });

        var hold = KTween.delayedCall(gameObject, holdDuration);

        var fadeOut = KTween.AddTween(gameObject, fadeOutDuration, (t) =>
        {
            m_CanvasGroup.alpha = 1f - t;
        });

        // 串联成序列
        fadeIn.AppendTween(hold);
        hold.AppendTween(fadeOut);
    }
}