#pragma once

#include "CoreMinimal.h"
#include "Components/Widget.h"
#include "Components/CanvasPanelSlot.h"
#include "GameFramework/Actor.h"
#include "KTween.h"

class KTweenEx
{
public:
    // ════════════════════════════════════════════════════════════════════
    //  Actor 3D 路径移动方法
    // ════════════════════════════════════════════════════════════════════

    /// 世界空间移动到目标点
    static TSharedPtr<KTweenItem> Move(AActor* target, FVector to, float time)
    {
        FVector from = target->GetActorLocation();
        return KTween::AddTween(target, time,
            [target, from, to](float fPercent)
            {
                target->SetActorLocation(FMath::Lerp(from, to, fPercent));
            });
    }

    /// 世界空间线性路径 — path.Length >= 2，总时间均匀分配到每段
    static TSharedPtr<KTweenItem> Move(AActor* target, const TArray<FVector>& path, float time)
    {
        int32 segCount = path.Num() - 1;
        if (segCount <= 0) return nullptr;
        return KTween::AddTween(target, time,
            [target, path](float fPercent)
            {
                if (fPercent >= 1.0f) { target->SetActorLocation(path.Last()); return; }
                float t = fPercent * (path.Num() - 1);
                int32 idx = FMath::Min((int32)t, path.Num() - 2);
                float segT = t - idx;
                target->SetActorLocation(FMath::Lerp(path[idx], path[idx + 1], segT));
            });
    }

    /// 世界空间贝塞尔路径 — 每 4 个点为一段三次贝塞尔 (P0,C1,C2,P1)
    /// 总长必须满足 (path.Num()-1) % 3 == 0
    static TSharedPtr<KTweenItem> MoveBezier(AActor* target, const TArray<FVector>& path, float time)
    {
        int32 segCount = (path.Num() - 1) / 3;
        if (segCount <= 0 || (path.Num() - 1) % 3 != 0) return nullptr;
        return KTween::AddTween(target, time,
            [target, path](float fPercent)
            {
                if (fPercent >= 1.0f) { target->SetActorLocation(path.Last()); return; }
                int32 totalSeg = (path.Num() - 1) / 3;
                float t = fPercent * totalSeg;
                int32 segIdx = FMath::Min((int32)t, totalSeg - 1);
                float bt = t - segIdx;

                int32 i = segIdx * 3; // P0 下标
                float u = 1.0f - bt;
                FVector pos =
                    u * u * u * path[i] +
                    3.0f * u * u * bt * path[i + 1] +
                    3.0f * u * bt * bt * path[i + 2] +
                    bt * bt * bt * path[i + 3];
                target->SetActorLocation(pos);
            });
    }

    /// 本地空间移动到目标点
    static TSharedPtr<KTweenItem> MoveLocal(AActor* target, FVector to, float time)
    {
        FVector from = target->GetRootComponent()
            ? target->GetRootComponent()->GetRelativeLocation()
            : target->GetActorLocation();
        return KTween::AddTween(target, time,
            [target, from, to](float fPercent)
            {
                target->SetActorRelativeLocation(FMath::Lerp(from, to, fPercent));
            });
    }

    /// 本地空间线性路径移动
    static TSharedPtr<KTweenItem> MoveLocal(AActor* target, const TArray<FVector>& path, float time)
    {
        int32 segCount = path.Num() - 1;
        if (segCount <= 0) return nullptr;
        return KTween::AddTween(target, time,
            [target, path](float fPercent)
            {
                if (fPercent >= 1.0f) { target->SetActorRelativeLocation(path.Last()); return; }
                float t = fPercent * (path.Num() - 1);
                int32 idx = FMath::Min((int32)t, path.Num() - 2);
                float segT = t - idx;
                target->SetActorRelativeLocation(FMath::Lerp(path[idx], path[idx + 1], segT));
            });
    }

    /// 本地空间贝塞尔路径移动
    static TSharedPtr<KTweenItem> MoveLocalBezier(AActor* target, const TArray<FVector>& path, float time)
    {
        int32 segCount = (path.Num() - 1) / 3;
        if (segCount <= 0 || (path.Num() - 1) % 3 != 0) return nullptr;
        return KTween::AddTween(target, time,
            [target, path](float fPercent)
            {
                if (fPercent >= 1.0f) { target->SetActorRelativeLocation(path.Last()); return; }
                int32 totalSeg = (path.Num() - 1) / 3;
                float t = fPercent * totalSeg;
                int32 segIdx = FMath::Min((int32)t, totalSeg - 1);
                float bt = t - segIdx;

                int32 i = segIdx * 3;
                float u = 1.0f - bt;
                FVector pos =
                    u * u * u * path[i] +
                    3.0f * u * u * bt * path[i + 1] +
                    3.0f * u * bt * bt * path[i + 2] +
                    bt * bt * bt * path[i + 3];
                target->SetActorRelativeLocation(pos);
            });
    }

