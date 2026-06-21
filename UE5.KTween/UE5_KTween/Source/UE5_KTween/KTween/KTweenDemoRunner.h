// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "KTween/KTween.h"
#include "KTweenDemoRunner.generated.h"

UCLASS()
class UE5_KTWEEN_API AKTweenDemoRunner : public AActor
{
    GENERATED_BODY()

public:
    AKTweenDemoRunner();

    virtual void BeginPlay() override;

    UFUNCTION(BlueprintCallable, CallInEditor, Category="KTween Demo")
    void RunAllDemos();

protected:
    void ClearAllSpawned();
    AActor* SpawnShape(FVector Location, FColor Color, int32 ShapeIdx, float Scale = 1.0f);
    TArray<AActor*> SpawnGridBatch(int32 CountX, int32 CountY, float Spacing, FVector Origin);

    // Demo sections
    void Demo_BasicMove(const TArray<AActor*>& Targets);
    void Demo_ScalePulse(const TArray<AActor*>& Targets);
    void Demo_Rotation(const TArray<AActor*>& Targets);
    void Demo_DelayAndChain(const TArray<AActor*>& Targets);
    void Demo_Loop(const TArray<AActor*>& Targets);
    void Demo_PingPong(const TArray<AActor*>& Targets);
    void Demo_EaseComparison(const TArray<AActor*>& Targets);
    void Demo_MultiTweenCombined(const TArray<AActor*>& Targets);
    void Demo_CancelHandle(const TArray<AActor*>& Targets);
    void Demo_100Objects(const TArray<AActor*>& Targets);

    UPROPERTY()
    TArray<AActor*> mSpawnedActors;

    UPROPERTY(EditAnywhere, Category="KTween Demo")
    bool bAutoRun = true;

    UPROPERTY(EditAnywhere, Category="KTween Demo")
    float Duration = 2.0f;

private:
    UStaticMesh* MeshCube;
    UStaticMesh* MeshSphere;
    UStaticMesh* MeshCylinder;
    static int32 ColorIndex;
    static FColor GetNextColor();
};
