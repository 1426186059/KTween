using UnityEngine;

/// <summary>
/// 示例脚本 09 - 多物体同步 Tween
/// 演示：对多个物体同时应用不同类型的 Tween 动画
/// </summary>
public class Example_09_MultiObject : MonoBehaviour
{
    public GameObject[] targets;
    public float duration = 2.0f;
    public bool autoPlay = true;

    private void Start()
    {
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (targets == null || targets.Length == 0) return;

        for (int i = 0; i < targets.Length; i++)
        {
            var go = targets[i];
            if (go == null) continue;

            float phase = (float)i / targets.Length; // 错开相位
            Vector3 from = go.transform.position;
            Vector3 to = from + new Vector3(Mathf.Sin(i * 1.5f) * 3f, 0f, 0f);

            SimpleTween.AddTween(go, duration, (t) =>
            {
                // 将时间错开相位
                float p = (t + phase) % 1f;
                go.transform.position = SimpleTweenFunc.easeLinear(from, to, p);
            }).SetLoop(-1); // 无限循环
        }
    }
}
