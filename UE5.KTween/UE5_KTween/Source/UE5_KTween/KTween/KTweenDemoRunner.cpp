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
// 1. Basic Move — 演示 SetEase 链式调用，跟 Unity 一样
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_BasicMove(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(0, 0, 400);
        int32 Idx = i;

        // T 已被 SetEase 自动缓动，直接 Lerp（与 Unity 用法一致）
        KTween::AddTween(Targets[i], Duration,
            [Targets, Idx, StartLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, EndLoc, T));
            }
        )->SetEase(KTweenType::easeOutQuad)->SetLoopPingPong(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 2. Scale Pulse — SetEase 自动缓动
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_ScalePulse(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        int32 Idx = i;
        KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx](float T)
            {
                Targets[Idx]->SetActorScale3D(FMath::Lerp(FVector(0.4f), FVector(1.5f), T));
            }
        )->SetEase(KTweenType::easeInOutSine)->SetLoopPingPong(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 3. Rotation — SetEase 自动缓动
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_Rotation(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        int32 Idx = i;
        KTween::AddTween(Targets[i], Duration,
            [Targets, Idx](float T)
            {
                Targets[Idx]->SetActorRotation(FRotator(0, 360.0f * T, 0));
            }
        )->SetEase(KTweenType::linear)->SetLoop(-1);
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

        auto T1 = KTween::AddTween(Targets[i], Duration * 0.4f,
            [Targets, Idx, StartLoc, MidLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, MidLoc, T));
            })->SetEase(KTweenType::easeOutQuad);

        auto T2 = KTween::AddTween(Targets[i], Duration * 0.6f,
            [Targets, Idx, MidLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(MidLoc, EndLoc, T));
            })->SetEase(KTweenType::easeInQuad);

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
        KTween::AddTween(Targets[i], Duration * 0.3f,
            [Targets, Idx, StartLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, EndLoc, T));
            }
        )->SetEase(KTweenType::easeInOutCubic)->SetLoop(i < Targets.Num() / 2 ? 5 : -1);
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
        KTween::AddTween(Targets[i], Duration * 0.3f,
            [Targets, Idx, StartLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, EndLoc, T));
            }
        )->SetEase(KTweenType::easeInOutSine)->SetLoopPingPong(-1);
    }
}

