#pragma once

#include "KTweenHead.h"
#include "KTweenFunc.h"
#include "CoreMinimal.h"

using namespace KTweenAPI;

class KTween
{
public:
    using EaseType = KTweenAPI::EaseType;
    using EaseFunc = KTweenFunc;

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
            else
            {
                this->mInnerPtr = mOtherTween.mInnerPtr;
                this->nVersion = mOtherTween.mInnerPtr.Pin()->nVersion;
                check(this->nVersion > 0);
            }
        }
        
        void AppendTween(TSharedPtr<KTweenItem> mOtherTween)
        {
            if (IsValid() && mOtherTween.IsValid())
            {
                mInnerPtr.Pin()->AppendTween(mOtherTween.Get());
            }
            else
            {
                this->mInnerPtr = mOtherTween;
                this->nVersion = mOtherTween->nVersion;
                check(this->nVersion > 0);
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

    static void Cancel(Handle handle)
    {
        handle.Cancel();
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
