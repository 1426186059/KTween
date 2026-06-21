#pragma once

#include "CoreMinimal.h"
#include "KTweenHead.h"

/// <summary>
/// 缓动函数工具类 — 与 Unity KTweenFunc 完全一致
/// 核心方法：ApplyEase(type, percent) → eased percent
/// </summary>
class KTweenFunc
{
public:
    using EaseType = KTweenAPI::EaseType;

    /// 核心方法：输入 0~1 的百分比，返回缓动后的百分比
    static float ApplyEase(EaseType type, float fPercent)
    {
        fPercent = FMath::Clamp(fPercent, 0.0f, 1.0f);
        switch (type)
        {
            case EaseType::linear:        return fPercent;
            case EaseType::easeInQuad:    return QuadIn(fPercent);
            case EaseType::easeOutQuad:   return QuadOut(fPercent);
            case EaseType::easeInOutQuad: return QuadInOut(fPercent);
            case EaseType::easeInCubic:   return CubicIn(fPercent);
            case EaseType::easeOutCubic:  return CubicOut(fPercent);
            case EaseType::easeInOutCubic:return CubicInOut(fPercent);
            case EaseType::easeInQuart:   return QuartIn(fPercent);
            case EaseType::easeOutQuart:  return QuartOut(fPercent);
            case EaseType::easeInOutQuart:return QuartInOut(fPercent);
            case EaseType::easeInQuint:   return QuintIn(fPercent);
            case EaseType::easeOutQuint:  return QuintOut(fPercent);
            case EaseType::easeInOutQuint:return QuintInOut(fPercent);
            case EaseType::easeInSine:    return SineIn(fPercent);
            case EaseType::easeOutSine:   return SineOut(fPercent);
            case EaseType::easeInOutSine: return SineInOut(fPercent);
            case EaseType::easeInExpo:    return ExpoIn(fPercent);
            case EaseType::easeOutExpo:   return ExpoOut(fPercent);
            case EaseType::easeInOutExpo: return ExpoInOut(fPercent);
            case EaseType::easeInCirc:    return CircIn(fPercent);
            case EaseType::easeOutCirc:   return CircOut(fPercent);
            case EaseType::easeInOutCirc: return CircInOut(fPercent);
            case EaseType::easeInBounce:  return 1.0f - BounceOut(1.0f - fPercent);
            case EaseType::easeOutBounce: return BounceOut(fPercent);
            case EaseType::easeInOutBounce: return fPercent < 0.5f ? (1.0f - BounceOut(1.0f - 2.0f*fPercent))*0.5f : (1.0f + BounceOut(2.0f*fPercent - 1.0f))*0.5f;
            case EaseType::easeInBack:   return BackIn(fPercent);
            case EaseType::easeOutBack:  return BackOut(fPercent);
            case EaseType::easeInOutBack:return BackInOut(fPercent);
            case EaseType::easeInElastic: return ElasticIn(fPercent);
            case EaseType::easeOutElastic:return ElasticOut(fPercent);
            case EaseType::easeInOutElastic: return ElasticInOut(fPercent);
            case EaseType::easeSpring:    return easeSpring(fPercent);
            case EaseType::easeShake:     return easeShake(fPercent);
            case EaseType::punch:         return Punch(fPercent);
            default: return fPercent;
        }
    }

    // ── 模板便利方法：自动 Lerp + ApplyEase ──
    template<typename T>
    static T Apply(T from, T to, float fPercent, EaseType type)
    {
        float t = ApplyEase(type, fPercent);
        return from * (1.0f - t) + to * t;
    }

private:
    KTweenFunc() = delete;
    ~KTweenFunc() = delete;

    // ==================== 核心缓动函数实现 ====================
    static float QuadIn(float f)     { return f * f; }
    static float QuadOut(float f)    { return f * (2.0f - f); }
    static float QuadInOut(float f)  { return f < 0.5f ? 2.0f * f * f : -1.0f + (4.0f - 2.0f * f) * f; }

    static float CubicIn(float f)    { return f * f * f; }
    static float CubicOut(float f)   { f -= 1.0f; return f * f * f + 1.0f; }
    static float CubicInOut(float f) { return f < 0.5f ? 4.0f * f * f * f : (f - 1.0f) * (2.0f * f - 2.0f) * (2.0f * f - 2.0f) + 1.0f; }

    static float QuartIn(float f)    { return f * f * f * f; }
    static float QuartOut(float f)   { f -= 1.0f; return 1.0f - f * f * f * f; }
    static float QuartInOut(float f) { return f < 0.5f ? 8.0f * f * f * f * f : 1.0f - 8.0f * (f - 1.0f) * (f - 1.0f) * (f - 1.0f) * (f - 1.0f); }

