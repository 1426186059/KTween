using UnityEngine;

/// <summary>
/// LeanTween Following 风格示例
/// 用 SimpleTween 实现跟随动画（damp/spring/bounce 风格）
/// 一个引导物体上下移动，多个跟随物体以不同方式跟随
/// </summary>
public class LT_Style_Following : MonoBehaviour
{
    private void Start()
    {
        // 创建引导箭头
        var arrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        arrow.transform.position = new Vector3(0, 0, 0);
        arrow.name = "FollowArrow";
        arrow.GetComponent<Renderer>().material.color = Color.red;
        arrow.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
        var arrowLabel = arrow.AddComponent<ExampleLabel>();
        arrowLabel.text = "引导";
        arrowLabel.color = Color.red;

        // 引导箭头上下摆动
        Vector3 arrowBase = arrow.transform.position;
        SimpleTween.AddTween(arrow, 1.5f, (t) =>
        {
            arrow.transform.position = arrowBase + Vector3.up * Mathf.Sin(t * Mathf.PI * 2f) * 3f;
        }).SetLoop(-1);

        // 创建 5 个跟随者，不同跟随方式
        string[] labels = { "Damp", "Spring", "Bounce", "Spring+", "Linear" };
        Color[] colors = { Color.cyan, Color.green, Color.yellow, Color.magenta, Color.gray };

        for (int i = 0; i < 5; i++)
        {
            int idx = i;
            var follower = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            follower.transform.position = new Vector3(-3f - i * 2f, 0, 0);
            follower.name = $"Follower_{labels[i]}";
            follower.transform.localScale = Vector3.one * 0.6f;
            follower.GetComponent<Renderer>().material.color = colors[i];

            var label = follower.AddComponent<ExampleLabel>();
            label.text = labels[i];
            label.color = colors[i];

            // 每种跟随方式用不同的平滑参数
            float smoothTime = 0.3f + i * 0.15f;
            float maxSpeed = 5f + i * 3f;

            // 用 Update + Mathf.SmoothDamp 模拟跟随效果
            StartCoroutine(FollowTarget(follower, arrow.transform, smoothTime, maxSpeed, i));
        }
    }

    private System.Collections.IEnumerator FollowTarget(GameObject follower, Transform target,
        float smoothTime, float maxSpeed, int mode)
    {
        Vector3 velocity = Vector3.zero;
        float velY = 0f;

        while (follower != null)
        {
            Vector3 targetPos = target.position;
            Vector3 currentPos = follower.transform.position;

            switch (mode)
            {
                case 0: // Damp
                    currentPos.y = Mathf.SmoothDamp(currentPos.y, targetPos.y, ref velY, smoothTime, maxSpeed);
                    break;
                case 1: // Spring (oscillate around target)
                    currentPos.y += (targetPos.y - currentPos.y) * 0.08f;
                    break;
                case 2: // Bounce out
                    float diff = targetPos.y - currentPos.y;
                    currentPos.y += diff * 0.12f;
                    if (Mathf.Abs(diff) > 0.1f)
                        currentPos.y += Mathf.Sign(diff) * 0.02f; // overshoot slightly
                    break;
                case 3: // Spring with damping
                    float springForce = (targetPos.y - currentPos.y) * 0.06f;
                    velocity.y += springForce;
                    velocity.y *= 0.85f; // damping
                    currentPos.y += velocity.y;
                    break;
                case 4: // Linear
                    currentPos.y = Mathf.MoveTowards(currentPos.y, targetPos.y, maxSpeed * Time.deltaTime);
                    break;
            }

            follower.transform.position = new Vector3(currentPos.x, currentPos.y, currentPos.z);
            yield return null;
        }
    }
}
