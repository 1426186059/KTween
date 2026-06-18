using UnityEngine;

public enum SimpleTweenType
{
    notUsed, linear, easeOutQuad, easeInQuad, easeInOutQuad, easeInCubic, easeOutCubic, easeInOutCubic,
    easeInQuart, easeOutQuart, easeInOutQuart,
    easeInQuint, easeOutQuint, easeInOutQuint, easeInSine, easeOutSine, easeInOutSine, easeInExpo, easeOutExpo, easeInOutExpo, easeInCirc, easeOutCirc, easeInOutCirc,
    easeInBounce, easeOutBounce, easeInOutBounce, easeInBack, easeOutBack, easeInOutBack, easeInElastic, easeOutElastic, easeInOutElastic, easeSpring, easeShake, punch, once, clamp, pingPong, animationCurve
}

public static class SimpleTweenFunc
{
    /// <summary>通过枚举选取缓动函数</summary>
    public static float ApplyEase(SimpleTweenType type, float fPercent)
    {
        fPercent = Mathf.Clamp01(fPercent);
        return type switch
        {
            SimpleTweenType.linear          => fPercent,
            SimpleTweenType.easeInQuad      => QuadIn(fPercent),
            SimpleTweenType.easeOutQuad     => QuadOut(fPercent),
            SimpleTweenType.easeInOutQuad   => QuadInOut(fPercent),
            SimpleTweenType.easeInCubic     => CubicIn(fPercent),
            SimpleTweenType.easeOutCubic    => CubicOut(fPercent),
            SimpleTweenType.easeInOutCubic  => CubicInOut(fPercent),
            SimpleTweenType.easeInQuart     => QuartIn(fPercent),
            SimpleTweenType.easeOutQuart    => QuartOut(fPercent),
            SimpleTweenType.easeInOutQuart  => QuartInOut(fPercent),
            SimpleTweenType.easeInQuint     => QuintIn(fPercent),
            SimpleTweenType.easeOutQuint    => QuintOut(fPercent),
            SimpleTweenType.easeInOutQuint  => QuintInOut(fPercent),
            SimpleTweenType.easeInSine      => SineIn(fPercent),
            SimpleTweenType.easeOutSine     => SineOut(fPercent),
            SimpleTweenType.easeInOutSine   => SineInOut(fPercent),
            SimpleTweenType.easeInExpo      => ExpoIn(fPercent),
            SimpleTweenType.easeOutExpo     => ExpoOut(fPercent),
            SimpleTweenType.easeInOutExpo   => ExpoInOut(fPercent),
            SimpleTweenType.easeInCirc      => CircIn(fPercent),
            SimpleTweenType.easeOutCirc     => CircOut(fPercent),
            SimpleTweenType.easeInOutCirc   => CircInOut(fPercent),
            SimpleTweenType.easeInBounce    => 1f - BounceOut(1f - fPercent),
            SimpleTweenType.easeOutBounce   => BounceOut(fPercent),
            SimpleTweenType.easeInOutBounce => fPercent < 0.5f ? (1f - BounceOut(1f - 2f*fPercent))/2f : (1f + BounceOut(2f*fPercent - 1f))/2f,
            SimpleTweenType.easeInBack      => BackIn(fPercent),
            SimpleTweenType.easeOutBack     => BackOut(fPercent),
            SimpleTweenType.easeInOutBack   => BackInOut(fPercent),
            SimpleTweenType.easeInElastic   => ElasticIn(fPercent),
            SimpleTweenType.easeOutElastic  => ElasticOut(fPercent),
            SimpleTweenType.easeInOutElastic=> ElasticInOut(fPercent),
            SimpleTweenType.easeSpring      => Spring(fPercent),
            SimpleTweenType.easeShake       => Shake(fPercent),
            SimpleTweenType.punch           => Punch(fPercent),
            _ => fPercent,
        };
    }

    // ==================== 3 重载模式：Vector3 / Vector2 / float ====================

