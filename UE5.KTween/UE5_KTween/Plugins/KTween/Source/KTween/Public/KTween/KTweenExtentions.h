#pragma once

#include "CoreMinimal.h"
#include "Components/Widget.h"
#include "Components/CanvasPanelSlot.h"
#include "KTween.h"

class KTweenExtentions
{
public:
    static TSharedPtr<KTweenItem> UMG_MoveLocal_RenderPos(
        UWidget* target, 
        FVector2D to, 
        float time, 
        KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        FVector2D InnerFrom = target->GetRenderTransform().Translation;
        FVector2D InnerTo = to;
        auto EaseFunc = KTween::GetEaseFunc<FVector2D>(nEaseType);
        return KTween::AddTween(target, time,
            [=](float fPercent)
            {
                FVector2D targetPos = EaseFunc(InnerFrom, InnerTo, fPercent);
                target->SetRenderTranslation(targetPos);
            });
    }

    static TSharedPtr<KTweenItem> UMG_MoveLocal_SlotPos(
        UWidget* target, 
        FVector2D to, 
        float time, 
        KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        auto* Slot = Cast<UCanvasPanelSlot>(target->Slot);
        FVector2D InnerFrom = Slot ? Slot->GetPosition() : FVector2D::ZeroVector;
        FVector2D InnerTo = to;
        auto EaseFunc = KTween::GetEaseFunc<FVector2D>(nEaseType);
        return KTween::AddTween(target, time,
            [=](float fPercent)
            {
                FVector2D targetPos = EaseFunc(InnerFrom, InnerTo, fPercent);
                if (Slot) Slot->SetPosition(targetPos);
            });
    }

    static TSharedPtr<KTweenItem> UMG_MoveLocal_SlotPosX(UWidget* target, float to, float time, KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        auto* Slot = Cast<UCanvasPanelSlot>(target->Slot);
        FVector2D oriPos = Slot ? Slot->GetPosition() : FVector2D::ZeroVector;
        FVector2D InnerTo = FVector2D(to, oriPos.Y);
        return UMG_MoveLocal_SlotPos(target, InnerTo, time, nEaseType);
    }
    
    static TSharedPtr<KTweenItem> UMG_MoveLocal_SlotPosY(UWidget* target, float to, float time, KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        auto* Slot = Cast<UCanvasPanelSlot>(target->Slot);
        FVector2D oriPos = Slot ? Slot->GetPosition() : FVector2D::ZeroVector;
        FVector2D InnerTo = FVector2D(oriPos.X, to);
        return UMG_MoveLocal_SlotPos(target, InnerTo, time, nEaseType);
    }

    static TSharedPtr<KTweenItem> UMG_MoveLocal_RenderPosX(UWidget* target, float to, float time, KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        FVector2D oriPos = target->GetRenderTransform().Translation;
        FVector2D InnerTo = FVector2D(to, oriPos.Y);
        return UMG_MoveLocal_RenderPos(target, InnerTo, time, nEaseType);
    }

    static TSharedPtr<KTweenItem> UMG_MoveLocal_RenderPosY(UWidget* target, float to, float time, KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        FVector2D oriPos = target->GetRenderTransform().Translation;
        FVector2D InnerTo = FVector2D(oriPos.X, to);
        return UMG_MoveLocal_RenderPos(target, InnerTo, time, nEaseType);
    }

    static TSharedPtr<KTweenItem> UMG_Opacity(UWidget* target, float to, float time, KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        float InnerFrom = target->GetRenderOpacity();
        float InnerTo = to;
        auto EaseFunc = KTween::GetEaseFunc<float>(nEaseType);
        return KTween::AddTween(target, time,
            [=](float fPercent)
            {
                float value = EaseFunc(InnerFrom, InnerTo, fPercent);
                target->SetRenderOpacity(value);
            });
    }

    static TSharedPtr<KTweenItem> UMG_Scale(UWidget* target, FVector2D to, float time, KTween::EaseType nEaseType = KTween::EaseType::linear)
    {
        FVector2D InnerFrom = target->GetRenderTransform().Scale;
        FVector2D InnerTo = to;
        auto EaseFunc = KTween::GetEaseFunc<FVector2D>(nEaseType);
        return KTween::AddTween(target, time,
            [=](float fPercent)
            {
                FVector2D value = EaseFunc(InnerFrom, InnerTo, fPercent);
                target->SetRenderScale(value);
            });
    }

};

   