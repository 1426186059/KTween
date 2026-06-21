using UnityEngine;

/// <summary>
/// 示例脚本 13 - 路径运动（多点路径）
/// 演示：用 AppendTween 链式串联多个位置更新，实现沿路径运动
/// </summary>
public class Example_13_PathMovement : MonoBehaviour
{
    public Transform[] pathPoints;
    public float moveDurationPerSegment = 1.0f;
    public bool loopPath = true;
    public bool autoPlay = true;

    private void Start()
    {
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;

        transform.position = pathPoints[0].position;

        SimpleTween.TweenItem last = null;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Vector3 from = pathPoints[i].position;
            Vector3 to = pathPoints[i + 1].position;

            var seg = SimpleTween.AddTween(gameObject, moveDurationPerSegment, (t) =>
            {
                transform.position = SimpleTweenFunc.easeLinear(from, to, t);
            });

            if (last != null) last.AppendTween(seg);
            last = seg;
        }

        // 如果要回到起点循环
        if (loopPath && pathPoints.Length > 1)
        {
            Vector3 lastPos = pathPoints[pathPoints.Length - 1].position;
            Vector3 firstPos = pathPoints[0].position;

            var back = SimpleTween.AddTween(gameObject, moveDurationPerSegment, (t) =>
            {
                transform.position = SimpleTweenFunc.easeLinear(lastPos, firstPos, t);
            });

            last.AppendTween(back);
            last = back;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                Gizmos.DrawSphere(pathPoints[i].position, 0.2f);
            }
        }
        if (pathPoints[pathPoints.Length - 1] != null)
            Gizmos.DrawSphere(pathPoints[pathPoints.Length - 1].position, 0.2f);
    }
}
