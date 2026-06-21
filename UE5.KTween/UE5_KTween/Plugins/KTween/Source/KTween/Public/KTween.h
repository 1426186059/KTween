#pragma once

#include "KTweenHead.h"
#include "CoreMinimal.h"

using namespace KTweenAPI;

class KTween
{
public:
    enum EaseType
    {
        linear, easeOutQuad, easeInQuad, easeInOutQuad, easeInCubic, easeOutCubic,
        easeInOutCubic, easeInQuart, easeOutQuart, easeInOutQuart,
        easeInQuint, easeOutQuint, easeInOutQuint, easeInSine, easeOutSine, easeInOutSine,
        easeInExpo, easeOutExpo, easeInOutExpo, easeInCirc, easeOutCirc, easeInOutCirc,
        easeInBounce, easeOutBounce, easeInOutBounce, easeInBack, easeOutBack, easeInOutBack,
        easeInElastic, easeOutElastic, easeInOutElastic,
        easeSpring, easeShake, punch, once, clamp, pingPong, animationCurve
    };

    struct Handle
    {
    private:
        uint32 nVersion = 0;
        TWeakPtr<KTweenItem> mInnerPtr;

        friend class KTween;
        Handle(TSharedPtr<KTweenItem> mItem)
        {
            this->mInnerPtr = mItem;
            this->nVersion = mItem->nVersion;
            check(this->nVersion > 0);
        }
    public:
        Handle() {}

        bool IsValid()
        {
            return this->nVersion > 0 && mInnerPtr.IsValid() && mInnerPtr.Pin()->nVersion == nVersion;
        }

        void AppendTween(Handle mOtherTween)
        {
            if (IsValid() && mOtherTween.IsValid())
            {
                mInnerPtr.Pin()->AppendTween(mOtherTween.mInnerPtr.Pin().Get());
            }
        }

        void Cancel()
        {
            if (IsValid())
            {
                mInnerPtr.Pin()->cancel();
            }
            mInnerPtr.Reset();
            nVersion = 0;
        }

        void Reset() { Cancel(); }
    };

    // ── 缓动函数工具 ──
    class EaseFunc
    {
    public:
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
                case EaseType::easeSpring:    return Spring(fPercent);
                case EaseType::easeShake:     return Shake(fPercent);
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

