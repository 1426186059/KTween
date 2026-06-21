#pragma once

#define DEFAULT_TWEEN_MAX_COUNT 1500

#include "CoreMinimal.h"
#include "Engine/Engine.h"
#include "Engine/World.h"
#include "GameFramework/Actor.h"
#include "KTweenHead.generated.h"

class KTween;
namespace KTweenAPI
{
    typedef TFunction<void(void)> ActionDelegate;
    typedef TFunction<void(float)> Action_Float_Delegate;

    enum EaseType
    {
        linear, easeOutQuad, easeInQuad, easeInOutQuad, easeInCubic, easeOutCubic, easeInOutCubic,
        easeInQuart, easeOutQuart, easeInOutQuart,
        easeInQuint, easeOutQuint, easeInOutQuint, easeInSine, easeOutSine, easeInOutSine,
        easeInExpo, easeOutExpo, easeInOutExpo, easeInCirc, easeOutCirc, easeInOutCirc,
        easeInBounce, easeOutBounce, easeInOutBounce, easeInBack, easeOutBack, easeInOutBack,
        easeInElastic, easeOutElastic, easeInOutElastic,
        easeSpring, easeShake, punch, once, clamp, pingPong, animationCurve
    };

    class KTweenItem;
    class ObjectPool;

    class KTweenItem:public TSharedFromThis<KTweenItem>
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
        EaseType nEaseType;
        Action_Float_Delegate updateFunc;
        ActionDelegate finishFunc;
        ActionDelegate startFunc;

        FVector From;
        FVector To;

        TDoubleLinkedList<TSharedPtr<KTweenItem>>::TDoubleLinkedListNode* mNodeEntry = nullptr;
        TDoubleLinkedList<TSharedPtr<KTweenItem>>::TDoubleLinkedListNode* GetNodeEntry()
        {
            if (this->mNodeEntry == nullptr)
            {
                this->mNodeEntry = new TDoubleLinkedList<TSharedPtr<KTweenItem>>::TDoubleLinkedListNode(this->GetTSharedPtr());
            }
            return this->mNodeEntry;
        }

        ~KTweenItem()
        {
            UE_LOG(LogTemp, Log, TEXT("~KTweenItem Destroy"));

            if (mNodeEntry)
            {
                delete mNodeEntry;
                mNodeEntry = nullptr;
            }
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
            if (mNodeEntry)
            {
                delete mNodeEntry;
                mNodeEntry = nullptr;
            }
        }

        KTweenItem()
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
            nEaseType = KTweenAPI::linear;
        }

        static TSharedPtr<KTweenItem> Create()
        {
            return MakeShareable(new KTweenItem());
        }
    public:
        TSharedPtr<KTweenItem> GetTSharedPtr()
        {
            return AsShared();
        }

        TSharedPtr<KTweenItem> cancel()
        {
            toggle = false;
            return GetTSharedPtr();
        }

        TSharedPtr<KTweenItem> SetDelay(float fTime)
        {
            this->delay = fTime;
            return GetTSharedPtr();
        }

        TSharedPtr<KTweenItem> SetLoop(int nCount = -1)
        {
            this->nLoopCount = nCount;
            return GetTSharedPtr();
        }

        TSharedPtr<KTweenItem> SetLoopPingPong(int nCount = -1)
        {
            this->nLoopCount = nCount;
            nLoopPingTong = 1;
            return GetTSharedPtr();
        }

        TSharedPtr<KTweenItem> SetEase(EaseType EaseType)
        {
            this->nEaseType = EaseType;
            return GetTSharedPtr();
        }

        void AppendTween(KTweenItem* mItem)
        {
            float mTweenSumTime = this->delay + this->sumTime;
            mItem->delay += mTweenSumTime;
        }

        TSharedPtr<KTweenItem> SetOnUpdateFunc(Action_Float_Delegate func)
        {
            this->updateFunc = func;
            return GetTSharedPtr();
        }

        TSharedPtr<KTweenItem> SetOnStartFunc(ActionDelegate func)
        {
            this->startFunc = func;
            return GetTSharedPtr();
        }

        TSharedPtr<KTweenItem> SetOnCompleteFunc(ActionDelegate func)
        {
            this->finishFunc = func;
            return GetTSharedPtr();
        }
    };

    class ObjectPool
    {
    private:
        TArray<TSharedPtr<KTweenItem>> mObjectPool;
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

        TSharedPtr<KTweenItem> Pop()
        {
            if (mObjectPool.Num() > 0)
            {
                auto mItem = mObjectPool.Pop();
                mItem->OnPoolPop();
                return mItem;
            }
            else
            {
                return KTweenItem::Create();
            }
        }

        void recycle(TSharedPtr<KTweenItem> t)
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

    class KTweenByLinkedList
    {
    private:
        ObjectPool mItemPool;
        TDoubleLinkedList<TSharedPtr<KTweenItem>> mTweenT;
        AKTweenMgr* defaultBindObj;
    public:
        KTweenByLinkedList(AKTweenMgr* mDefaultBindObj);

        void Update(float DeltaTime);
        void SetMaxTweenCount(int nCount);
        void Cancel(UObject* obj);
        void CancelAll();
        TSharedPtr<KTweenAPI::KTweenItem> AddTween(
            UObject* obj,
            float time,
            Action_Float_Delegate updateFunc = nullptr,
            ActionDelegate finishFunc = nullptr);

        TDoubleLinkedList<TSharedPtr<KTweenItem>>::TDoubleLinkedListNode* DoRemove(
            TSharedPtr<KTweenItem> mItem);

    };

};


UCLASS()
class KTWEEN_API AKTweenMgr : public AActor
{
    GENERATED_BODY()

protected:
    AKTweenMgr();
    virtual void BeginPlay() override;
    virtual void EndPlay(EEndPlayReason::Type Reason) override;
public:
    virtual void Tick(float DeltaTime) override;
    void EnsureManager();
public:
    static AKTweenMgr* GetSingleton(bool bCreate = true)
    {
        static AKTweenMgr* Instance = nullptr;

        // 如果 Instance 是野指针（PIE 结束后 Actor 被销毁），重置
        if (Instance != nullptr && !IsValid(Instance))
        {
            Instance = nullptr;
        }

        if (Instance == nullptr && bCreate)
        {
            UWorld* World = nullptr;
            if (GEngine)
            {
                for (const FWorldContext& Context : GEngine->GetWorldContexts())
                {
                    World = Context.World();
                    if (World && World->IsGameWorld()) break;
                }
            }
            if (World)
            {
                FActorSpawnParameters SpawnParams;
                SpawnParams.Name = TEXT("KTween~");
                SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
                Instance = Cast<AKTweenMgr>(World->SpawnActor(AKTweenMgr::StaticClass(), &FVector::ZeroVector, &FRotator::ZeroRotator, SpawnParams));
            }
        }
        return Instance;
    }

private:
    KTweenAPI::KTweenByLinkedList* mManager;

public:
    void Update(float DeltaTime);
    void SetMaxTweenCount(int nCount);
    void Cancel(UObject* obj);
    void CancelAll();
    TSharedPtr<KTweenAPI::KTweenItem> AddTween(UObject* obj, float time, KTweenAPI::Action_Float_Delegate updateFunc = nullptr, KTweenAPI::ActionDelegate finishFunc = nullptr);


};
