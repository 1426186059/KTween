using UnityEngine;

/// <summary>
/// 示例脚本 07 - Cancel 与 Handle 使用
/// 演示：
///   - cancel() 停止 Tween
///   - TweenItemHandle 安全句柄的使用
///   - IsValid() 检查句柄状态
///   - IDisposable 释放模式
/// </summary>
public class Example_07_CancelAndHandle : MonoBehaviour
{
    public KeyCode cancelKey = KeyCode.Space;
    public float duration = 3.0f;
    public bool autoPlay = true;

    private KTween.TweenItemHandle m_Handle;
    private bool m_IsCancelled = false;

    private void Start()
    {
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        m_IsCancelled = false;
        Vector3 from = transform.position;

        var item = KTween.AddTween(gameObject, duration, (t) =>
        {
            if (!m_IsCancelled)
            {
                transform.position = KTweenFunc.linear(from, from + Vector3.right * 5f, t);
            }
        });

        m_Handle = item.GetHandle();
        Debug.Log($"[{name}] Tween 开始 (3s), 按 {cancelKey} 取消, IsValid={m_Handle.IsValid()}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(cancelKey) && m_Handle.IsValid())
        {
            m_IsCancelled = true;
            m_Handle.Cancel();
            Debug.Log($"[{name}] Tween 已取消, IsValid={m_Handle.IsValid()}");
        }
    }

    private void OnDestroy()
    {
        if (m_Handle.IsValid())
        {
            m_Handle.Dispose();
            Debug.Log($"[{name}] Handle 已释放 (Dispose)");
        }
    }

    private void OnGUI()
    {
        if (m_Handle.IsValid())
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(10, 10, 300, 20), $"Tween 运行中... 按 {cancelKey} 取消");
        }
        else if (!m_IsCancelled)
        {
            GUI.color = Color.gray;
            GUI.Label(new Rect(10, 10, 300, 20), "Tween 已完成");
        }
        else
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 300, 20), "Tween 已取消 (Cancel)");
        }
    }
}