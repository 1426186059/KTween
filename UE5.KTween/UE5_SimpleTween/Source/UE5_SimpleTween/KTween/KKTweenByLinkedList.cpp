
#include "KKTweenHead.h"

#ifdef USE_LinkedList
namespace KKTweenAPI
{
    KKTweenByLinkedList::KKTweenByLinkedList(AKKTweenMgr* mDefaultBindObj)
    {
        this->defaultBindObj = mDefaultBindObj;
    }

    void KKTweenByLinkedList::Update(float DeltaTime)
    {
        auto mNode = this->mTweenT.GetHead();
        while (mNode != nullptr)
        {
           TSharedPtr<KKTweenItem> mItem = mNode->GetValue();
            if (mItem->toggle == false || mItem->bindObj == nullptr)
            {
                mNode = DoRemove(mItem);
                continue;
            }

            if (mItem->delay > 0.0f)
            {
                mItem->delay -= DeltaTime;
                mNode = mNode->GetNextNode();
                continue;
            }

            if (mItem->startFunc.IsSet())
            {
                mItem->startFunc();
                mItem->startFunc.Reset();
            }

            if (mItem->nLoopPingTong > 0)
            {
                if (mItem->nLoopPingTong == 2)
                {
                    mItem->time -= DeltaTime;
                }
                else
                {
                    mItem->time += DeltaTime;
                }

                mItem->time = FMath::Clamp(mItem->time, 0.0, mItem->sumTime);

                float fTimePercent = 0;
                if (mItem->sumTime > 0)
                {
                    fTimePercent = mItem->time / mItem->sumTime;
                }
                else
                {
                    fTimePercent = 1.0;
                }

                if (mItem->updateFunc.IsSet())
                {
                    mItem->updateFunc(fTimePercent);
                }

                if (mItem->nLoopPingTong == 2)
                {
                    if (fTimePercent <= 0)
                    {
                        if (mItem->finishFunc.IsSet())
                        {
                            mItem->finishFunc();
                        }

                        mItem->nLoopPingTong = 1;
                        if (mItem->nLoopCount > 0)
                        {
                            mItem->nLoopCount--;

                            if (mItem->nLoopCount <= 0)
                            {
                                mNode = DoRemove(mItem);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (fTimePercent >= 1.0f)
                    {
                        mItem->nLoopPingTong = 2;
                    }
                }

            }
            else
            {
                mItem->time += DeltaTime;
                mItem->time = FMath::Clamp(mItem->time, 0, mItem->sumTime);

                float fTimePercent = 0;
                if (mItem->sumTime > 0)
                {
                    fTimePercent = mItem->time / mItem->sumTime;
                }
                else
                {
                    fTimePercent = 1.0;
                }

                if (mItem->updateFunc.IsSet())
                {
                    mItem->updateFunc(fTimePercent);
                }

                if (fTimePercent >= 1.0f)
                {
                    if (mItem->finishFunc.IsSet())
                    {
                        mItem->finishFunc();
                    }

                    if (mItem->nLoopCount == -1)
                    {
                        mItem->time = 0;
                    }
                    else
                    {
                        mItem->nLoopCount--;
                        mItem->time = 0;

                        if (mItem->nLoopCount <= 0)
                        {
                            mNode = DoRemove(mItem);
                            continue;
                        }
                    }
                }
            }

            mNode = mNode->GetNextNode();
        }
    }

    TDoubleLinkedList<TSharedPtr<KKTweenItem>>::TDoubleLinkedListNode* KKTweenByLinkedList::DoRemove(
        TSharedPtr<KKTweenItem> mItem)
    {
        auto mNextNode = mItem->GetNodeEntry()->GetNextNode();
        mTweenT.RemoveNode(mItem->GetNodeEntry(), false);
        mItemPool.recycle(mItem);
        return mNextNode;
    }

    void KKTweenByLinkedList::SetMaxTweenCount(int nCount)
    {
        mItemPool.SetMaxCapacity(nCount);
    }

    TSharedPtr<KKTweenItem> KKTweenByLinkedList::AddTween(
        UObject* obj,
        float time,
        Action_Float_Delegate updateFunc,
        ActionDelegate finishFunc)
    {
        check(obj);
        check(time >= 0);

        if (obj == nullptr)
        {
            obj = defaultBindObj;
        }

        TSharedPtr<KKTweenItem> mItem = mItemPool.Pop();
        mItem->bindObj = obj;
        mItem->toggle = true;
        mItem->time = 0;
        mItem->sumTime = time;
        mItem->updateFunc = updateFunc;
        mItem->finishFunc = finishFunc;
        mTweenT.AddTail(mItem->GetNodeEntry());
        return mItem;
    }

    void KKTweenByLinkedList::Cancel(UObject* obj)
    {
        auto mNode = this->mTweenT.GetHead();
        while (mNode != nullptr)
        {
            TSharedPtr<KKTweenItem> mItem = mNode->GetValue();
            if (mItem->bindObj == obj)
            {
                mItem->toggle = false;
            }
            mNode = mNode->GetNextNode();
        }
    }

    void KKTweenByLinkedList::CancelAll()
    {
        auto mNode = this->mTweenT.GetHead();
        while (mNode != nullptr)
        {
            TSharedPtr<KKTweenItem> mItem = mNode->GetValue();
            mItem->toggle = false;
            mNode = mNode->GetNextNode();
        }
    }
}
#endif