        // ── 命名便利方法（兼容旧代码） ──
        template<typename T> static T easeLinear(T from, T to, float t)       { return Apply(from, to, t, EaseType::linear); }
        template<typename T> static T easeInQuad(T from, T to, float t)       { return Apply(from, to, t, EaseType::easeInQuad); }
        template<typename T> static T easeOutQuad(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeOutQuad); }
        template<typename T> static T easeInOutQuad(T from, T to, float t)    { return Apply(from, to, t, EaseType::easeInOutQuad); }
        template<typename T> static T easeInCubic(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeInCubic); }
        template<typename T> static T easeOutCubic(T from, T to, float t)     { return Apply(from, to, t, EaseType::easeOutCubic); }
        template<typename T> static T easeInOutCubic(T from, T to, float t)   { return Apply(from, to, t, EaseType::easeInOutCubic); }
        template<typename T> static T easeInQuart(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeInQuart); }
        template<typename T> static T easeOutQuart(T from, T to, float t)     { return Apply(from, to, t, EaseType::easeOutQuart); }
        template<typename T> static T easeInOutQuart(T from, T to, float t)   { return Apply(from, to, t, EaseType::easeInOutQuart); }
        template<typename T> static T easeInQuint(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeInQuint); }
        template<typename T> static T easeOutQuint(T from, T to, float t)     { return Apply(from, to, t, EaseType::easeOutQuint); }
        template<typename T> static T easeInOutQuint(T from, T to, float t)   { return Apply(from, to, t, EaseType::easeInOutQuint); }
        template<typename T> static T easeInSine(T from, T to, float t)       { return Apply(from, to, t, EaseType::easeInSine); }
        template<typename T> static T easeOutSine(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeOutSine); }
        template<typename T> static T easeInOutSine(T from, T to, float t)    { return Apply(from, to, t, EaseType::easeInOutSine); }
        template<typename T> static T easeInExpo(T from, T to, float t)       { return Apply(from, to, t, EaseType::easeInExpo); }
        template<typename T> static T easeOutExpo(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeOutExpo); }
        template<typename T> static T easeInOutExpo(T from, T to, float t)    { return Apply(from, to, t, EaseType::easeInOutExpo); }
        template<typename T> static T easeInCirc(T from, T to, float t)       { return Apply(from, to, t, EaseType::easeInCirc); }
        template<typename T> static T easeOutCirc(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeOutCirc); }
        template<typename T> static T easeInOutCirc(T from, T to, float t)    { return Apply(from, to, t, EaseType::easeInOutCirc); }
        template<typename T> static T easeInBounce(T from, T to, float t)     { return Apply(from, to, t, EaseType::easeInBounce); }
        template<typename T> static T easeOutBounce(T from, T to, float t)    { return Apply(from, to, t, EaseType::easeOutBounce); }
        template<typename T> static T easeInOutBounce(T from, T to, float t)  { return Apply(from, to, t, EaseType::easeInOutBounce); }
        template<typename T> static T easeInBack(T from, T to, float t)       { return Apply(from, to, t, EaseType::easeInBack); }
        template<typename T> static T easeOutBack(T from, T to, float t)      { return Apply(from, to, t, EaseType::easeOutBack); }
        template<typename T> static T easeInOutBack(T from, T to, float t)    { return Apply(from, to, t, EaseType::easeInOutBack); }
        template<typename T> static T easeInElastic(T from, T to, float t)    { return Apply(from, to, t, EaseType::easeInElastic); }
        template<typename T> static T easeOutElastic(T from, T to, float t)   { return Apply(from, to, t, EaseType::easeOutElastic); }
        template<typename T> static T easeInOutElastic(T from, T to, float t) { return Apply(from, to, t, EaseType::easeInOutElastic); }
        template<typename T> static T easeSpring(T from, T to, float t)       { return Apply(from, to, t, EaseType::easeSpring); }
        template<typename T> static T easeShake(T from, T to, float t)        { return Apply(from, to, t, EaseType::easeShake); }
        template<typename T> static T punch(T from, T to, float t)            { return Apply(from, to, t, EaseType::punch); }

    private:
        EaseFunc() = delete;
        ~EaseFunc() = delete;

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

        static float Spring(float f) { f = FMath::Clamp(f, 0.0f, 1.0f); return FMath::Pow(2.0f, -10.0f*f) * FMath::Sin((f - 0.075f) * UE_PI * 2.0f / 0.3f) + 1.0f; }
        static float Shake(float f)  { return FMath::Pow(2.0f, -10.0f * f) * FMath::Sin(f * 7.0f * UE_PI); }
        static float Punch(float f)  { if (f == 0 || f == 1) return 0; return FMath::Pow(2.0f, -10.0f*f) * FMath::Sin(f * 9.0f * UE_PI); }
    };

public:
    // ── API 方法 ──
    static void SetMaxTweenCount(int nCount)
    {
        AKTweenMgr::GetSingleton()->SetMaxTweenCount(nCount);
    }

    static Handle GetHandle(TSharedPtr<KTweenItem> mTSharePtr)
    {
        return KTween::Handle(mTSharePtr);
    }

    static void CancelAll()
    {
        AKTweenMgr::GetSingleton()->CancelAll();
    }

    static void Cancel(UObject* obj)
    {
        AKTweenMgr::GetSingleton()->Cancel(obj);
    }

    static TSharedPtr<KTweenItem> AddTween(float time, Action_Float_Delegate updateFunc = nullptr, ActionDelegate finishFunc = nullptr)
    {
        return AKTweenMgr::GetSingleton()->AddTween(nullptr, time, updateFunc, finishFunc);
    }

    static TSharedPtr<KTweenItem> AddTween(UObject* obj, float time, Action_Float_Delegate updateFunc = nullptr, ActionDelegate finishFunc = nullptr)
    {
        return AKTweenMgr::GetSingleton()->AddTween(obj, time, updateFunc, finishFunc);
    }

    static TSharedPtr<KTweenItem> delayedCall(float time, ActionDelegate finishFunc = nullptr)
    {
        return AddTween(nullptr, time, nullptr, finishFunc);
    }

    static TSharedPtr<KTweenItem> delayedCall(UObject* obj, float time, ActionDelegate finishFunc = nullptr)
    {
        return AddTween(obj, time, nullptr, finishFunc);
    }

    /// 获取缓动函数（兼容旧代码）
    template<typename T>
    static TFunction<T(T, T, float)> GetEaseFunc(EaseType nType)
    {
        return [nType](T from, T to, float t) -> T
        {
            return EaseFunc::Apply(from, to, t, nType);
        };
    }
};
