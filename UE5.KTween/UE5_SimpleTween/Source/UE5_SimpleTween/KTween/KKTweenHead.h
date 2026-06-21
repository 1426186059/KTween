#pragma once

#define DEFAULT_TWEEN_MAX_COUNT 1500
#define USE_LinkedList

#include "UE5_Solitaire/SimpleFramework/KKActorSingleton.h"
#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "KKTweenHead.generated.h"

class KKTween;
namespace KKTweenAPI
{
    //typedef void (*ActionDelegate)(); //C语言定义方法指针
    //typedef void (*Action_Float_Delegate)(float); //C语言定义方法指针

    // 修改你的 typedef
    typedef TFunction<void(void)> ActionDelegate;
    typedef TFunction<void(float)> Action_Float_Delegate;

    class KKTweenByList;
    class KKTweenItem;
    class ObjectPool;

    class KKTweenItem:public TSharedFromThis<KKTweenItem>
    {
        friend class ObjectPool;
    public:
        TWeakObjectPtr<UObject> bindObj;
        bool toggle;
        float delay;
        float time;
        float sumTime;
        int32 nLoopCount;
        int32 nLoopPingTong;
        uint32 nVersion;
        Action_Float_Delegate updateFunc;
        ActionDelegate finishFunc;
        ActionDelegate startFunc;

        FVector From;
        FVector To;

#ifdef USE_LinkedList
        TDoubleLinkedList<TSharedPtr<KKTweenItem>>::TDoubleLinkedListNode* mNodeEntry = nullptr;
        TDoubleLinkedList<TSharedPtr<KKTweenItem>>::TDoubleLinkedListNode* GetNodeEntry()
        {
            if (this->mNodeEntry == nullptr)
            {
                this->mNodeEntry = new TDoubleLinkedList<TSharedPtr<KKTweenItem>>::TDoubleLinkedListNode(this->GetTSharedPtr());
            }
            return this->mNodeEntry;
        }
#endif

        ~KKTweenItem()
        {
            UE_LOG(LogTemp, Log, TEXT("~KKTweenItem Destroy"));

#ifdef USE_LinkedList
            if (mNodeEntry)
            {
                delete mNodeEntry;
                mNodeEntry = nullptr;
            }
#endif
        }
    private:
        void OnPoolPop()
        {
            
        }

        void OnPoolRecycle()
        {
            Reset();
            IncrementVersion();
        }

        void OnPoolDestory()
        {
#ifdef USE_LinkedList
            if (mNodeEntry)
            {
                delete mNodeEntry;
                mNodeEntry = nullptr;
            }
#endif
        }

        KKTweenItem()
        {
            Reset();
            IncrementVersion();
        }

        void IncrementVersion()
        {
            nVersion++;
            if (nVersion == 0)
            {
                nVersion++;
            }
        }

        void Reset()
        {
            bindObj = nullptr;
            toggle = false;

            delay = 0.0;
            time = 0.0;
            sumTime = 0.0;
            updateFunc.Reset();
            finishFunc.Reset();
            startFunc.Reset();
            nLoopCount = 0;
            nLoopPingTong = 0;
        }

        static TSharedPtr<KKTweenItem> Create()
        {
            return MakeShareable(new KKTweenItem());
        }
    public:
        TSharedPtr<KKTweenItem> GetTSharedPtr()
        {
            return AsShared();
        }

        TSharedPtr<KKTweenItem> cancel()
        {
            toggle = false;
            return GetTSharedPtr();
        }

        TSharedPtr<KKTweenItem> SetDelay(float fTime)
        {
            this->delay = fTime;
            return GetTSharedPtr();
        }

        TSharedPtr<KKTweenItem> SetLoop(int nCount = -1)
        {
            this->nLoopCount = nCount;
            return GetTSharedPtr();
        }

        TSharedPtr<KKTweenItem> SetLoopPingPong(int nCount = -1)
        {
            this->nLoopCount = nCount;
            nLoopPingTong = 1;
            return GetTSharedPtr();
        }

        void AppendTween(KKTweenItem* mItem)
        {
            float mTweenSumTime = this->delay + this->sumTime;
            mItem->delay += mTweenSumTime;
        }

        TSharedPtr<KKTweenItem> SetOnUpdateFunc(Action_Float_Delegate func)
        {
            this->updateFunc = func;
            return GetTSharedPtr();
        }

        TSharedPtr<KKTweenItem> SetOnStartFunc(ActionDelegate func)
        {
            this->startFunc = func;
            return GetTSharedPtr();
        }

