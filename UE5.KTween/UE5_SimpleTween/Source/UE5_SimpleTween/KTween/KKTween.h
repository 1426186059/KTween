#pragma once

#include "KKTweenHead.h"
#include "CoreMinimal.h"

using namespace KKTweenAPI;

class KKTween
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
        TWeakPtr<KKTweenItem> mInnerPtr;

        friend class KKTween;
        Handle(TSharedPtr<KKTweenItem> mItem)
        {
            this->mInnerPtr = mItem;
            this->nVersion = mItem->nVersion;
            check(this->nVersion > 0);
        }
    public:
        Handle()
        {

        }

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

        void Reset()
        {
            Cancel();
        }
    };

    class EaseFunc
    {
    public:
        template<typename T>
        static T easeLinear(T from, T to, float fTimePercent)
        {
            return from * (1 - fTimePercent) + to * fTimePercent;
        }

        template<typename T>
        static T easeInOutQuad(T from, T to, float timePercent)
        {
            timePercent = timePercent * 2.0;
            to = to - from;
            if (timePercent < 1.0)
            {
                return to / 2 * timePercent * timePercent + from;
            }

            timePercent -= 1.0;
            return -to / 2 * (timePercent * (timePercent - 2) - 1) + from;
        }

        template<typename T>
        static T easeInOutSine(T from, T to, float timePercent)
        {
            to = to - from;
            return -to / 2 * (FMath::Cos(PI * timePercent) - 1) + from;
        }

        template<typename T>
        static T easeInOutQuart(T from, T to, float timePercent)
        {
            timePercent = timePercent * 2.0;
            to = to - from;
            if (timePercent < 1.0)
            {
                return to / 2.0 * timePercent * timePercent * timePercent * timePercent + from;
            }

            timePercent = timePercent - 2;
            return -to / 2.0 * (timePercent * timePercent * timePercent * timePercent - 2) + from;
        }
        
        template<typename T>
        static T easeOutQuad(T from, T to, float timePercent)
        {
            T diff = to - from;
            float val = -timePercent * (timePercent - 2);
            return diff * val + from;
        }

        template<typename T>
        static T easeInQuad(T from, T to, float timePercent)
        {
            T diff = to - from;
            float val = timePercent * timePercent;
            return diff * val + from;
        }

        template<typename T>
        static T easeOutSine(T from, T to, float timePercent)
        {
            T diff = to - from;
            float val = FMath::Sin(timePercent * PI / 2);
            return from + diff * val;
        }

        template<typename T>
        static T easeInSine(T from, T to, float timePercent)
        {
            T diff = to - from;
            float val = -FMath::Cos(timePercent * PI / 2);
            return from + diff + diff * val;
        }

        //static FVector tweenOnCurve(FVector from, FVector to, float timePercent, animationCurve)
        //{
        //    return from + (to - from) * animationCurve:Evaluate(timePercent);
        //}

          //local LTBezierPath = CS.LTBezierPath(to);
  /*      function TweenCommonFunc : tweenOnBezierPath(val, mLTBezierPath)
            return mLTBezierPath : point(val);
        end*/

    private:
        EaseFunc() = delete;
        ~EaseFunc() = delete;
        EaseFunc(const EaseFunc&) = delete;
        EaseFunc& operator=(const EaseFunc&) = delete;
        EaseFunc(EaseFunc&&) = delete;
        EaseFunc& operator=(EaseFunc&&) = delete;
    };

public:
    static void SetMaxTweenCount(int nCount)
    {
        return AKKTweenMgr::GetSingleton()->SetMaxTweenCount(nCount);
    }

    static Handle GetHandle(TSharedPtr<KKTweenItem> mTSharePtr)
    {
        return KKTween::Handle(mTSharePtr);
    }

    static void CancelAll()
    {
        AKKTweenMgr::GetSingleton()->CancelAll();
    }

    static void Cancel(UObject* obj)
    {
        AKKTweenMgr::GetSingleton()->Cancel(obj);
    }

    static TSharedPtr<KKTweenItem> AddTween(float time, Action_Float_Delegate updateFunc = nullptr, ActionDelegate finishFunc = nullptr)
    {
        return AKKTweenMgr::GetSingleton()->AddTween(nullptr, time, updateFunc, finishFunc);
    }

    static TSharedPtr<KKTweenItem> AddTween(UObject* obj, float time, Action_Float_Delegate updateFunc = nullptr, ActionDelegate finishFunc = nullptr)
    {
        return AKKTweenMgr::GetSingleton()->AddTween(obj, time, updateFunc, finishFunc);
    }
    
    static TSharedPtr<KKTweenItem> delayedCall(float time, ActionDelegate finishFunc = nullptr)
    {
        return AddTween(nullptr, time, nullptr, finishFunc);
    }

    static TSharedPtr<KKTweenItem> delayedCall(UObject* obj, float time, ActionDelegate finishFunc = nullptr)
    {
        return AddTween(obj, time, nullptr, finishFunc);
    }

    template<typename T>
    static TFunction<T(T, T, float)> GetEaseFunc(EaseType nType)
    {
        TFunction<T(T, T, float)> mFunc;

        switch(nType)
        {
            case EaseType::linear: { mFunc = &EaseFunc::easeLinear<T>; break;}
            case EaseType::easeInQuad: { mFunc = &EaseFunc::easeInQuad<T>; break; }
            case EaseType::easeOutQuad: { mFunc = &EaseFunc::easeOutQuad<T>; break; }
            case EaseType::easeInSine: { mFunc = &EaseFunc::easeInSine<T>; break; }
            case EaseType::easeOutSine: { mFunc = &EaseFunc::easeOutSine<T>; break; }
        }

        if (!mFunc.IsSet())
        {
            ensureMsgf(false, TEXT("GetEaseFunc(nType) Miss : %d"), nType);
            mFunc = &EaseFunc::easeLinear<T>;
        }
        return mFunc;
    }
};

   