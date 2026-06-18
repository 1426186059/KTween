using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 完整的 SimpleTween API 使用示例（单一文件浓缩版）
/// 涵盖所有核心功能，适合快速查阅
/// </summary>
public class Example_AllInOne : MonoBehaviour
{
    // ============================================================
    // 1. 基础移动 - AddTween + easeLinear
    // ============================================================
    [ContextMenu("1 - 基础移动")]
    public void Demo_BasicMove()
    {
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * 5f;

        SimpleTween.AddTween(gameObject, 2.0f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(from, to, t);
        }).GetHandle();
    }

    // ============================================================
    // 2. 基础旋转
    // ============================================================
    [ContextMenu("2 - 旋转")]
    public void Demo_Rotation()
    {
        Vector3 from = transform.eulerAngles;
        Vector3 to = from + new Vector3(0f, 0f, 360f);

        SimpleTween.AddTween(gameObject, 2.0f, (t) =>
        {
            transform.eulerAngles = SimpleTweenFunc.easeLinear(from, to, t);
        });
    }

    // ============================================================
    // 3. 缩放动画
    // ============================================================
    [ContextMenu("3 - 缩放")]
    public void Demo_Scale()
    {
        Vector3 from = transform.localScale;
        Vector3 to = from * 2f;

        SimpleTween.AddTween(gameObject, 1.0f, (t) =>
        {
            transform.localScale = SimpleTweenFunc.easeLinear(from, to, t);
        });
    }

    // ============================================================
    // 4. 延迟启动 - SetDelay
    // ============================================================
    [ContextMenu("4 - 延迟启动")]
    public void Demo_Delay()
    {
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * 3f;

        SimpleTween.AddTween(gameObject, 1.0f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(from, to, t);
        }).SetDelay(2.0f);
    }

    // ============================================================
    // 5. 无限循环 - SetLoop(-1)
    // ============================================================
    [ContextMenu("5 - 无限循环")]
    public void Demo_LoopInfinite()
    {
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * 3f;

        SimpleTween.AddTween(gameObject, 1.0f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(from, to, t);
        }).SetLoop(-1);
    }

    // ============================================================
    // 6. 有限循环 - SetLoop(count)
    // ============================================================
    [ContextMenu("6 - 有限循环")]
    public void Demo_LoopFinite()
    {
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * 3f;

        SimpleTween.AddTween(gameObject, 0.5f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(from, to, t);
        }).SetLoop(5);
    }

    // ============================================================
    // 7. PingPong 无限往返
    // ============================================================
    [ContextMenu("7 - PingPong 无限")]
    public void Demo_PingPongInfinite()
    {
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * 3f;

        SimpleTween.AddTween(gameObject, 1.0f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(from, to, t);
        }).SetLoopPingPong(-1);
    }

    // ============================================================
    // 8. PingPong 有限往返
    // ============================================================
    [ContextMenu("8 - PingPong 有限")]
    public void Demo_PingPongFinite()
    {
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * 3f;

        SimpleTween.AddTween(gameObject, 0.5f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(from, to, t);
        }).SetLoopPingPong(3);
    }

    // ============================================================
    // 9. 链式序列 - AppendTween
    // ============================================================
    [ContextMenu("9 - 链式序列")]
    public void Demo_ChainSequence()
    {
        Vector3 p0 = transform.position;
        Vector3 p1 = p0 + Vector3.right * 2f;
        Vector3 p2 = p1 + Vector3.forward * 2f;
        Vector3 p3 = p2 + Vector3.left * 2f;

        var t1 = SimpleTween.AddTween(gameObject, 1.0f, (t) =>
            transform.position = SimpleTweenFunc.easeLinear(p0, p1, t));

        var t2 = SimpleTween.AddTween(gameObject, 1.0f, (t) =>
            transform.position = SimpleTweenFunc.easeLinear(p1, p2, t));

        var t3 = SimpleTween.AddTween(gameObject, 1.0f, (t) =>
            transform.position = SimpleTweenFunc.easeLinear(p2, p3, t));

        t1.AppendTween(t2);
        t2.AppendTween(t3);
    }

    // ============================================================
    // 10. 延迟调用 - delayedCall
    // ============================================================
    [ContextMenu("10 - DelayedCall")]
    public void Demo_DelayedCall()
    {
        Debug.Log($"延迟调用开始: {Time.time:F2}");

        SimpleTween.delayedCall(gameObject, 1.0f, () =>
        {
            Debug.Log($"1秒后: {Time.time:F2}");
        });

        SimpleTween.delayedCall(gameObject, 2.0f, () =>
        {
            Debug.Log($"2秒后: {Time.time:F2}");
        });
    }

    // ============================================================
    // 11. 取消 Tween - cancel()
    // ============================================================
    [ContextMenu("11 - 取消 Tween")]
    public void Demo_Cancel()
    {
        Vector3 from = transform.position;
        var item = SimpleTween.AddTween(gameObject, 10.0f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(from, from + Vector3.right * 10f, t);
        });

        // 1 秒后取消
        SimpleTween.delayedCall(gameObject, 1.0f, () =>
        {
            item.cancel();
            Debug.Log("Tween 已取消");
        });
    }

    // ============================================================
    // 12. Handle 安全句柄使用
    // ============================================================
    [ContextMenu("12 - Handle 句柄")]
    public void Demo_Handle()
    {
        var item = SimpleTween.AddTween(gameObject, 3.0f, null, null);
        var handle = item.GetHandle();

        Debug.Log($"Handle IsValid: {handle.IsValid()}"); // True

        SimpleTween.delayedCall(gameObject, 1.0f, () =>
        {
            handle.Cancel();
            Debug.Log($"Handle IsValid After Cancel: {handle.IsValid()}"); // False
        });
    }

    // ============================================================
    // 13. 对象绑定自动停止
    // ============================================================
    [ContextMenu("13 - 对象绑定")]
    public void Demo_BindGameObject()
    {
        var tempGo = new GameObject("TempTweenObject");
        tempGo.transform.position = new Vector3(5, 0, 0);

        SimpleTween.AddTween(tempGo, 10.0f, (t) =>
        {
            Debug.Log($"Tween 运行中: {t}");
        }, () =>
        {
            Debug.Log("这个不会执行，因为对象会被销毁");
        });

        // 销毁绑定的对象 → Tween 自动停止
        Destroy(tempGo, 2.0f);
    }

    // ============================================================
    // 14. 不绑定 GameObject 的 Tween
    // ============================================================
    [ContextMenu("14 - 无 GameObject 绑定")]
    public void Demo_WithoutGameObject()
    {
        int counter = 0;
        SimpleTween.AddTween(2.0f, (t) =>
        {
            counter++;
        }, () =>
        {
            Debug.Log($"Tween 结束, counter = {counter}");
        });
    }

    // ============================================================
    // 15. 颜色渐变
    // ============================================================
    [ContextMenu("15 - 颜色渐变")]
    public void Demo_ColorLerp()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("需要 Renderer 组件");
            return;
        }

        Color from = Color.red;
        Color to = Color.blue;

        SimpleTween.AddTween(gameObject, 2.0f, (t) =>
        {
            renderer.material.color = Color.Lerp(from, to, t);
        }).SetLoopPingPong(-1);
    }

    // ============================================================
    // 16. CanvasGroup 透明度 (UI)
    // ============================================================
    [ContextMenu("16 - 透明度 (CanvasGroup)")]
    public void Demo_CanvasGroupAlpha()
    {
        var cg = GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = gameObject.AddComponent<CanvasGroup>();
        }

        SimpleTween.AddTween(gameObject, 1.0f, (t) =>
        {
            cg.alpha = t;
        }).SetLoopPingPong(-1);
    }

    // ============================================================
    // 17. 组合动画：同时移动 + 旋转 + 缩放
    // ============================================================
    [ContextMenu("17 - 组合动画")]
    public void Demo_CombinedTransform()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.right * 3f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.5f;

        SimpleTween.AddTween(gameObject, 2.0f, (t) =>
        {
            transform.position = SimpleTweenFunc.easeLinear(startPos, endPos, t);
            transform.localScale = SimpleTweenFunc.easeLinear(startScale, endScale, t);
            transform.eulerAngles = SimpleTweenFunc.easeLinear(Vector3.zero, new Vector3(0f, 0f, 360f * 2f), t);
        }).SetLoopPingPong(-1);
    }

    // ============================================================
    // 18. 自定义缓动函数
    // ============================================================
    [ContextMenu("18 - 自定义缓动")]
    public void Demo_CustomEasing()
    {
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * 5f;

        SimpleTween.AddTween(gameObject, 1.5f, (t) =>
        {
            // 弹性缓动曲线
            float eased = EaseOutBack(t);
            transform.position = SimpleTweenFunc.easeLinear(from, to, eased);
        });
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    // ============================================================
    // 19. 弹跳效果（多 Tween 组合）
    // ============================================================
    [ContextMenu("19 - 弹跳效果")]
    public void Demo_Bouncing()
    {
        Vector3 basePos = transform.position;
        SimpleTween.TweenItem last = null;

        for (int i = 0; i < 5; i++)
        {
            float height = 2f * Mathf.Pow(0.6f, i);
            Vector3 peak = basePos + Vector3.up * height;

            var up = SimpleTween.AddTween(gameObject, 0.3f, (t) =>
            {
                transform.position = Vector3.Lerp(basePos, peak, t);
            });

            var down = SimpleTween.AddTween(gameObject, 0.4f, (t) =>
            {
                transform.position = Vector3.Lerp(peak, basePos, t);
            });

            if (last != null) last.AppendTween(up);
            up.AppendTween(down);
            last = down;
        }
    }

    // ============================================================
    // 20. 清除所有 Tween（通过句柄批量取消）
    // ============================================================
    private readonly List<SimpleTween.TweenItemHandle> m_AllHandles = new List<SimpleTween.TweenItemHandle>();

    [ContextMenu("20 - 批量取消所有 Tween")]
    public void Demo_CancelAllTweens()
    {
        foreach (var h in m_AllHandles)
        {
            if (h.IsValid())
                h.Cancel();
        }
        m_AllHandles.Clear();
        Debug.Log("所有 Tween 已取消");
    }

    /// <summary>
    /// 包装方法：自动记录句柄以便批量取消
    /// </summary>
    public SimpleTween.TweenItemHandle TrackedTween(SimpleTween.TweenItem item)
    {
        var handle = item.GetHandle();
        m_AllHandles.Add(handle);
        return handle;
    }

    private void OnDestroy()
    {
        Demo_CancelAllTweens();
    }
}