    // --- easeLinear ---
    public static Vector3 easeLinear(Vector3 from, Vector3 to, float fPercent) => Vector3.LerpUnclamped(from, to, fPercent);
    public static Vector2 easeLinear(Vector2 from, Vector2 to, float fPercent) => Vector2.LerpUnclamped(from, to, fPercent);
    public static float   easeLinear(float from, float to, float fPercent)   => Mathf.LerpUnclamped(from, to, fPercent);

    // --- easeInQuad ---
    public static Vector3 easeInQuad(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuadIn(fPercent));
    public static Vector2 easeInQuad(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuadIn(fPercent));
    public static float   easeInQuad(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuadIn(fPercent));

    // --- easeOutQuad ---
    public static Vector3 easeOutQuad(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuadOut(fPercent));
    public static Vector2 easeOutQuad(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuadOut(fPercent));
    public static float   easeOutQuad(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuadOut(fPercent));

    // --- easeInOutQuad ---
    public static Vector3 easeInOutQuad(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuadInOut(fPercent));
    public static Vector2 easeInOutQuad(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuadInOut(fPercent));
    public static float   easeInOutQuad(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuadInOut(fPercent));

    // --- easeInCubic ---
    public static Vector3 easeInCubic(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, CubicIn(fPercent));
    public static Vector2 easeInCubic(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, CubicIn(fPercent));
    public static float   easeInCubic(float from, float to, float fPercent)     => Mathf.Lerp(from, to, CubicIn(fPercent));

    // --- easeOutCubic ---
    public static Vector3 easeOutCubic(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, CubicOut(fPercent));
    public static Vector2 easeOutCubic(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, CubicOut(fPercent));
    public static float   easeOutCubic(float from, float to, float fPercent)     => Mathf.Lerp(from, to, CubicOut(fPercent));

    // --- easeInOutCubic ---
    public static Vector3 easeInOutCubic(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, CubicInOut(fPercent));
    public static Vector2 easeInOutCubic(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, CubicInOut(fPercent));
    public static float   easeInOutCubic(float from, float to, float fPercent)     => Mathf.Lerp(from, to, CubicInOut(fPercent));

    // --- easeInQuart ---
    public static Vector3 easeInQuart(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuartIn(fPercent));
    public static Vector2 easeInQuart(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuartIn(fPercent));
    public static float   easeInQuart(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuartIn(fPercent));

    // --- easeOutQuart ---
    public static Vector3 easeOutQuart(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuartOut(fPercent));
    public static Vector2 easeOutQuart(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuartOut(fPercent));
    public static float   easeOutQuart(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuartOut(fPercent));

    // --- easeInOutQuart ---
    public static Vector3 easeInOutQuart(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuartInOut(fPercent));
    public static Vector2 easeInOutQuart(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuartInOut(fPercent));
    public static float   easeInOutQuart(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuartInOut(fPercent));

    // --- easeInQuint ---
    public static Vector3 easeInQuint(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuintIn(fPercent));
    public static Vector2 easeInQuint(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuintIn(fPercent));
    public static float   easeInQuint(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuintIn(fPercent));

    // --- easeOutQuint ---
    public static Vector3 easeOutQuint(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuintOut(fPercent));
    public static Vector2 easeOutQuint(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuintOut(fPercent));
    public static float   easeOutQuint(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuintOut(fPercent));

    // --- easeInOutQuint ---
    public static Vector3 easeInOutQuint(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, QuintInOut(fPercent));
    public static Vector2 easeInOutQuint(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, QuintInOut(fPercent));
    public static float   easeInOutQuint(float from, float to, float fPercent)     => Mathf.Lerp(from, to, QuintInOut(fPercent));

    // --- easeInSine ---
    public static Vector3 easeInSine(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, SineIn(fPercent));
    public static Vector2 easeInSine(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, SineIn(fPercent));
    public static float   easeInSine(float from, float to, float fPercent)     => Mathf.Lerp(from, to, SineIn(fPercent));

