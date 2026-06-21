#include "KKTweenHead.h"
using namespace KKTweenAPI;

AKKTweenMgr::AKKTweenMgr()
{
    PrimaryActorTick.bCanEverTick = true;
#ifdef USE_LinkedList
    this->mManager = new KKTweenAPI::KKTweenByLinkedList(this);
#else
    this->mManager = new KKTweenAPI::KKTweenByList(this);
#endif
}

void AKKTweenMgr::BeginPlay()
{
    Super::BeginPlay();
}

void AKKTweenMgr::EndPlay(EEndPlayReason::Type Reason)
{
    Super::EndPlay(Reason);
}

void AKKTweenMgr::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
    this->Update(DeltaTime);
}

void AKKTweenMgr::Update(float DeltaTime)
{
    this->mManager->Update(DeltaTime);
}

void AKKTweenMgr::SetMaxTweenCount(int nCount)
{
    this->mManager->SetMaxTweenCount(nCount);
}

TSharedPtr<KKTweenAPI::KKTweenItem> AKKTweenMgr::AddTween(UObject* obj, float time, KKTweenAPI::Action_Float_Delegate updateFunc, KKTweenAPI::ActionDelegate finishFunc)
{
    if (obj == nullptr)
    {
        obj = this;
    }

    return this->mManager->AddTween(obj, time, updateFunc, finishFunc);
}

void AKKTweenMgr::Cancel(UObject* obj)
{
    return this->mManager->Cancel(obj);
}


