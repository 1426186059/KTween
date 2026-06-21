using UnityEngine;

/// <summary>
/// 示例脚本 01 - 基础移动 Tween
/// 演示：使用 KTweenFunc.linear 在 X 轴上往复移动
/// </summary>
public class Example_01_BasicMove : MonoBehaviour
{
    public float duration = 1.5f;
    public float distance = 5f;
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

        // 正向移动
        KTween.AddTween(gameObject, duration, (t) =>
        {
            float x = KTweenFunc.easeLinear(0f, distance, t);
            transform.position = m_StartPos + new Vector3(x, 0f, 0f);
        });
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Vector3 from = transform.position;
            Vector3 to = from + new Vector3(distance, 0f, 0f);
            Gizmos.DrawLine(from, to);
            Gizmos.DrawSphere(to, 0.2f);
        }
    }
}