    // --- easeOutSine ---
    public static Vector3 easeOutSine(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, SineOut(fPercent));
    public static Vector2 easeOutSine(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, SineOut(fPercent));
    public static float   easeOutSine(float from, float to, float fPercent)     => Mathf.Lerp(from, to, SineOut(fPercent));

    // --- easeInOutSine ---
    public static Vector3 easeInOutSine(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, SineInOut(fPercent));
    public static Vector2 easeInOutSine(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, SineInOut(fPercent));
    public static float   easeInOutSine(float from, float to, float fPercent)     => Mathf.Lerp(from, to, SineInOut(fPercent));

    // --- easeInExpo ---
    public static Vector3 easeInExpo(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, ExpoIn(fPercent));
    public static Vector2 easeInExpo(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, ExpoIn(fPercent));
    public static float   easeInExpo(float from, float to, float fPercent)     => Mathf.Lerp(from, to, ExpoIn(fPercent));

    // --- easeOutExpo ---
    public static Vector3 easeOutExpo(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, ExpoOut(fPercent));
    public static Vector2 easeOutExpo(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, ExpoOut(fPercent));
    public static float   easeOutExpo(float from, float to, float fPercent)     => Mathf.Lerp(from, to, ExpoOut(fPercent));

    // --- easeInOutExpo ---
    public static Vector3 easeInOutExpo(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, ExpoInOut(fPercent));
    public static Vector2 easeInOutExpo(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, ExpoInOut(fPercent));
    public static float   easeInOutExpo(float from, float to, float fPercent)     => Mathf.Lerp(from, to, ExpoInOut(fPercent));

    // --- easeInCirc ---
    public static Vector3 easeInCirc(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, CircIn(fPercent));
    public static Vector2 easeInCirc(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, CircIn(fPercent));
    public static float   easeInCirc(float from, float to, float fPercent)     => Mathf.Lerp(from, to, CircIn(fPercent));

    // --- easeOutCirc ---
    public static Vector3 easeOutCirc(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, CircOut(fPercent));
    public static Vector2 easeOutCirc(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, CircOut(fPercent));
    public static float   easeOutCirc(float from, float to, float fPercent)     => Mathf.Lerp(from, to, CircOut(fPercent));

    // --- easeInOutCirc ---
    public static Vector3 easeInOutCirc(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, CircInOut(fPercent));
    public static Vector2 easeInOutCirc(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, CircInOut(fPercent));
    public static float   easeInOutCirc(float from, float to, float fPercent)     => Mathf.Lerp(from, to, CircInOut(fPercent));

    // --- easeInBounce ---
    public static Vector3 easeInBounce(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, 1f - BounceOut(1f - fPercent));
    public static Vector2 easeInBounce(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, 1f - BounceOut(1f - fPercent));
    public static float   easeInBounce(float from, float to, float fPercent)     => Mathf.Lerp(from, to, 1f - BounceOut(1f - fPercent));

    // --- easeOutBounce ---
    public static Vector3 easeOutBounce(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, BounceOut(fPercent));
    public static Vector2 easeOutBounce(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, BounceOut(fPercent));
    public static float   easeOutBounce(float from, float to, float fPercent)     => Mathf.Lerp(from, to, BounceOut(fPercent));

    // --- easeInOutBounce ---
    public static Vector3 easeInOutBounce(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, fPercent < 0.5f ? (1f-BounceOut(1f-2f*fPercent))/2f : (1f+BounceOut(2f*fPercent-1f))/2f);
    public static Vector2 easeInOutBounce(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, fPercent < 0.5f ? (1f-BounceOut(1f-2f*fPercent))/2f : (1f+BounceOut(2f*fPercent-1f))/2f);
    public static float   easeInOutBounce(float from, float to, float fPercent)     => Mathf.Lerp(from, to, fPercent < 0.5f ? (1f-BounceOut(1f-2f*fPercent))/2f : (1f+BounceOut(2f*fPercent-1f))/2f);