// ──────────────────────────────────────────────────────────────────────
// 7. Ease Type Comparison — 全部 30+ 缓动类型测试
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_EaseComparison(const TArray<AActor*>& Targets)
{
    TArray<KTweenType> EaseTypes = {
        KTweenType::linear,
        KTweenType::easeInQuad,    KTweenType::easeOutQuad,    KTweenType::easeInOutQuad,
        KTweenType::easeInCubic,   KTweenType::easeOutCubic,   KTweenType::easeInOutCubic,
        KTweenType::easeInQuart,   KTweenType::easeOutQuart,   KTweenType::easeInOutQuart,
        KTweenType::easeInQuint,   KTweenType::easeOutQuint,   KTweenType::easeInOutQuint,
        KTweenType::easeInSine,    KTweenType::easeOutSine,    KTweenType::easeInOutSine,
        KTweenType::easeInExpo,    KTweenType::easeOutExpo,    KTweenType::easeInOutExpo,
        KTweenType::easeInCirc,    KTweenType::easeOutCirc,    KTweenType::easeInOutCirc,
        KTweenType::easeInBounce,  KTweenType::easeOutBounce,  KTweenType::easeInOutBounce,
        KTweenType::easeInBack,    KTweenType::easeOutBack,    KTweenType::easeInOutBack,
        KTweenType::easeInElastic, KTweenType::easeOutElastic, KTweenType::easeInOutElastic,
        KTweenType::easeSpring,    KTweenType::easeShake,      KTweenType::punch,
    };
    int32 Count = FMath::Min(Targets.Num(), EaseTypes.Num());

    for (int32 i = 0; i < Count; i++)
    {
        FVector StartLoc = Targets[i]->GetActorLocation();
        FVector EndLoc = StartLoc + FVector(500, 0, 0);
        int32 Idx = i;
        KTween::AddTween(Targets[i], Duration * 1.5f,
            [Targets, Idx, StartLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, EndLoc, T));
            }
        )->SetEase(EaseTypes[i])->SetLoopPingPong(-1);
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

        KTween::AddTween(Targets[i], Duration,
            [Targets, Idx, StartLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, EndLoc, T));
            }
        )->SetEase(KTweenType::easeInOutBack)->SetDelay(Offset)->SetLoopPingPong(-1);

        KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx](float T)
            {
                Targets[Idx]->SetActorScale3D(FMath::Lerp(FVector(0.4f), FVector(1.2f), T));
            }
        )->SetEase(KTweenType::easeInOutSine)->SetDelay(Offset)->SetLoopPingPong(-1);

        KTween::AddTween(Targets[i], Duration * 0.8f,
            [Targets, Idx](float T)
            {
                float Angle = 360.0f * T;
                Targets[Idx]->SetActorRotation(FRotator(Angle * 0.5f, Angle, 0));
            }
        )->SetEase(KTweenType::easeInOutQuad)->SetDelay(Offset)->SetLoop(-1);
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
        auto Item = KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, StartLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, EndLoc, T));
            }
        )->SetEase(KTweenType::easeInOutBounce)->SetLoopPingPong(-1);

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
        KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, StartLoc, EndLoc](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(StartLoc, EndLoc, T));
            }
        )->SetEase(KTweenType::easeInOutSine)->SetDelay(Offset)->SetLoopPingPong(-1);
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
// 11. Handle 序列动画 — 用空 Handle 构建链式序列
// ──────────────────────────────────────────────────────────────────────
void AKTweenDemoRunner::Demo_HandleSequence(const TArray<AActor*>& Targets)
{
    for (int32 i = 0; i < Targets.Num(); i++)
    {
        FVector P0 = Targets[i]->GetActorLocation();
        FVector P1 = P0 + FVector(0, 0, 250);
        FVector P2 = P1 + FVector(0, 250, 0);
        FVector P3 = P2 + FVector(0, 0, -250);
        int32 Idx = i;

        // 用空 Handle 构建序列：上升 → 右移 → 下降
        KTween::Handle Seq;
        Seq.AppendTween(KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, P0, P1](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(P0, P1, T));
            })->SetEase(KTweenType::easeOutQuad));

        Seq.AppendTween(KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, P1, P2](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(P1, P2, T));
            })->SetEase(KTweenType::easeInOutSine));

        Seq.AppendTween(KTween::AddTween(Targets[i], Duration * 0.5f,
            [Targets, Idx, P2, P3](float T)
            {
                Targets[Idx]->SetActorLocation(FMath::Lerp(P2, P3, T));
            })->SetEase(KTweenType::easeInQuad));
    }
}

