#include "KTweenDemoRunner.h"
#include "KTweenHead.h"
#include "Engine/World.h"
#include "Engine/StaticMeshActor.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

int32 AKTweenDemoRunner::ColorIndex = 0;

FColor AKTweenDemoRunner::GetNextColor()
{
    static TArray<FColor> Colors = {
        FColor::Red, FColor::Green, FColor::Blue, FColor::Yellow,
        FColor::Cyan, FColor::Magenta, FColor::Orange, FColor::Purple,
        FColor::Emerald, FColor::Turquoise, FColor(255, 165, 0, 255),
    };
    return Colors[ColorIndex++ % Colors.Num()];
}

AKTweenDemoRunner::AKTweenDemoRunner()
{
    PrimaryActorTick.bCanEverTick = false;

    static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeFinder(TEXT("/Engine/BasicShapes/Cube.Cube"));
    static ConstructorHelpers::FObjectFinder<UStaticMesh> SphereFinder(TEXT("/Engine/BasicShapes/Sphere.Sphere"));
    static ConstructorHelpers::FObjectFinder<UStaticMesh> CylinderFinder(TEXT("/Engine/BasicShapes/Cylinder.Cylinder"));
    MeshCube = CubeFinder.Object;
    MeshSphere = SphereFinder.Object;
    MeshCylinder = CylinderFinder.Object;
}

void AKTweenDemoRunner::BeginPlay()
{
    Super::BeginPlay();
    if (bAutoRun)
    {
        RunAllDemos();
    }
}

AActor* AKTweenDemoRunner::SpawnShape(FVector Location, FColor Color, int32 ShapeIdx, float Scale)
{
    FActorSpawnParameters SpawnParams;
    SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
    AStaticMeshActor* Actor = GetWorld()->SpawnActor<AStaticMeshActor>(Location, FRotator::ZeroRotator, SpawnParams);

    UStaticMesh* Meshes[] = { MeshCube, MeshSphere, MeshCylinder };
    UStaticMesh* Mesh = Meshes[ShapeIdx % 3];
    if (!Mesh) Mesh = MeshCube;

    Actor->SetActorScale3D(FVector(Scale));
    UStaticMeshComponent* MeshComp = Actor->GetStaticMeshComponent();
    MeshComp->SetStaticMesh(Mesh);
    MeshComp->SetMobility(EComponentMobility::Movable);

    UMaterialInstanceDynamic* Mat = MeshComp->CreateAndSetMaterialInstanceDynamic(0);
    if (Mat)
    {
        Mat->SetVectorParameterValue(TEXT("BaseColor"), FLinearColor(Color));
    }
    MeshComp->SetCollisionEnabled(ECollisionEnabled::NoCollision);
    return Actor;
}

void AKTweenDemoRunner::ClearAllSpawned()
{
    for (auto* A : mSpawnedActors)
    {
        if (A && A->IsValidLowLevel())
            A->Destroy();
    }
    mSpawnedActors.Empty();
    ColorIndex = 0;
}

TArray<AActor*> AKTweenDemoRunner::SpawnGridBatch(int32 CountX, int32 CountY, float Spacing, FVector Origin)
{
    TArray<AActor*> Batch;
    for (int32 Y = 0; Y < CountY; Y++)
    {
        for (int32 X = 0; X < CountX; X++)
        {
            FVector Loc = Origin + FVector(X * Spacing - (CountX - 1) * Spacing * 0.5f,
                                            Y * Spacing - (CountY - 1) * Spacing * 0.5f,
                                            0);
            AActor* A = SpawnShape(Loc, GetNextColor(), X + Y, 0.4f);
            Batch.Add(A);
        }
    }
    mSpawnedActors.Append(Batch);
    return Batch;
}

// ──────────────────────────────────────────────────────────────────────
// 1. Basic Move — objects bounce up and down
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_BasicMove(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(0, 0, 400);
        int32 Idx = i;
        auto EaseFunc = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeOutQuad);
        KTween::AddTween(Targets[i], Duration,
            [Targets, Idx, StartLoc, EndLoc, EaseFunc](float T)
            {
                Targets[Idx]->SetActorLocation(EaseFunc(StartLoc, EndLoc, T));
            }
        )->SetLoopPingPong(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 2. Scale Pulse — 使用新 Apply() 接口
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_ScalePulse(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        int32 Idx = i;
        KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx](float T)
            {
                FVector S = KTween::EaseFunc::Apply<FVector>(FVector(0.4f), FVector(1.5f), T, KTween::EaseType::easeInOutSine);
                Targets[Idx]->SetActorScale3D(S);
            }
        )->SetLoopPingPong(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 3. Rotation
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_Rotation(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        int32 Idx = i;
        auto EaseFunc = KTween::GetEaseFunc<float>(KTween::EaseType::linear);
        KTween::AddTween(Targets[i], Duration,
            [Targets, Idx, EaseFunc](float T)
            {
                Targets[Idx]->SetActorRotation(FRotator(0, EaseFunc(0, 360, T), 0));
            }
        )->SetLoop(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 4. Delay & Chain — sequential movement
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_DelayAndChain(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector MidLoc = StartLoc + FVector(0, 0, 300);
        FVector EndLoc = StartLoc + FVector(400, 0, 0);
        int32 Idx = i;

        auto EaseOut = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeOutQuad);
        auto EaseIn = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeInQuad);

        auto T1 = KTween::AddTween(Targets[i], Duration * 0.4f,
            [Targets, Idx, StartLoc, MidLoc, EaseOut](float T)
            {
                Targets[Idx]->SetActorLocation(EaseOut(StartLoc, MidLoc, T));
            });

        auto T2 = KTween::AddTween(Targets[i], Duration * 0.6f,
            [Targets, Idx, MidLoc, EndLoc, EaseIn](float T)
            {
                Targets[Idx]->SetActorLocation(EaseIn(MidLoc, EndLoc, T));
            });

        T1->AppendTween(T2.Get());
    }
}

