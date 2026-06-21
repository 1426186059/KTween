using UnityEngine;

/// <summary>
/// 示例脚本 06 - 循环模式对比
/// 演示：
///   - SetLoop: 普通循环（从头重复）
///   - SetLoopPingPong: 往返循环
///   - 有限次循环 vs 无限循环
/// </summary>
public class Example_06_LoopModes : MonoBehaviour
{
    public enum LoopDemoType
    {
        NormalLoop,
        NormalLoopFinite,
        PingPongInfinite,
        PingPongFinite
    }

    public LoopDemoType loopType = LoopDemoType.PingPongInfinite;
    public float duration = 0.6f;
    public float moveDistance = 3f;
    public int finiteCount = 3;
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
            float x = KTweenFunc.linear(0f, moveDistance, t);
            transform.position = m_StartPos + new Vector3(x, 0f, 0f);
        });

        switch (loopType)
        {
            case LoopDemoType.NormalLoop:
                tween.SetLoop(-1); // 无限循环
                Debug.Log($"[{name}] 普通无限循环开始");
                break;

            case LoopDemoType.NormalLoopFinite:
                tween.SetLoop(finiteCount);
                Debug.Log($"[{name}] 普通有限循环开始 ({finiteCount} 次)");
                break;

            case LoopDemoType.PingPongInfinite:
                tween.SetLoopPingPong(-1); // 无限 PingPong
                Debug.Log($"[{name}] PingPong 无限循环开始");
                break;

            case LoopDemoType.PingPongFinite:
                tween.SetLoopPingPong(finiteCount);
                Debug.Log($"[{name}] PingPong 有限循环开始 ({finiteCount} 次)");
                break;
        }
    }
}