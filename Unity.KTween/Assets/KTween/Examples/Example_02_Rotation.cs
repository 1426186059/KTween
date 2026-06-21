using UnityEngine;

/// <summary>
/// 示例脚本 02 - 旋转 Tween
/// 演示：用 Tween 旋转一个物体 360 度
/// </summary>
public class Example_02_Rotation : MonoBehaviour
{
    public float duration = 2.0f;
    public Vector3 targetRotation = new Vector3(0f, 0f, 360f);
    public bool autoPlay = true;

    private void Start()
    {
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        Vector3 from = transform.eulerAngles;
        Vector3 to = from + targetRotation;

        KTween.AddTween(gameObject, duration, (t) =>
        {
            transform.eulerAngles = KTweenFunc.easeLinear(from, to, t);
        });
    }
}