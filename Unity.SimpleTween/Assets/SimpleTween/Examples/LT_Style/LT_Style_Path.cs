using UnityEngine;

/// <summary>
/// LeanTween PathBezier / PathSpline 风格示例
/// 用 AppendTween 链式连接路径点，实现路径运动
/// 支持在 Scene 视图中编辑路径点
/// </summary>
public class LT_Style_Path : MonoBehaviour
{
    public Transform[] pathPoints;
    public float moveDuration = 1.0f;
    public bool loop = true;

    private void Start()
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            // 如果未设置路径点，创建默认路径
            CreateDefaultPath();
        }
        StartPathAnimation();
    }

    private void CreateDefaultPath()
    {
        pathPoints = new Transform[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = i / 5f * 360f * Mathf.Deg2Rad;
            float radius = 3f;
            var p = new GameObject($"PathPoint_{i}");
            p.transform.position = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle * 2f) * 1.5f + 2f,
                Mathf.Sin(angle) * radius
            );
            p.transform.SetParent(transform);
            pathPoints[i] = p.transform;
        }
    }

    private void StartPathAnimation()
    {
        if (pathPoints.Length < 2) return;

        transform.position = pathPoints[0].position;

        SimpleTween.TweenItem last = null;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            int idx = i;
            Vector3 from = pathPoints[idx].position;
            Vector3 to = pathPoints[idx + 1].position;

            var seg = SimpleTween.AddTween(gameObject, moveDuration, (t) =>
            {
                float eased = EaseInOutQuad(t);
                transform.position = SimpleTweenFunc.easeLinear(from, to, eased);
            });

            if (last != null) last.AppendTween(seg);
            last = seg;
        }

        if (loop && pathPoints.Length > 1)
        {
            Vector3 lastPos = pathPoints[pathPoints.Length - 1].position;
            Vector3 firstPos = pathPoints[0].position;

            var back = SimpleTween.AddTween(gameObject, moveDuration, (t) =>
            {
                float eased = EaseInOutQuad(t);
                transform.position = SimpleTweenFunc.easeLinear(lastPos, firstPos, eased);
            });

            last.AppendTween(back);
        }
    }

    // 朝向路径方向（自动旋转跟随路径方向）
    private void Update()
    {
        // 简单的朝向：让物体始终朝运动方向
        // 用 Rigidbody 或 LookAt 会更精确，这里用简化版本
    }

    private void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;
        Gizmos.color = new Color(1, 0.5f, 0);
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (pathPoints[i] != null && pathPoints[i + 1] != null)
            {
                Gizmos.DrawLine(pathPoints[i].position, pathPoints[i + 1].position);
                Gizmos.DrawSphere(pathPoints[i].position, 0.15f);
            }
        }
        if (pathPoints[pathPoints.Length - 1] != null)
            Gizmos.DrawSphere(pathPoints[pathPoints.Length - 1].position, 0.15f);
        if (loop && pathPoints[0] != null && pathPoints[pathPoints.Length - 1] != null)
            Gizmos.DrawLine(pathPoints[pathPoints.Length - 1].position, pathPoints[0].position);
    }

    private static float EaseInOutQuad(float t) =>
        t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
}