    static float QuintIn(float f)    { return f * f * f * f * f; }
    static float QuintOut(float f)   { f -= 1.0f; return 1.0f + f * f * f * f * f; }
    static float QuintInOut(float f) { return f < 0.5f ? 16.0f * f * f * f * f * f : 1.0f + 16.0f * (f - 1.0f) * (f - 1.0f) * (f - 1.0f) * (f - 1.0f) * (f - 1.0f); }

    static float SineIn(float f)    { return 1.0f - FMath::Cos(f * UE_PI * 0.5f); }
    static float SineOut(float f)   { return FMath::Sin(f * UE_PI * 0.5f); }
    static float SineInOut(float f) { return 0.5f * (1.0f - FMath::Cos(f * UE_PI)); }

    static float ExpoIn(float f)    { return f <= 0 ? 0 : FMath::Pow(2.0f, 10.0f * f - 10.0f); }
    static float ExpoOut(float f)   { return f >= 1 ? 1 : 1.0f - FMath::Pow(2.0f, -10.0f * f); }
    static float ExpoInOut(float f) { return f == 0 || f == 1 ? f : f < 0.5f ? FMath::Pow(2.0f, 20.0f * f - 10.0f) * 0.5f : (2.0f - FMath::Pow(2.0f, -20.0f * f + 10.0f)) * 0.5f; }

    static float CircIn(float f)    { return 1.0f - FMath::Sqrt(1.0f - f * f); }
    static float CircOut(float f)   { return FMath::Sqrt(1.0f - (f - 1.0f) * (f - 1.0f)); }
    static float CircInOut(float f) { return f < 0.5f ? (1.0f - FMath::Sqrt(1.0f - 4.0f * f * f)) * 0.5f : (FMath::Sqrt(1.0f - (-2.0f * f + 2.0f) * (-2.0f * f + 2.0f)) + 1.0f) * 0.5f; }

    static float BounceOut(float f)
    {
        f = FMath::Clamp(f, 0.0f, 1.0f);
        float n1 = 7.5625f, d1 = 2.75f;
        if (f < 1.0f / d1) return n1 * f * f;
        if (f < 2.0f / d1) { f -= 1.5f / d1; return n1 * f * f + 0.75f; }
        if (f < 2.5f / d1) { f -= 2.25f / d1; return n1 * f * f + 0.9375f; }
        f -= 2.625f / d1; return n1 * f * f + 0.984375f;
    }

    static float BackIn(float f)    { float c1 = 1.70158f, c3 = c1 + 1.0f; return c3 * f * f * f - c1 * f * f; }
    static float BackOut(float f)   { float c1 = 1.70158f, c3 = c1 + 1.0f; return 1.0f + c3 * (f - 1.0f) * (f - 1.0f) * (f - 1.0f) + c1 * (f - 1.0f) * (f - 1.0f); }
    static float BackInOut(float f) { float c1 = 1.70158f, c2 = c1 * 1.525f; return f < 0.5f ? ((2.0f*f)*(2.0f*f)*((c2+1.0f)*2.0f*f - c2))*0.5f : ((2.0f*f-2.0f)*(2.0f*f-2.0f)*((c2+1.0f)*(f*2.0f-2.0f)+c2)+2.0f)*0.5f; }

    static float ElasticIn(float f)    { if (f <= 0 || f >= 1) return f; float c4 = 2.0943951f; return -FMath::Pow(2.0f, 10.0f*f-10.0f) * FMath::Sin((f*10.0f-10.75f)*c4); }
    static float ElasticOut(float f)   { if (f <= 0 || f >= 1) return f; float c4 = 2.0943951f; return FMath::Pow(2.0f, -10.0f*f) * FMath::Sin((f*10.0f-0.75f)*c4) + 1.0f; }
    static float ElasticInOut(float f) { if (f <= 0 || f >= 1) return f; float c5 = 1.3962634f; return f < 0.5f ? -(FMath::Pow(2.0f,20.0f*f-10.0f)*FMath::Sin((20.0f*f-11.125f)*c5))*0.5f : FMath::Pow(2.0f,-20.0f*f+10.0f)*FMath::Sin((20.0f*f-11.125f)*c5)*0.5f+1.0f; }

    static float easeSpring(float f) { f = FMath::Clamp(f, 0.0f, 1.0f); return FMath::Pow(2.0f, -10.0f*f) * FMath::Sin((f - 0.075f) * UE_PI * 2.0f / 0.3f) + 1.0f; }
    static float easeShake(float f)  { return FMath::Pow(2.0f, -10.0f * f) * FMath::Sin(f * 7.0f * UE_PI); }
    static float Punch(float f)  { if (f == 0 || f == 1) return 0; return FMath::Pow(2.0f, -10.0f*f) * FMath::Sin(f * 9.0f * UE_PI); }
};
