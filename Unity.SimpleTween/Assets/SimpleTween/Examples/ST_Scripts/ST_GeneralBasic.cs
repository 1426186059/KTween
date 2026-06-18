using UnityEngine;

/// <summary>
/// SimpleTween 版本的 GeneralBasic
/// 场景物体已预置，脚本只负责驱动动画
/// </summary>
public class ST_GeneralBasic : MonoBehaviour
{
    public GameObject prefabAvatar;

    private void Start()
    {
        var avatarRotate = GameObject.Find("AvatarRotate");
        var avatarScale = GameObject.Find("AvatarScale");
        var avatarMove = GameObject.Find("AvatarMove");
        if (avatarRotate == null || avatarScale == null || avatarMove == null) return;

        // 旋转
        Vector3 startRot = avatarRotate.transform.eulerAngles;
        SimpleTween.AddTween(avatarRotate, 5f, t =>
        {
            avatarRotate.transform.eulerAngles = SimpleTweenFunc.easeLinear(startRot, startRot + new Vector3(0, 0, 360f), t);
        }).SetLoop(-1);

        // 缩放 + 移动同时
        Vector3 sScale = avatarScale.transform.localScale;
        Vector3 sPos = avatarScale.transform.position;
        SimpleTween.AddTween(avatarScale, 5f, t =>
        {
            float e = EaseOutBounce(t);
            avatarScale.transform.localScale = SimpleTweenFunc.easeLinear(sScale, sScale * 1.7f, e);
            avatarScale.transform.position = SimpleTweenFunc.easeLinear(sPos, sPos + Vector3.right * 5f, e);
        });

        // 移动
        Vector3 mPos = avatarMove.transform.position;
        SimpleTween.AddTween(avatarMove, 2f, t =>
        {
            avatarMove.transform.position = SimpleTweenFunc.easeLinear(mPos, mPos + new Vector3(-9f, 0, 1f), EaseInQuad(t));
        });
        SimpleTween.AddTween(avatarMove, 2f, t =>
        {
            avatarMove.transform.position = SimpleTweenFunc.easeLinear(mPos + new Vector3(-9f, 0, 1f), mPos + new Vector3(-15f, 0, 2f), EaseInQuad(t));
        }).SetDelay(3f);

        // 链式 PingPong 缩放
        SimpleTween.AddTween(avatarScale, 1f, t =>
        {
            avatarScale.transform.localScale = SimpleTweenFunc.easeLinear(sScale * 1.7f, Vector3.one * 0.2f, EaseInOutCirc(t));
        }).SetDelay(7f).SetLoopPingPong(3);

        SimpleTween.delayedCall(gameObject, 0.2f, AdvancedExamples);
    }

    private void AdvancedExamples()
    {
        SimpleTween.delayedCall(gameObject, 14f, () =>
        {
            for (int i = 0; i < 10; i++)
            {
                var rotator = new GameObject("rotator" + i);
                rotator.transform.position = new Vector3(10.2f, 2.85f, 0);

                GameObject dude;
                if (prefabAvatar != null)
                {
                    dude = Instantiate(prefabAvatar, Vector3.zero, prefabAvatar.transform.rotation);
                }
                else
                {
                    dude = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    dude.GetComponent<Renderer>().material.color = Color.white;
                }
                dude.transform.SetParent(rotator.transform);
                dude.transform.localPosition = new Vector3(0, 1.5f, 2.5f * i);

                dude.transform.localScale = Vector3.zero;
                SimpleTween.AddTween(dude, 1f, t =>
                {
                    dude.transform.localScale = Vector3.one * 0.65f * EaseOutBack(t);
                }).SetDelay(i * 0.2f);

                float period = Mathf.PI * 2f / 10 * i;
                Color rainbow = new Color(
                    Mathf.Sin(period) * 0.5f + 0.5f,
                    Mathf.Sin(period + Mathf.PI * 2f / 3f) * 0.5f + 0.5f,
                    Mathf.Sin(period + Mathf.PI * 4f / 3f) * 0.5f + 0.5f);
                var renderer = dude.GetComponent<Renderer>();
                Color c0 = renderer.material.color;
                SimpleTween.AddTween(dude, 0.3f, t =>
                {
                    renderer.material.color = Color.Lerp(c0, rainbow, t);
                }).SetDelay(1.2f + i * 0.4f);

                Vector3 pushFrom = dude.transform.localPosition;
                Vector3 pushTo = new Vector3(pushFrom.x, pushFrom.y, 0);
                SimpleTween.AddTween(dude, 0.3f, t =>
                {
                    dude.transform.localPosition = SimpleTweenFunc.easeLinear(pushFrom, pushTo, EaseSpring(t));
                }).SetDelay(1.2f + i * 0.4f);

                Vector3 jumpBase = dude.transform.localPosition;
                SimpleTween.AddTween(dude, 1.2f, t =>
                {
                    dude.transform.localPosition = SimpleTweenFunc.easeLinear(jumpBase, jumpBase + Vector3.up * 4f, EaseInOutQuad(t));
                }).SetDelay(5f + i * 0.2f).SetLoopPingPong(1);

                var ren = renderer;
                Color origColor = ren.material.color;
                SimpleTween.AddTween(dude, 0.6f, t =>
                {
                    Color c = ren.material.color;
                    ren.material.color = new Color(c.r, c.g, c.b, 1f - t);
                }).SetDelay(9.2f + i * 0.4f);
            }
        });
    }

    private static float EaseOutBounce(float t) { t = Mathf.Clamp01(t); const float n1 = 7.5625f, d1 = 2.75f; if (t < 1f / d1) return n1 * t * t; if (t < 2f / d1) { t -= 1.5f / d1; return n1 * t * t + 0.75f; } if (t < 2.5f / d1) { t -= 2.25f / d1; return n1 * t * t + 0.9375f; } t -= 2.625f / d1; return n1 * t * t + 0.984375f; }
    private static float EaseInQuad(float t) => t * t;
    private static float EaseOutBack(float t) { const float c1 = 1.70158f, c3 = c1 + 1f; return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f); }
    private static float EaseSpring(float t) { t = Mathf.Clamp01(t); return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.075f) * (2f * Mathf.PI) / 0.3f) + 1f; }
    private static float EaseInOutCirc(float t) => t < 0.5f ? (1f - Mathf.Sqrt(1f - 4f * t * t)) / 2f : (Mathf.Sqrt(1f - (-2f * t + 2f) * (-2f * t + 2f)) + 1f) / 2f;
    private static float EaseInOutQuad(float t) => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
}