// ══════════════════════════════════════════════════════════════════════
// 12. Path Linear — move(path[]) 线性路径 (菱形 / 锯齿 / 圆形)
// ══════════════════════════════════════════════════════════════════════
void AKTweenDemoRunner::Demo_PathLinear(const TArray<AActor*>& Targets)
{
    if (Targets.Num() < 3) return;

    // Actor 0: Diamond path (5-point closed, easeInOutQuad, Loop)
    {
        FVector O = Targets[0]->GetActorLocation();
        TArray<FVector> diamondPath = {
            O + FVector(-150, 0, 0),
            O + FVector(0, 150, 0),
            O + FVector(150, 0, 0),
            O + FVector(0, -150, 0),
            O + FVector(-150, 0, 0),
        };
        KTweenEx::Move(Targets[0], diamondPath, Duration * 2.5f)
            ->SetEase(KTweenType::easeInOutQuad)->SetLoop(-1);
    }

    // Actor 1: Zigzag path (5-point, easeOutBounce, PingPong)
    {
        FVector O = Targets[1]->GetActorLocation();
        TArray<FVector> zigPath = {
            O + FVector(-150, -100, 0),
            O + FVector(-75,  100, 0),
            O + FVector(0,   -100, 0),
            O + FVector(75,   100, 0),
            O + FVector(150, -100, 0),
        };
        KTweenEx::Move(Targets[1], zigPath, Duration * 1.8f)
            ->SetEase(KTweenType::easeOutBounce)->SetLoopPingPong(-1);
    }

    // Actor 2: Octagon circle (9-point, linear, Loop)
    {
        FVector O = Targets[2]->GetActorLocation();
        TArray<FVector> cirPath;
        int32 n = 8;
        float r = 130.0f;
        for (int32 i = 0; i <= n; i++)
        {
            float a = UE_PI * 2.0f * i / n - UE_PI / 2.0f;
            cirPath.Add(O + FVector(FMath::Cos(a) * r, FMath::Sin(a) * r, 0));
        }
        KTweenEx::Move(Targets[2], cirPath, Duration * 2.5f)
            ->SetEase(KTweenType::linear)->SetLoop(-1);
    }
}

// ══════════════════════════════════════════════════════════════════════
// 13. Path Bezier — moveBezier(path[]) 贝塞尔曲线 (S / U / 花形)
// ══════════════════════════════════════════════════════════════════════
void AKTweenDemoRunner::Demo_PathBezier(const TArray<AActor*>& Targets)
{
    if (Targets.Num() < 3) return;

    // Actor 0: S-curve (7-point, 2 segments, PingPong)
    {
        FVector O = Targets[0]->GetActorLocation();
        TArray<FVector> sPath = {
            O + FVector(-150, -120, 0),  // P0
            O + FVector(-50,  -200, 0),  // C1
            O + FVector(0,    -30,  0),  // C2
            O + FVector(0,     0,   0),  // P1 (shared)
            O + FVector(0,     30,  0),  // C1
            O + FVector(50,    200, 0),  // C2
            O + FVector(150,   120, 0),  // P2
        };
        KTweenEx::MoveBezier(Targets[0], sPath, Duration * 2.2f)
            ->SetEase(KTweenType::linear)->SetLoopPingPong(-1);
    }

    // Actor 1: U-curve (4-point, 1 segment, PingPong)
    {
        FVector O = Targets[1]->GetActorLocation();
        TArray<FVector> uPath = {
            O + FVector(-100, -120, 0),  // P0
            O + FVector(-100,  120, 0),  // C1
            O + FVector( 100,  120, 0),  // C2
            O + FVector( 100, -120, 0),  // P1
        };
        KTweenEx::MoveBezier(Targets[1], uPath, Duration * 2.0f)
            ->SetEase(KTweenType::linear)->SetLoopPingPong(-1);
    }

    // Actor 2: Flower/ellipse bezier (7-point closed, 2 segments, Loop)
    {
        FVector O = Targets[2]->GetActorLocation();
        TArray<FVector> flowerPath = {
            O + FVector(-150, 0,   0),   // P0
            O + FVector(-150, 90,  0),   // C1
            O + FVector( 150, 90,  0),   // C2
            O + FVector( 150, 0,   0),   // P1
            O + FVector( 150, -90, 0),   // C1
            O + FVector(-150, -90, 0),   // C2
            O + FVector(-150, 0,   0),   // P2 (back to start)
        };
        KTweenEx::MoveBezier(Targets[2], flowerPath, Duration * 3.0f)
            ->SetEase(KTweenType::linear)->SetLoop(-1);
    }
}

