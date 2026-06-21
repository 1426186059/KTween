#include "KTween/KTweenHead.h"
using namespace KTweenAPI;

AKTweenMgr::AKTweenMgr()
{
    PrimaryActorTick.bCanEverTick = true;
    mManager = nullptr;
}

void AKTweenMgr::EnsureManager()
{
    if (mManager == nullptr)
    {
#ifdef USE_LinkedList
        this->mManager = new KTweenAPI::KTweenByLinkedList(this);
#else
        this->mManager = new KTweenAPI::KTweenByList(this);
#endif
    }
}

void AKTweenMgr::BeginPlay()
{
    Super::BeginPlay();
    EnsureManager();
}

void AKTweenMgr::EndPlay(EEndPlayReason::Type Reason)
{
    // mManager is a raw C++ heap object with shared_ptr cycles
    // between TDoubleLinkedList nodes and KTweenItem.
    // Let the process clean-up handle it to avoid double-free.
    mManager = nullptr;
    Super::EndPlay(Reason);
}

void AKTweenMgr::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
    this->Update(DeltaTime);
}

void AKTweenMgr::Update(float DeltaTime)
{
    EnsureManager();
    this->mManager->Update(DeltaTime);
}

void AKTweenMgr::SetMaxTweenCount(int nCount)
{
    EnsureManager();
    this->mManager->SetMaxTweenCount(nCount);
}

TSharedPtr<KTweenAPI::KTweenItem> AKTweenMgr::AddTween(UObject* obj, float time, KTweenAPI::Action_Float_Delegate updateFunc, KTweenAPI::ActionDelegate finishFunc)
{
    EnsureManager();

    if (obj == nullptr)
    {
        obj = this;
    }

    return this->mManager->AddTween(obj, time, updateFunc, finishFunc);
}

void AKTweenMgr::Cancel(UObject* obj)
{
    EnsureManager();
    this->mManager->Cancel(obj);
}

void AKTweenMgr::CancelAll()
{
    EnsureManager();
    this->mManager->CancelAll();
}



