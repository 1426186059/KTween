using UnityEngine;

/// <summary>
/// 示例脚本 12 - 弹跳效果
/// 演示：利用多个 Tween 组合实现弹跳落地效果
/// </summary>
public class Example_12_BounceEffect : MonoBehaviour
{
    public float bounceHeight = 3f;
    public float bounceDuration = 0.4f;
    public int bounceCount = 4;
    public bool autoPlay = true;

    private Vector3 m_BasePos;

    private void Start()
    {
        m_BasePos = transform.position;
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        transform.position = m_BasePos;

        KTween.TweenItem last = null;
        for (int i = 0; i < bounceCount; i++)
        {
            int bounceIndex = i;
            float height = bounceHeight * Mathf.Pow(0.6f, i); // 每次弹跳高度递减
            float dur = bounceDuration * Mathf.Pow(0.8f, i);  // 每次持续时间递减

            Vector3 peakPos = m_BasePos + Vector3.up * height;

            // 上升段
            var up = KTween.AddTween(gameObject, dur * 0.4f, (t) =>
            {
                transform.position = Vector3.Lerp(m_BasePos, peakPos, t);
            });

            // 下降段
            var down = KTween.AddTween(gameObject, dur * 0.6f, (t) =>
            {
                transform.position = Vector3.Lerp(peakPos, m_BasePos, t);
            });

            if (last != null) last.AppendTween(up);
            up.AppendTween(down);
            last = down;
        }

        // 最后一次弹跳后加一点小震动
        var settle = KTween.AddTween(gameObject, 0.2f, (t) =>
        {
            float offset = Mathf.Sin(t * Mathf.PI * 4f) * 0.1f * (1f - t);
            transform.position = m_BasePos + Vector3.up * offset;
        });
        last?.AppendTween(settle);
    }
}