// ══════════════════════════════════════════════════════════════════════
// 14. Path Local — moveLocal(path[]) / moveLocalBezier(path[]) + 旋转父节点
// ══════════════════════════════════════════════════════════════════════
void AKTweenDemoRunner::Demo_PathLocal()
{
    UWorld* World = GetWorld();
    if (!World) return;

    FActorSpawnParameters SpawnParams;
    SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;

    // ── Pair 1: 父节点绕 Yaw 旋转 + 子节点线性菱形路径 ──
    {
        AActor* Parent = World->SpawnActor<AActor>(FVector(-600, 1000, 0), FRotator::ZeroRotator, SpawnParams);
        mSpawnedActors.Add(Parent);

        AActor* Child = SpawnShape(FVector::ZeroVector, FColor::Green, 1, 0.4f); // Sphere
        Child->AttachToActor(Parent, FAttachmentTransformRules::KeepRelativeTransform);
        Child->SetActorRelativeLocation(FVector::ZeroVector);
        mSpawnedActors.Add(Child);

        // 父节点 Yaw 旋转
        KTweenEx::RotateAround(Parent, FVector(0, 0, 1), 360.0f, Duration * 3.5f)
            ->SetEase(KTweenType::linear)->SetLoop(-1);

        // 子节点本地菱形路径
        TArray<FVector> localDiamond = {
            FVector(-100, 0, 0),
            FVector(0, 100, 0),
            FVector(100, 0, 0),
            FVector(0, -100, 0),
            FVector(-100, 0, 0),
        };
        KTweenEx::MoveLocal(Child, localDiamond, Duration * 2.5f)
            ->SetEase(KTweenType::easeInOutQuad)->SetLoop(-1);
    }

    // ── Pair 2: 父节点反向旋转 + 子节点贝塞尔花形路径 ──
    {
        AActor* Parent = World->SpawnActor<AActor>(FVector(0, 1000, 0), FRotator::ZeroRotator, SpawnParams);
        mSpawnedActors.Add(Parent);

        AActor* Child = SpawnShape(FVector::ZeroVector, FColor::Red, 1, 0.4f); // Sphere
        Child->AttachToActor(Parent, FAttachmentTransformRules::KeepRelativeTransform);
        Child->SetActorRelativeLocation(FVector::ZeroVector);
        mSpawnedActors.Add(Child);

        // 父节点反向 Yaw 旋转
        KTweenEx::RotateAround(Parent, FVector(0, 0, 1), -360.0f, Duration * 4.0f)
            ->SetEase(KTweenType::linear)->SetLoop(-1);

        // 子节点本地贝塞尔椭圆路径
        TArray<FVector> localBez = {
            FVector(-120, 0,   0),
            FVector(-120, 70,  0),
            FVector( 120, 70,  0),
            FVector( 120, 0,   0),
            FVector( 120, -70, 0),
            FVector(-120, -70, 0),
            FVector(-120, 0,   0),
        };
        KTweenEx::MoveLocalBezier(Child, localBez, Duration * 3.2f)
            ->SetEase(KTweenType::linear)->SetLoop(-1);
    }
}

