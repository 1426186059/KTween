using UnityEngine;

/// <summary>
/// LeanTween GeneralSequencer 风格示例
/// 使用 AppendTween 实现序列动画：跳跃 → 旋转 → 落地 → 生成粒子
/// </summary>
public class LT_Style_Sequencer : MonoBehaviour
{
    private void Start()
    {
        // 第一个物体
        var avatar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        avatar.transform.position = new Vector3(-2, 0, 0);
        avatar.name = "Avatar1";
        avatar.GetComponent<Renderer>().material.color = new Color(0, 0.7f, 1);

        Sequence_JumpRotateLand(avatar);

        // 第二个物体（带缩放 alpha 效果）
        var star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        star.transform.position = new Vector3(2, 0, 0);
        star.transform.localScale = Vector3.one * 0.5f;
        star.name = "Star";
        star.GetComponent<Renderer>().material.color = Color.yellow;

        Sequence_StarPowerUp(star);
    }

    private void Sequence_JumpRotateLand(GameObject obj)
    {
        Vector3 basePos = obj.transform.position;

        // 1. 跳起
        Vector3 peakPos = basePos + Vector3.up * 6f;
        var jumpUp = SimpleTween.AddTween(obj, 1f, (t) =>
        {
            float eased = EaseOutQuad(t);
            obj.transform.position = SimpleTweenFunc.easeLinear(basePos, peakPos, eased);
        });

        // 2. 旋转 360
        Vector3 startRot = obj.transform.eulerAngles;
        Vector3 endRot = startRot + new Vector3(0, 0, 360f);
        var spin = SimpleTween.AddTween(obj, 0.6f, (t) =>
        {
            float eased = BackIn(t);
            obj.transform.eulerAngles = SimpleTweenFunc.easeLinear(startRot, endRot, eased);
        });

        // 3. 落地
        var land = SimpleTween.AddTween(obj, 1f, (t) =>
        {
            float eased = EaseInQuad(t);
            obj.transform.position = SimpleTweenFunc.easeLinear(peakPos, basePos, eased);
        });

        // 4. 落地后产生许多小方块飞散（回调效果）
        var spawnParticles = SimpleTween.delayedCall(obj, 0f, () =>
        {
            for (int i = 0; i < 20; i++)
            {
                var cloud = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cloud.transform.position = obj.transform.position + Random.insideUnitSphere * 0.5f;
                cloud.transform.localScale = Vector3.one * 0.2f;
                cloud.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                cloud.GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);

                Vector3 target = cloud.transform.position + new Vector3(
                    Random.Range(-3f, 3f), Random.Range(0, 4f), Random.Range(-5f, 5f));

                Vector3 from = cloud.transform.position;
                SimpleTween.AddTween(cloud, 3f, (t) =>
                {
                    cloud.transform.position = SimpleTweenFunc.easeLinear(from, target, EaseOutCirc(t));
                    float alpha = 1f - t;
                    cloud.GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f, alpha);
                });
            }
        });

        // 串联成序列
        jumpUp.AppendTween(spin);
        spin.AppendTween(land);
        land.AppendTween(spawnParticles);
    }

    private void Sequence_StarPowerUp(GameObject star)
    {
        Vector3 baseScale = star.transform.localScale;
        Vector3 bigScale = baseScale * 3f;

        // 放大 + 淡出再淡入（模拟 power up）
        var fadeOut = SimpleTween.AddTween(star, 0.8f, (t) =>
        {
            star.transform.localScale = SimpleTweenFunc.easeLinear(baseScale, bigScale, t);
            var c = star.GetComponent<Renderer>().material.color;
            c.a = 1f - t;
            star.GetComponent<Renderer>().material.color = c;
        });

        var fadeIn = SimpleTween.AddTween(star, 0.5f, (t) =>
        {
            star.transform.localScale = SimpleTweenFunc.easeLinear(bigScale, baseScale, t);
            var c = star.GetComponent<Renderer>().material.color;
            c.a = t;
            star.GetComponent<Renderer>().material.color = c;
        });

        fadeOut.AppendTween(fadeIn);
    }

    // Easing helpers
    private static float EaseOutQuad(float t) => t * (2f - t);
    private static float EaseInQuad(float t) => t * t;
    private static float EaseOutCirc(float t) => Mathf.Sqrt(1f - (t - 1f) * (t - 1f));
    private static float BackIn(float t)
    {
        const float c1 = 1.70158f;
        return (c1 + 1f) * t * t * t - c1 * t * t;
    }
}