// ──────────────────────────────────────────────────────────────────────
// 5. Loop — half finite, half infinite
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_Loop(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(300, 0, 0);
        int32 Idx = i;
        auto EaseFunc = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeInOutCubic);
        KTween::AddTween(Targets[i], Duration * 0.3f,
            [Targets, Idx, StartLoc, EndLoc, EaseFunc](float T)
            {
                Targets[Idx]->SetActorLocation(EaseFunc(StartLoc, EndLoc, T));
            }
        )->SetLoop(i < Targets.Num() / 2 ? 5 : -1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 6. PingPong
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_PingPong(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(0, 300 * ((i % 2 == 0) ? 1 : -1), 0);
        int32 Idx = i;
        auto EaseFunc = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeInOutSine);
        KTween::AddTween(Targets[i], Duration * 0.3f,
            [Targets, Idx, StartLoc, EndLoc, EaseFunc](float T)
            {
                Targets[Idx]->SetActorLocation(EaseFunc(StartLoc, EndLoc, T));
            }
        )->SetLoopPingPong(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 7. Ease Type Comparison — 全部 30+ 缓动类型测试
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_EaseComparison(const TArray<AActor*>& Targets)
{
    TArray<KTween::EaseType> EaseTypes = {
        KTween::EaseType::linear,
        KTween::EaseType::easeInQuad,    KTween::EaseType::easeOutQuad,    KTween::EaseType::easeInOutQuad,
        KTween::EaseType::easeInCubic,   KTween::EaseType::easeOutCubic,   KTween::EaseType::easeInOutCubic,
        KTween::EaseType::easeInQuart,   KTween::EaseType::easeOutQuart,   KTween::EaseType::easeInOutQuart,
        KTween::EaseType::easeInQuint,   KTween::EaseType::easeOutQuint,   KTween::EaseType::easeInOutQuint,
        KTween::EaseType::easeInSine,    KTween::EaseType::easeOutSine,    KTween::EaseType::easeInOutSine,
        KTween::EaseType::easeInExpo,    KTween::EaseType::easeOutExpo,    KTween::EaseType::easeInOutExpo,
        KTween::EaseType::easeInCirc,    KTween::EaseType::easeOutCirc,    KTween::EaseType::easeInOutCirc,
        KTween::EaseType::easeInBounce,  KTween::EaseType::easeOutBounce,  KTween::EaseType::easeInOutBounce,
        KTween::EaseType::easeInBack,    KTween::EaseType::easeOutBack,    KTween::EaseType::easeInOutBack,
        KTween::EaseType::easeInElastic, KTween::EaseType::easeOutElastic, KTween::EaseType::easeInOutElastic,
        KTween::EaseType::easeSpring,    KTween::EaseType::easeShake,      KTween::EaseType::punch,
    };
    int32 Count = FMath::Min(Targets.Num(), EaseTypes.Num());

    for (int32 i = 0; i < Count; i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(500, 0, 0);
        int32 Idx = i;
        KTween::EaseType Type = EaseTypes[i];
        KTween::AddTween(Targets[i], Duration * 1.5f,
            [Targets, Idx, StartLoc, EndLoc, Type](float T)
            {
                FVector Pos = KTween::EaseFunc::Apply<FVector>(StartLoc, EndLoc, T, Type);
                Targets[Idx]->SetActorLocation(Pos);
            }
        )->SetLoopPingPong(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 8. Combined — move + scale + rotate simultaneously
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_MultiTweenCombined(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(300, 300 * ((i % 2 == 0) ? 1 : -1), 200);
        float Offset = i * 0.12f;
        int32 Idx = i;

        auto EaseMove = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeInOutBack);
        auto EaseScale = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeInOutSine);
        auto EaseRot = KTween::GetEaseFunc<float>(KTween::EaseType::easeInOutQuad);

        KTween::AddTween(Targets[i], Duration,
            [Targets, Idx, StartLoc, EndLoc, EaseMove](float T)
            {
                Targets[Idx]->SetActorLocation(EaseMove(StartLoc, EndLoc, T));
            }
        )->SetDelay(Offset)->SetLoopPingPong(-1);

        KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, EaseScale](float T)
            {
                Targets[Idx]->SetActorScale3D(EaseScale(FVector(0.4f), FVector(1.2f), T));
            }
        )->SetDelay(Offset)->SetLoopPingPong(-1);

        KTween::AddTween(Targets[i], Duration * 0.8f,
            [Targets, Idx, EaseRot](float T)
            {
                float Angle = EaseRot(0, 360, T);
                Targets[Idx]->SetActorRotation(FRotator(Angle * 0.5f, Angle, 0));
            }
        )->SetDelay(Offset)->SetLoop(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 9. Cancel & Handle
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_CancelHandle(const TArray<AActor*>& Targets)
{
    TArray<KTween::Handle> Handles;

    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(0, 0, 500);
        int32 Idx = i;
        auto EaseFunc = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeInOutBounce);
        auto Item = KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, StartLoc, EndLoc, EaseFunc](float T)
            {
                Targets[Idx]->SetActorLocation(EaseFunc(StartLoc, EndLoc, T));
            }
        )->SetLoopPingPong(-1);

        Handles.Add(KTween::GetHandle(Item));
    }

    // Cancel all after 3 seconds
    KTween::delayedCall(nullptr, 3.0f, [Handles]() mutable
    {
        for (auto& H : Handles)
            H.Cancel();
    });
}

