using UnityEngine;

/// <summary>
/// 示例脚本 08 - delayedCall 使用
/// 演示：SimpleTween.delayedCall() 用于延时执行逻辑
/// </summary>
public class Example_08_DelayedCall : MonoBehaviour
{
    public float delay1 = 1.0f;
    public float delay2 = 2.0f;
    public float delay3 = 3.0f;
    public bool autoPlay = true;

    private void Start()
    {
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        Debug.Log($"[{name}] 开始: {Time.time:F2}s");

        // 1 秒后
        SimpleTween.delayedCall(gameObject, delay1, () =>
        {
            Debug.Log($"[{name}] 延迟 {delay1}s 到达: {Time.time:F2}s");
        });

        // 2 秒后
        SimpleTween.delayedCall(gameObject, delay2, () =>
        {
            Debug.Log($"[{name}] 延迟 {delay2}s 到达: {Time.time:F2}s");
        });

        // 3 秒后
        SimpleTween.delayedCall(gameObject, delay3, () =>
        {
            Debug.Log($"[{name}] 延迟 {delay3}s 到达: {Time.time:F2}s");
        });

        // 也可以不绑定 GameObject
        SimpleTween.delayedCall(0.5f, () =>
        {
            Debug.Log($"[{name}] 不带 GameObject 的延迟调用: {Time.time:F2}s");
        });
    }
}
