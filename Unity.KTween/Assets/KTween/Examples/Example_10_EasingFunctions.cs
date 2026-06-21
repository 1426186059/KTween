using UnityEngine;

/// <summary>
/// 示例脚本 10 - 自定义缓动函数
/// 演示：在 KTweenFunc 提供的 Linear 基础上，自定义缓动曲线
/// 并展示多种缓动效果对比
/// </summary>
public class Example_10_EasingFunctions : MonoBehaviour
{
    public enum EasingType
    {
        Linear,
        QuadIn,
        QuadOut,
        QuadInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        SineIn,
        SineOut,
        SineInOut,
        BounceOut,
        ElasticOut,
    }

    public EasingType easing = EasingType.QuadOut;
    public float duration = 1.5f;
    public float moveDistance = 4f;
    public bool useLoop = true;
    public bool autoPlay = true;

    private Vector3 m_StartPos;

    private void Start()
    {
        m_StartPos = transform.position;
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        transform.position = m_StartPos;

        var tween = KTween.AddTween(gameObject, duration, (t) =>
        {
            float eased = ApplyEasing(t);
            float x = KTweenFunc.easeLinear(0f, moveDistance, eased);
            transform.position = m_StartPos + new Vector3(x, 0f, 0f);
        });

        if (useLoop) tween.SetLoopPingPong(-1);
    }

    /// <summary>
    /// 根据选择的缓动类型应用缓动函数
    /// </summary>
    private float ApplyEasing(float t)
    {
        t = Mathf.Clamp01(t);

        return easing switch
        {
            EasingType.Linear    => t,
            EasingType.QuadIn    => t * t,
            EasingType.QuadOut   => t * (2f - t),
            EasingType.QuadInOut => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t,
            EasingType.CubicIn   => t * t * t,
            EasingType.CubicOut  => (t - 1f) * (t - 1f) * (t - 1f) + 1f,
            EasingType.CubicInOut => t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f,
            EasingType.SineIn    => 1f - Mathf.Cos(t * Mathf.PI * 0.5f),
            EasingType.SineOut   => Mathf.Sin(t * Mathf.PI * 0.5f),
            EasingType.SineInOut => 0.5f * (1f - Mathf.Cos(t * Mathf.PI)),
            EasingType.BounceOut => BounceOut(t),
            EasingType.ElasticOut => ElasticOut(t),
            _ => t,
        };
    }

    private static float BounceOut(float t)
    {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (t < 1f / d1)       return n1 * t * t;
        else if (t < 2f / d1)  { t -= 1.5f / d1; return n1 * t * t + 0.75f; }
        else if (t < 2.5f / d1){ t -= 2.25f / d1; return n1 * t * t + 0.9375f; }
        else                   { t -= 2.625f / d1; return n1 * t * t + 0.984375f; }
    }

    private static float ElasticOut(float t)
    {
        const float c4 = 2.0943951f; // 2π/3
        if (t <= 0) return 0;
        if (t >= 1) return 1;
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUI.Label(new Rect(10, Screen.height - 30, 200, 20), $"缓动: {easing}");
    }
}