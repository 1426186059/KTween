using UnityEngine;

public class ST_Path : MonoBehaviour
{
    public Transform[] pathPoints;
    public float moveDuration = 1f;
    public bool loop = true;

    private void Start()
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            pathPoints = GetComponentsInChildren<Transform>();
            if (pathPoints.Length < 3) return;
            // 跳过第一个（自身）
            var temp = new System.Collections.Generic.List<Transform>();
            for (int i = 1; i < pathPoints.Length; i++)
                temp.Add(pathPoints[i]);
            pathPoints = temp.ToArray();
        }

        transform.position = pathPoints[0].position;

        SimpleTween.TweenItem last = null;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            int idx = i;
            Vector3 from = pathPoints[idx].position;
            Vector3 to = pathPoints[idx + 1].position;

            var seg = SimpleTween.AddTween(gameObject, moveDuration, t =>
            {
                transform.position = SimpleTweenFunc.easeLinear(from, to, t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t);
            });

            if (last != null) last.AppendTween(seg);
            last = seg;
        }

        if (loop && pathPoints.Length > 1)
        {
            int lastIdx = pathPoints.Length - 1;
            var back = SimpleTween.AddTween(gameObject, moveDuration, t =>
            {
                transform.position = SimpleTweenFunc.easeLinear(pathPoints[lastIdx].position, pathPoints[0].position, t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t);
            });
            if (last != null) last.AppendTween(back);
        }
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
    }
}
