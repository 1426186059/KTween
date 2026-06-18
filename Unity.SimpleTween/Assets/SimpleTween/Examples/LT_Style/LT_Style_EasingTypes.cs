using UnityEngine;

/// <summary>
/// LeanTween GeneralEasingTypes 风格示例
/// 可视化展示 10 种缓动曲线
/// 每条曲线用一个小球来回运动展示
/// </summary>
public class LT_Style_EasingTypes : MonoBehaviour
{
    public float lineDrawScale = 10f;

    private void Start()
    {
        var types = new (string name, System.Func<float, float> func, Color color)[]
        {
            ("Linear",     t => t, Color.gray),
            ("Quad In",    t => t * t, Color.blue),
            ("Quad Out",   t => t * (2f - t), Color.cyan),
            ("Quad InOut", t => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t, new Color(0, 0.7f, 0.9f)),
            ("Cubic In",   t => t * t * t, new Color(0.3f, 0.5f, 1f)),
            ("Cubic Out",  t => (t - 1f) * (t - 1f) * (t - 1f) + 1f, Color.green),
            ("Sine InOut", t => 0.5f * (1f - Mathf.Cos(t * Mathf.PI)), new Color(0.5f, 1f, 0.5f)),
            ("Bounce Out", BounceOut, Color.yellow),
            ("Elastic Out", ElasticOut, Color.magenta),
            ("Back Out",   BackOut, new Color(1f, 0.5f, 0)),
        };

        float startY = 4f;
        for (int i = 0; i < types.Length; i++)
        {
            int idx = i;
            var pos = new Vector3(-3f, startY - i * 0.9f, 0);
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = pos;
            sphere.transform.localScale = Vector3.one * 0.4f;
            sphere.name = types[i].name;
            sphere.GetComponent<Renderer>().material.color = types[i].color;

            Vector3 from = pos;
            Vector3 to = pos + Vector3.right * lineDrawScale * 0.5f;

            SimpleTween.AddTween(sphere, 2f, (t) =>
            {
                float eased = types[idx].func(t);
                sphere.transform.position = SimpleTweenFunc.easeLinear(from, to, eased);
            }).SetLoopPingPong(-1);
        }
    }

    private static float BounceOut(float t)
    {
        t = Mathf.Clamp01(t);
        const float n1 = 7.5625f, d1 = 2.75f;
        if (t < 1f / d1) return n1 * t * t;
        if (t < 2f / d1) { t -= 1.5f / d1; return n1 * t * t + 0.75f; }
        if (t < 2.5f / d1) { t -= 2.25f / d1; return n1 * t * t + 0.9375f; }
        t -= 2.625f / d1;
        return n1 * t * t + 0.984375f;
    }

    private static float ElasticOut(float t)
    {
        t = Mathf.Clamp01(t);
        if (t <= 0) return 0;
        if (t >= 1) return 1;
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * 2.0943951f) + 1f;
    }

    private static float BackOut(float t)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
