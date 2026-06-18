using UnityEngine;

/// <summary>
/// LeanTween GeneralCameraShake 风格示例
/// 用旋转 Tween 模拟相机震动效果
/// </summary>
public class LT_Style_CameraShake : MonoBehaviour
{
    public float shakeAmount = 2f;
    public float shakeDuration = 0.5f;

    private void Start()
    {
        // 创建一个跳跳球来触发震动
        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = new Vector3(0, 3, 0);
        ball.GetComponent<Renderer>().material.color = Color.red;
        ball.AddComponent<Rigidbody>().AddForce(Vector3.down * 100f);

        var labelGo = new GameObject("ShakeLabel");
        labelGo.transform.position = new Vector3(0, 4, 0);
        var label = labelGo.AddComponent<ExampleLabel>();
        label.text = "点击场景触发震动";
        label.color = Color.white;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShakeCamera();
        }
    }

    [ContextMenu("Shake!")]
    public void ShakeCamera()
    {
        if (Camera.main == null) return;

        Vector3 startRot = Camera.main.transform.eulerAngles;

        // 左右快速震动
        SimpleTween.AddTween(Camera.main.gameObject, shakeDuration, (t) =>
        {
            float decay = 1f - t;
            float offset = Mathf.Sin(t * Mathf.PI * 12f) * shakeAmount * decay;
            Camera.main.transform.eulerAngles = startRot + new Vector3(0, 0, offset);
        });

        // 配合垂直震动
        SimpleTween.AddTween(Camera.main.gameObject, shakeDuration * 0.7f, (t) =>
        {
            float decay = 1f - t;
            float offset = Mathf.Sin(t * Mathf.PI * 16f) * shakeAmount * 0.5f * decay;
            Camera.main.transform.eulerAngles = startRot + new Vector3(offset, 0, 0);
        });
    }
}