// ══════════════════════════════════════════════════════════════════════
// 15. Path Extra — Move / Scale / RotateAround 补充演示
// ══════════════════════════════════════════════════════════════════════
void AKTweenDemoRunner::Demo_PathExtra()
{
    UWorld* World = GetWorld();
    if (!World) return;

    FActorSpawnParameters SpawnParams;
    SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;

    // ── 1. Move to target + Scale + RotateAround 组合 ──
    {
        AActor* Act = SpawnShape(FVector(500, 1000, 0), FColor::Cyan, 0, 0.5f);
        mSpawnedActors.Add(Act);

        // 移动到目标点
        KTweenEx::Move(Act, FVector(500, 1300, 0), Duration * 1.5f)
            ->SetEase(KTweenType::easeInOutBack)->SetLoopPingPong(-1);

        // 同时缩放
        KTweenEx::Scale(Act, FVector(1.5f), Duration * 1.0f)
            ->SetEase(KTweenType::easeInOutSine)->SetLoopPingPong(-1);

        // 同时绕 Pitch 自转
        KTweenEx::RotateAround(Act, FVector(0, 1, 0), 360.0f, Duration * 2.0f)
            ->SetEase(KTweenType::linear)->SetLoop(-1);
    }

    // ── 2. Move zigzag + Scale pulse ──
    {
        AActor* Act = SpawnShape(FVector(900, 1000, 0), FColor::Orange, 0, 0.5f);
        mSpawnedActors.Add(Act);

        FVector O = Act->GetActorLocation();
        TArray<FVector> zigPath = {
            O + FVector(-80, -60, 0),
            O + FVector(0, 60, 0),
            O + FVector(80, -60, 0),
            O + FVector(0, 60, 0),
            O + FVector(-80, -60, 0),
        };
        KTweenEx::Move(Act, zigPath, Duration * 1.5f)
            ->SetEase(KTweenType::easeOutElastic)->SetLoop(-1);

        KTweenEx::Scale(Act, FVector(1.3f), Duration * 0.8f)
            ->SetEase(KTweenType::easeInOutSine)->SetLoopPingPong(-1);
    }

    // ── 3. MoveLocal to target + parent rotation ──
    {
        AActor* Parent = World->SpawnActor<AActor>(FVector(1300, 1000, 0), FRotator::ZeroRotator, SpawnParams);
        mSpawnedActors.Add(Parent);

        AActor* Child = SpawnShape(FVector::ZeroVector, FColor::Purple, 2, 0.4f); // Cylinder
        Child->AttachToActor(Parent, FAttachmentTransformRules::KeepRelativeTransform);
        Child->SetActorRelativeLocation(FVector::ZeroVector);
        mSpawnedActors.Add(Child);

        // 父节点旋转
        KTweenEx::RotateAround(Parent, FVector(0, 0, 1), 720.0f, Duration * 5.0f)
            ->SetEase(KTweenType::linear)->SetLoop(-1);

        // 子节点 local 往复移动
        KTweenEx::MoveLocal(Child, FVector(120, 0, 0), Duration * 1.2f)
            ->SetEase(KTweenType::easeInOutCubic)->SetLoopPingPong(-1);
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

    // 7. Ease Comparison (4x8 = 32) — 测试全部 32 种缓动类型
    Demo_EaseComparison(SpawnGridBatch(4, 8, 100, FVector(-300, 0, 0)));

    // 8. Combined (3x4 = 12)
    Demo_MultiTweenCombined(SpawnGridBatch(3, 4, 150, FVector(200, 0, 0)));

    // 9. Cancel & Handle (2x3 = 6)
    Demo_CancelHandle(SpawnGridBatch(2, 3, 140, FVector(800, 0, 0)));

    // 10. 100+ Objects Wave (10x15 = 150)
    Demo_100Objects(SpawnGridBatch(10, 15, 100, FVector(200, 700, 0)));

    // 11. Handle Sequence (1x6 = 6)
    Demo_HandleSequence(SpawnGridBatch(1, 6, 140, FVector(1200, 0, 0)));

    // 12. Path Linear — move(path[]) (1x3 = 3) 菱形/锯齿/圆形
    Demo_PathLinear(SpawnGridBatch(1, 3, 220, FVector(-600, 700, 0)));

    // 13. Path Bezier — moveBezier(path[]) (1x3 = 3) S/U/花形
    Demo_PathBezier(SpawnGridBatch(1, 3, 220, FVector(0, 700, 0)));

    // 14. Path Local — moveLocal(path[]) + 旋转父节点 (4 actors)
    Demo_PathLocal();

    // 15. Path Extra — Move/Scale/RotateAround 组合 (5 actors)
    Demo_PathExtra();

    UE_LOG(LogTemp, Warning, TEXT("KTween Demo: Spawned %d objects across 15 demos! Play in editor to see animations."),
           mSpawnedActors.Num());
}