    // --- easeInBack ---
    public static Vector3 easeInBack(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, BackIn(fPercent));
    public static Vector2 easeInBack(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, BackIn(fPercent));
    public static float   easeInBack(float from, float to, float fPercent)     => Mathf.Lerp(from, to, BackIn(fPercent));

    // --- easeOutBack ---
    public static Vector3 easeOutBack(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, BackOut(fPercent));
    public static Vector2 easeOutBack(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, BackOut(fPercent));
    public static float   easeOutBack(float from, float to, float fPercent)     => Mathf.Lerp(from, to, BackOut(fPercent));

    // --- easeInOutBack ---
    public static Vector3 easeInOutBack(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, BackInOut(fPercent));
    public static Vector2 easeInOutBack(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, BackInOut(fPercent));
    public static float   easeInOutBack(float from, float to, float fPercent)     => Mathf.Lerp(from, to, BackInOut(fPercent));

    // --- easeInElastic ---
    public static Vector3 easeInElastic(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, ElasticIn(fPercent));
    public static Vector2 easeInElastic(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, ElasticIn(fPercent));
    public static float   easeInElastic(float from, float to, float fPercent)     => Mathf.Lerp(from, to, ElasticIn(fPercent));

    // --- easeOutElastic ---
    public static Vector3 easeOutElastic(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, ElasticOut(fPercent));
    public static Vector2 easeOutElastic(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, ElasticOut(fPercent));
    public static float   easeOutElastic(float from, float to, float fPercent)     => Mathf.Lerp(from, to, ElasticOut(fPercent));

    // --- easeInOutElastic ---
    public static Vector3 easeInOutElastic(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, ElasticInOut(fPercent));
    public static Vector2 easeInOutElastic(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, ElasticInOut(fPercent));
    public static float   easeInOutElastic(float from, float to, float fPercent)     => Mathf.Lerp(from, to, ElasticInOut(fPercent));

    // --- easeSpring ---
    public static Vector3 easeSpring(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, Spring(fPercent));
    public static Vector2 easeSpring(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, Spring(fPercent));
    public static float   easeSpring(float from, float to, float fPercent)     => Mathf.Lerp(from, to, Spring(fPercent));

    // --- easeShake ---
    public static Vector3 easeShake(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, Shake(fPercent));
    public static Vector2 easeShake(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, Shake(fPercent));
    public static float   easeShake(float from, float to, float fPercent)     => Mathf.Lerp(from, to, Shake(fPercent));

    // --- punch ---
    public static Vector3 punch(Vector3 from, Vector3 to, float fPercent) => Vector3.Lerp(from, to, Punch(fPercent));
    public static Vector2 punch(Vector2 from, Vector2 to, float fPercent) => Vector2.Lerp(from, to, Punch(fPercent));
    public static float   punch(float from, float to, float fPercent)     => Mathf.Lerp(from, to, Punch(fPercent));

    // ==================== 核心缓动函数实现 ====================
    private static float QuadIn(float f) => f * f;
    private static float QuadOut(float f) => f * (2f - f);
    private static float QuadInOut(float f) => f < 0.5f ? 2f * f * f : -1f + (4f - 2f * f) * f;

    private static float CubicIn(float f) => f * f * f;
    private static float CubicOut(float f) { f -= 1f; return f * f * f + 1f; }
    private static float CubicInOut(float f) => f < 0.5f ? 4f * f * f * f : (f - 1f) * (2f * f - 2f) * (2f * f - 2f) + 1f;

    private static float QuartIn(float f) => f * f * f * f;
    private static float QuartOut(float f) { f -= 1f; return 1f - f * f * f * f; }
    private static float QuartInOut(float f) => f < 0.5f ? 8f * f * f * f * f : 1f - 8f * (f - 1f) * (f - 1f) * (f - 1f) * (f - 1f);

    private static float QuintIn(float f) => f * f * f * f * f;
    private static float QuintOut(float f) { f -= 1f; return 1f + f * f * f * f * f; }
    private static float QuintInOut(float f) => f < 0.5f ? 16f * f * f * f * f * f : 1f + 16f * (f - 1f) * (f - 1f) * (f - 1f) * (f - 1f) * (f - 1f);