        TSharedPtr<KKTweenItem> SetOnCompleteFunc(ActionDelegate func)
        {
            this->finishFunc = func;
            return GetTSharedPtr();
        }
    };

    class ObjectPool
    {
    private:
        TArray<TSharedPtr<KKTweenItem>> mObjectPool;
        int nMaxCapacity = 0;
    public:
        ObjectPool()
        {
            SetMaxCapacity(DEFAULT_TWEEN_MAX_COUNT);
        }

        void SetMaxCapacity(int nCapacity = 1)
        {
            this->nMaxCapacity = nCapacity;
        }

        TSharedPtr<KKTweenItem> Pop()
        {
            if (mObjectPool.Num() > 0)
            {
                auto mItem = mObjectPool.Pop();
                mItem->OnPoolPop();
                return mItem;
            }
            else
            {
                return KKTweenItem::Create();
            }
        }

        void recycle(TSharedPtr<KKTweenItem> t)
        {
            if (mObjectPool.Num() >= nMaxCapacity)
            {
                t->OnPoolDestory();
                t.Reset();
            }
            else
            {
                t->OnPoolRecycle();
                mObjectPool.Add(t);
            }
        }
    };

#ifdef USE_LinkedList
    class KKTweenByLinkedList
    {
    private:
        ObjectPool mItemPool;
        TDoubleLinkedList<TSharedPtr<KKTweenItem>> mTweenT;
        AKKTweenMgr* defaultBindObj;
    public:
        KKTweenByLinkedList(AKKTweenMgr* mDefaultBindObj);

        void Update(float DeltaTime);
        void SetMaxTweenCount(int nCount);
        void Cancel(UObject* obj);
        void CancelAll();
        TSharedPtr<KKTweenAPI::KKTweenItem> AddTween(
            UObject* obj,
            float time,
            Action_Float_Delegate updateFunc = nullptr,
            ActionDelegate finishFunc = nullptr);

        TDoubleLinkedList<TSharedPtr<KKTweenItem>>::TDoubleLinkedListNode* DoRemove(
            TSharedPtr<KKTweenItem> mItem);

    };

#else
    class KKTweenByList
    {
    private:
        ObjectPool mItemPool;
        TArray<TSharedPtr<KKTweenItem>> mTweenT;
        AKKTweenMgr* defaultBindObj;
    public:
        KKTweenByList(AKKTweenMgr* mDefaultBindObj);

        void Update(float DeltaTime);
        void SetMaxTweenCount(int nCount);
        void Cancel(UObject* obj);
        void CancelAll();
        TSharedPtr<KKTweenAPI::KKTweenItem> AddTween(
            UObject* obj,
            float time,
            Action_Float_Delegate updateFunc = nullptr,
            ActionDelegate finishFunc = nullptr);
    };
#endif

};


UCLASS()
class UE5_SOLITAIRE_API AKKTweenMgr : public AKKActorSingleton
{
    GENERATED_BODY()

protected:
    AKKTweenMgr();
    virtual void BeginPlay() override;
    virtual void EndPlay(EEndPlayReason::Type Reason) override;
public:
    virtual void Tick(float DeltaTime) override;
public:
    static AKKTweenMgr* GetSingleton(bool bCreate = true)
    {
        return AKKActorSingleton::GetActorSingleton<AKKTweenMgr>(bCreate);
    }

private:

#ifdef USE_LinkedList
    KKTweenAPI::KKTweenByLinkedList* mManager;
#else
    KKTweenAPI::KKTweenByList* mManager;
#endif

public:
    void Update(float DeltaTime);
    void SetMaxTweenCount(int nCount);
    void Cancel(UObject* obj);
    void CancelAll();
    TSharedPtr<KKTweenAPI::KKTweenItem> AddTween(UObject* obj, float time, KKTweenAPI::Action_Float_Delegate updateFunc = nullptr, KKTweenAPI::ActionDelegate finishFunc = nullptr);
public:
    //DECLARE_MULTICAST_DELEGATE(ActionDelegate);
    //DECLARE_MULTICAST_DELEGATE_OneParam(Action_Float_Delegate, float);
    //TFunction<void()> ActionDelegate; //UE5 提供的方法指针
    //using Action_Float_Delegate = void(*)(float); //C++ 定义方法指针
    //TFunction<void(float)> Action_Float_Delegate; //UE5 提供的方法指针
    //typedef void (*ActionDelegate)(); //C语言定义方法指针
    //typedef void (*Action_Float_Delegate)(float); //C语言定义方法指针
};