// ──────────────────────────────────────────────────────────────────────
// 10. 100+ Objects — wave motion + orbit
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_100Objects(const TArray<AActor*>& Targets)
{
    int32 Half = FMath::Max(1, Targets.Num() / 2);

    // First half: sine wave bounce with phase offsets
    for (int32 i = 0; i < Half; i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        float RandomHeight = 200 + (i % 7) * 50.0f;
        FVector EndLoc = StartLoc + FVector(0, 0, RandomHeight);
        float Offset = (i % 10) * 0.12f;
        int32 Idx = i;
        auto EaseFunc = KTween::GetEaseFunc<FVector>(KTween::EaseType::easeInOutSine);
        KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, StartLoc, EndLoc, EaseFunc](float T)
            {
                Targets[Idx]->SetActorLocation(EaseFunc(StartLoc, EndLoc, T));
            }
        )->SetDelay(Offset)->SetLoopPingPong(-1);
    }

    // Second half: orbit around center + vertical oscillation
    for (int32 i = Half; i < Targets.Num(); i++)
    {
        int32 Idx = i;
        int32 LocalIdx = i - Half;
        FVector Center = Targets[i]->GetActorLocation();
        float Radius = 120 + (LocalIdx % 5) * 40.0f;
        float Speed = 0.5f + (LocalIdx % 7) * 0.1f;
        float Phase = LocalIdx * 0.5f;

        KTween::AddTween(Targets[i], Speed,
            [Targets, Idx, Center, Radius, Phase](float T)
            {
                float Angle = Phase + T * 360.0f;
                float Rad = FMath::DegreesToRadians(Angle);
                FVector NewPos = Center + FVector(FMath::Cos(Rad) * Radius,
                                                     FMath::Sin(Rad) * Radius,
                                                     FMath::Sin(Rad * 2) * 100);
                Targets[Idx]->SetActorLocation(NewPos);
            }
        )->SetLoop(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// Run All Demos
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::RunAllDemos()
{
    UWorld* World = GetWorld();
    if (!World) return;

    ClearAllSpawned();

    // 1. Basic Move (2x5 = 10)
    Demo_BasicMove(SpawnGridBatch(2, 5, 130, FVector(-800, -600, 0)));

    // 2. Scale Pulse (3x4 = 12)
    Demo_ScalePulse(SpawnGridBatch(3, 4, 130, FVector(-300, -600, 0)));

    // 3. Rotation (2x5 = 10)
    Demo_Rotation(SpawnGridBatch(2, 5, 130, FVector(200, -600, 0)));

    // 4. Delay & Chain (1x6 = 6)
    Demo_DelayAndChain(SpawnGridBatch(1, 6, 140, FVector(700, -600, 0)));

    // 5. Loop (2x5 = 10)
    Demo_Loop(SpawnGridBatch(2, 5, 130, FVector(1200, -600, 0)));

    // 6. PingPong (2x4 = 8)
    Demo_PingPong(SpawnGridBatch(2, 4, 150, FVector(-600, 0, 0)));

    // 7. Ease Comparison (1x10 = 10)
    Demo_EaseComparison(SpawnGridBatch(1, 10, 110, FVector(-400, 0, 0)));

    // 8. Combined (3x4 = 12)
    Demo_MultiTweenCombined(SpawnGridBatch(3, 4, 150, FVector(200, 0, 0)));

    // 9. Cancel & Handle (2x3 = 6)
    Demo_CancelHandle(SpawnGridBatch(2, 3, 140, FVector(800, 0, 0)));

    // 10. 100+ Objects Wave (10x15 = 150)
    Demo_100Objects(SpawnGridBatch(10, 15, 100, FVector(200, 700, 0)));

    UE_LOG(LogTemp, Warning, TEXT("KTween Demo: Spawned %d objects across 10 demos! Play in editor to see animations."),
           mSpawnedActors.Num());
}
