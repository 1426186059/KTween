#pragma once

#include "CoreMinimal.h"
#include "KKTween.h"
#include "UE5_Solitaire/SimpleFramework/KKUI/UMGHelper.h"

class KKTweenExtentions
{
public:
    static TSharedPtr<KKTweenItem> UMG_MoveLocal_RenderPos(
        UWidget* target, 
        FVector2D to, 
        float time, 
        KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        FVector2D InnerFrom = UMGHelper::GetRenderPos(target);
        FVector2D InnerTo = to;
        auto EaseFunc = KKTween::GetEaseFunc<FVector2D>(nEaseType);
        return KKTween::AddTween(target, time,
            [=](float fPercent)
            {
                FVector2D targetPos = EaseFunc(InnerFrom, InnerTo, fPercent);
                UMGHelper::SetRenderPos(target, targetPos);
            });
    }

    static TSharedPtr<KKTweenItem> UMG_MoveLocal_SlotPos(
        UWidget* target, 
        FVector2D to, 
        float time, 
        KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        FVector2D InnerFrom = UMGHelper::GetSlotPos(target);
        FVector2D InnerTo = to;
        auto EaseFunc = KKTween::GetEaseFunc<FVector2D>(nEaseType);
        return KKTween::AddTween(target, time,
            [=](float fPercent)
            {
                FVector2D targetPos = EaseFunc(InnerFrom, InnerTo, fPercent);
                UMGHelper::SetSlotPos(target, targetPos);
            });
    }

    static TSharedPtr<KKTweenItem> UMG_MoveLocal_SlotPosX(UWidget* target, float to, float time, KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        FVector2D oriPos = UMGHelper::GetSlotPos(target);
        FVector2D InnerTo = FVector2D(to, oriPos.Y);
        return UMG_MoveLocal_SlotPos(target, InnerTo, time, nEaseType);
    }
    
    static TSharedPtr<KKTweenItem> UMG_MoveLocal_SlotPosY(UWidget* target, float to, float time, KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        FVector2D oriPos = UMGHelper::GetSlotPos(target);
        FVector2D InnerTo = FVector2D(oriPos.X, to);
        return UMG_MoveLocal_SlotPos(target, InnerTo, time, nEaseType);
    }

    static TSharedPtr<KKTweenItem> UMG_MoveLocal_RenderPosX(UWidget* target, float to, float time, KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        FVector2D oriPos = UMGHelper::GetRenderPos(target);
        FVector2D InnerTo = FVector2D(to, oriPos.Y);
        return UMG_MoveLocal_RenderPos(target, InnerTo, time, nEaseType);
    }

    static TSharedPtr<KKTweenItem> UMG_MoveLocal_RenderPosY(UWidget* target, float to, float time, KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        FVector2D oriPos = UMGHelper::GetRenderPos(target);
        FVector2D InnerTo = FVector2D(oriPos.X, to);
        return UMG_MoveLocal_RenderPos(target, InnerTo, time, nEaseType);
    }

    static TSharedPtr<KKTweenItem> UMG_Opacity(UWidget* target, float to, float time, KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        float InnerFrom = target->GetRenderOpacity();
        float InnerTo = to;
        auto EaseFunc = KKTween::GetEaseFunc<float>(nEaseType);
        return KKTween::AddTween(target, time,
            [=](float fPercent)
            {
                float value = EaseFunc(InnerFrom, InnerTo, fPercent);
                target->SetRenderOpacity(value);
            });
    }

    static TSharedPtr<KKTweenItem> UMG_Scale(UWidget* target, FVector2D to, float time, KKTween::EaseType nEaseType = KKTween::EaseType::linear)
    {
        FVector2D InnerFrom = target->GetRenderTransform().Scale;
        FVector2D InnerTo = to;
        auto EaseFunc = KKTween::GetEaseFunc<FVector2D>(nEaseType);
        return KKTween::AddTween(target, time,
            [=](float fPercent)
            {
                FVector2D value = EaseFunc(InnerFrom, InnerTo, fPercent);
                target->SetRenderScale(value);
            });
    }

};

   