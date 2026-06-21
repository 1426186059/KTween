using UnityEngine;

/// <summary>
/// 示例脚本 05 - Delay 与 Chain（序列）Tween
/// 演示：
///   - SetDelay 延迟启动
///   - AppendTween 链式串联多个 Tween 形成动画序列
/// </summary>
public class Example_05_DelayAndChain : MonoBehaviour
{
    public float moveDuration = 0.8f;
    public float waitDuration = 0.5f;
    public Vector3[] waypoints = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(3, 2, 0),
        new Vector3(5, 0, 0),
        new Vector3(0, 0, 0),
    };
    public bool autoPlay = true;

    private void Start()
    {
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (waypoints.Length < 2) return;

        KTween.TweenItem last = null;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Vector3 from = waypoints[i];
            Vector3 to = waypoints[i + 1];
            int idx = i; // 捕获循环变量

            var tween = KTween.AddTween(gameObject, moveDuration, (t) =>
            {
                transform.position = KTweenFunc.easeLinear(from, to, t);
            });

            if (last != null)
            {
                last.AppendTween(tween); // 串联到上一个之后
            }

            last = tween;

            // 在每个区段之间插入一个延迟（停顿效果）
            if (i < waypoints.Length - 2)
            {
                var delayTween = KTween.AddTween(gameObject, 0f, null);
                delayTween.SetDelay(waitDuration);
                last.AppendTween(delayTween);
                last = delayTween;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            Gizmos.DrawSphere(waypoints[i], 0.15f);
        }
        Gizmos.DrawSphere(waypoints[waypoints.Length - 1], 0.15f);
    }
}