using UnityEngine;

/// <summary>
/// LeanTween GeneralBasic 风格的 SimpleTween 示例
/// 旋转、缩放、移动、延迟、链式串联
/// </summary>
public class LT_Style_GeneralBasic : MonoBehaviour
{
    public GameObject prefabAvatar;

    private void Start()
    {
        // 旋转 - 绕 Z 轴 360 度
        var rotateObj = CreatePrimitive(PrimitiveType.Cube, new Vector3(-4, 2, 0), "AvatarRotate");
        AddRotation(rotateObj, 5f);

        // 缩放 + 移动同时进行
        var scaleObj = CreatePrimitive(PrimitiveType.Cube, new Vector3(0, 2, 0), "AvatarScale");
        AddScaleAndMove(scaleObj, 5f);

        // 移动 + 延迟
        var moveObj = CreatePrimitive(PrimitiveType.Cube, new Vector3(4, 2, 0), "AvatarMove");
        AddMoveWithDelay(moveObj, 2f);

        // 延迟调用高级示例
        SimpleTween.delayedCall(gameObject, 0.2f, AdvancedExamples);
    }

    private void AddRotation(GameObject obj, float duration)
    {
        Vector3 startRot = obj.transform.eulerAngles;
        Vector3 endRot = startRot + new Vector3(0, 0, 360f);
        SimpleTween.AddTween(obj, duration, (t) =>
        {
            obj.transform.eulerAngles = SimpleTweenFunc.easeLinear(startRot, endRot, t);
        }).SetLoop(-1);
    }

    private void AddScaleAndMove(GameObject obj, float duration)
    {
        Vector3 startScale = obj.transform.localScale;
        Vector3 bigScale = startScale * 1.7f;
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + Vector3.right * 5f;

        SimpleTween.AddTween(obj, duration, (t) =>
        {
            float eased = EaseOutBounce(t);
            obj.transform.localScale = SimpleTweenFunc.easeLinear(startScale, bigScale, eased);
            obj.transform.position = SimpleTweenFunc.easeLinear(startPos, endPos, eased);
        });
    }

    private void AddMoveWithDelay(GameObject obj, float duration)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 midPos = startPos + new Vector3(-9f, 0, 1f);
        Vector3 endPos = startPos + new Vector3(-15f, 0, 2f);

        SimpleTween.AddTween(obj, duration, (t) =>
        {
            float eased = EaseInQuad(t);
            obj.transform.position = SimpleTweenFunc.easeLinear(startPos, midPos, eased);
        });

        SimpleTween.AddTween(obj, duration, (t) =>
        {
            float eased = EaseInQuad(t);
            obj.transform.position = SimpleTweenFunc.easeLinear(midPos, endPos, eased);
        }).SetDelay(3f);
    }

    private void AdvancedExamples()
    {
        SimpleTween.delayedCall(gameObject, 14f, () =>
        {
            for (int i = 0; i < 10; i++)
            {
                var rotator = new GameObject($"Rotator_{i}");
                rotator.transform.position = new Vector3(10.2f, 2.85f, 0);

                var dude = CreatePrimitive(PrimitiveType.Cube, Vector3.zero, $"Dude_{i}");
                dude.transform.SetParent(rotator.transform);
                dude.transform.localPosition = new Vector3(0, 1.5f, 2.5f * i);

                // Scale pop-in
                dude.transform.localScale = Vector3.zero;
                SimpleTween.AddTween(dude, 1f, (t) =>
                {
                    float eased = EaseOutBack(t);
                    dude.transform.localScale = SimpleTweenFunc.easeLinear(Vector3.zero, Vector3.one * 0.65f, eased);
                }).SetDelay(i * 0.2f);

                // Color rainbow
                float period = Mathf.PI * 2f / 10 * i;
                Color rainbow = new Color(
                    Mathf.Sin(period) * 0.5f + 0.5f,
                    Mathf.Sin(period + Mathf.PI * 2f / 3f) * 0.5f + 0.5f,
                    Mathf.Sin(period + Mathf.PI * 4f / 3f) * 0.5f + 0.5f
                );
                var renderer = dude.GetComponent<Renderer>();
                Color fromColor = renderer.material.color;
                SimpleTween.AddTween(dude, 0.3f, (t) =>
                {
                    renderer.material.color = Color.Lerp(fromColor, rainbow, t);
                }).SetDelay(1.2f + i * 0.4f);

                // Push in Z and rotate parent
                Vector3 pushFrom = dude.transform.localPosition;
                Vector3 pushTo = new Vector3(pushFrom.x, pushFrom.y, 0);
                SimpleTween.AddTween(dude, 0.3f, (t) =>
                {
                    float eased = EaseSpring(t);
                    dude.transform.localPosition = SimpleTweenFunc.easeLinear(pushFrom, pushTo, eased);
                }).SetDelay(1.2f + i * 0.4f);

                // Jump up and down
                Vector3 jumpBase = dude.transform.localPosition;
                Vector3 jumpPeak = jumpBase + Vector3.up * 4f;
                SimpleTween.AddTween(dude, 1.2f, (t) =>
                {
                    dude.transform.localPosition = SimpleTweenFunc.easeLinear(jumpBase, jumpPeak, t);
                }).SetDelay(5f + i * 0.2f).SetLoopPingPong(1);

                // Fade out and destroy
                SimpleTween.AddTween(dude, 0.6f, (t) =>
                {
                    Color c = renderer.material.color;
                    c.a = 1f - t;
                    renderer.material.color = c;
                }).SetDelay(9.2f + i * 0.4f);
            }
        });
    }

    private GameObject CreatePrimitive(PrimitiveType type, Vector3 pos, string name)
    {
        var go = GameObject.CreatePrimitive(type);
        go.transform.position = pos;
        go.name = name;
        return go;
    }

    // ---- Easing helpers (mirror LeanTween) ----
    private static float EaseOutBounce(float t)
    {
        t = Mathf.Clamp01(t);
        const float n1 = 7.5625f, d1 = 2.75f;
        if (t < 1f / d1) return n1 * t * t;
        else if (t < 2f / d1) { t -= 1.5f / d1; return n1 * t * t + 0.75f; }
        else if (t < 2.5f / d1) { t -= 2.25f / d1; return n1 * t * t + 0.9375f; }
        else { t -= 2.625f / d1; return n1 * t * t + 0.984375f; }
    }

    private static float EaseInQuad(float t) => t * t;

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private static float EaseSpring(float t)
    {
        t = Mathf.Clamp01(t);
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.075f) * (2f * Mathf.PI) / 0.3f) + 1f;
    }
}