    private static float SineIn(float f) => 1f - Mathf.Cos(f * Mathf.PI * 0.5f);
    private static float SineOut(float f) => Mathf.Sin(f * Mathf.PI * 0.5f);
    private static float SineInOut(float f) => 0.5f * (1f - Mathf.Cos(f * Mathf.PI));

    private static float ExpoIn(float f) => f <= 0 ? 0 : Mathf.Pow(2f, 10f * f - 10f);
    private static float ExpoOut(float f) => f >= 1 ? 1 : 1f - Mathf.Pow(2f, -10f * f);
    private static float ExpoInOut(float f) => f == 0 || f == 1 ? f : f < 0.5f ? Mathf.Pow(2f, 20f * f - 10f) / 2f : (2f - Mathf.Pow(2f, -20f * f + 10f)) / 2f;

    private static float CircIn(float f) => 1f - Mathf.Sqrt(1f - f * f);
    private static float CircOut(float f) => Mathf.Sqrt(1f - (f - 1f) * (f - 1f));
    private static float CircInOut(float f) => f < 0.5f ? (1f - Mathf.Sqrt(1f - 4f * f * f)) / 2f : (Mathf.Sqrt(1f - (-2f * f + 2f) * (-2f * f + 2f)) + 1f) / 2f;

    private static float BounceOut(float f)
    {
        f = Mathf.Clamp01(f); float n1 = 7.5625f, d1 = 2.75f;
        if (f < 1f / d1) return n1 * f * f;
        if (f < 2f / d1) { f -= 1.5f / d1; return n1 * f * f + 0.75f; }
        if (f < 2.5f / d1) { f -= 2.25f / d1; return n1 * f * f + 0.9375f; }
        f -= 2.625f / d1; return n1 * f * f + 0.984375f;
    }

    private static float BackIn(float f) { float c1 = 1.70158f, c3 = c1 + 1f; return c3 * f * f * f - c1 * f * f; }
    private static float BackOut(float f) { float c1 = 1.70158f, c3 = c1 + 1f; return 1f + c3 * (f - 1f) * (f - 1f) * (f - 1f) + c1 * (f - 1f) * (f - 1f); }
    private static float BackInOut(float f) { float c1 = 1.70158f, c2 = c1 * 1.525f; return f < 0.5f ? ((2f*f)*(2f*f)*((c2+1f)*2f*f - c2))/2f : ((2f*f-2f)*(2f*f-2f)*((c2+1f)*(f*2f-2f)+c2)+2f)/2f; }

    private static float ElasticIn(float f) { if (f <= 0 || f >= 1) return f; float c4 = 2.0943951f; return -Mathf.Pow(2f, 10f*f-10f) * Mathf.Sin((f*10f-10.75f)*c4); }
    private static float ElasticOut(float f) { if (f <= 0 || f >= 1) return f; float c4 = 2.0943951f; return Mathf.Pow(2f, -10f*f) * Mathf.Sin((f*10f-0.75f)*c4) + 1f; }
    private static float ElasticInOut(float f) { if (f <= 0 || f >= 1) return f; float c5 = 1.3962634f; return f < 0.5f ? -(Mathf.Pow(2f,20f*f-10f)*Mathf.Sin((20f*f-11.125f)*c5))/2f : Mathf.Pow(2f,-20f*f+10f)*Mathf.Sin((20f*f-11.125f)*c5)/2f+1f; }

    private static float Spring(float f) { f = Mathf.Clamp01(f); return Mathf.Pow(2f, -10f*f) * Mathf.Sin((f - 0.075f) * Mathf.PI * 2f / 0.3f) + 1f; }
    private static float Shake(float f) { return Mathf.Pow(2f, -10f * f) * Mathf.Sin(f * 7f * Mathf.PI); }
    private static float Punch(float f) { if (f == 0 || f == 1) return 0; return Mathf.Pow(2f, -10f*f) * Mathf.Sin(f * 9f * Mathf.PI); }
}
