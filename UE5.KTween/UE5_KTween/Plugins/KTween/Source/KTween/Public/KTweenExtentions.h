#pragma once

#include "CoreMinimal.h"
#include "Components/Widget.h"
#include "Components/CanvasPanelSlot.h"
#include "KTween.h"

class KTweenExtentions
{
public:
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