    /// 缩放
    static TSharedPtr<KTweenItem> Scale(AActor* target, FVector to, float time)
    {
        FVector from = target->GetActorScale3D();
        return KTween::AddTween(target, time,
            [target, from, to](float fPercent)
            {
                target->SetActorScale3D(FMath::Lerp(from, to, fPercent));
            });
    }

    /// 欧拉角旋转 — axis 为 (1,0,0)/(0,1,0)/(0,0,1) 等方向向量
    static TSharedPtr<KTweenItem> RotateAround(AActor* target, FVector axis, float angle, float time)
    {
        FRotator startRot = target->GetActorRotation();
        FVector delta = axis.GetSafeNormal() * angle;
        return KTween::AddTween(target, time,
            [target, startRot, delta](float fPercent)
            {
                target->SetActorRotation(startRot + FRotator::MakeFromEuler(delta * fPercent));
            });
    }

    // ── 主方法：返回 TweenItem，缓动由 SetEase 自动处理 ──
    static TSharedPtr<KTweenItem> UMG_MoveLocal_RenderPos(
        UWidget* target, 
        FVector2D to, 
        float time)
    {
        FVector2D InnerFrom = target->GetRenderTransform().Translation;
        FVector2D InnerTo = to;
        return KTween::AddTween(target, time,
            [target, InnerFrom, InnerTo](float T)
            {
                target->SetRenderTranslation(FMath::Lerp(InnerFrom, InnerTo, T));
            });
    }

    static TSharedPtr<KTweenItem> UMG_MoveLocal_SlotPos(
        UWidget* target, 
        FVector2D to, 
        float time)
    {
        auto* Slot = Cast<UCanvasPanelSlot>(target->Slot);
        FVector2D InnerFrom = Slot ? Slot->GetPosition() : FVector2D::ZeroVector;
        FVector2D InnerTo = to;
        return KTween::AddTween(target, time,
            [InnerFrom, InnerTo, Slot](float T)
            {
                if (Slot) Slot->SetPosition(FMath::Lerp(InnerFrom, InnerTo, T));
            });
    }

    // ── 单轴快捷方法 ──
    static TSharedPtr<KTweenItem> UMG_MoveLocal_SlotPosX(UWidget* target, float to, float time)
    {
        auto* Slot = Cast<UCanvasPanelSlot>(target->Slot);
        FVector2D oriPos = Slot ? Slot->GetPosition() : FVector2D::ZeroVector;
        return UMG_MoveLocal_SlotPos(target, FVector2D(to, oriPos.Y), time);
    }
    
    static TSharedPtr<KTweenItem> UMG_MoveLocal_SlotPosY(UWidget* target, float to, float time)
    {
        auto* Slot = Cast<UCanvasPanelSlot>(target->Slot);
        FVector2D oriPos = Slot ? Slot->GetPosition() : FVector2D::ZeroVector;
        return UMG_MoveLocal_SlotPos(target, FVector2D(oriPos.X, to), time);
    }

    static TSharedPtr<KTweenItem> UMG_MoveLocal_RenderPosX(UWidget* target, float to, float time)
    {
        FVector2D oriPos = target->GetRenderTransform().Translation;
        return UMG_MoveLocal_RenderPos(target, FVector2D(to, oriPos.Y), time);
    }

    static TSharedPtr<KTweenItem> UMG_MoveLocal_RenderPosY(UWidget* target, float to, float time)
    {
        FVector2D oriPos = target->GetRenderTransform().Translation;
        return UMG_MoveLocal_RenderPos(target, FVector2D(oriPos.X, to), time);
    }

    static TSharedPtr<KTweenItem> UMG_Opacity(UWidget* target, float to, float time)
    {
        float InnerFrom = target->GetRenderOpacity();
        return KTween::AddTween(target, time,
            [target, InnerFrom, to](float T)
            {
                target->SetRenderOpacity(FMath::Lerp(InnerFrom, to, T));
            });
    }

    static TSharedPtr<KTweenItem> UMG_Scale(UWidget* target, FVector2D to, float time)
    {
        FVector2D InnerFrom = target->GetRenderTransform().Scale;
        return KTween::AddTween(target, time,
            [target, InnerFrom, to](float T)
            {
                target->SetRenderScale(FMath::Lerp(InnerFrom, to, T));
            });
    }

};
