using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class KTween
{
    public class KTweenByLinkedList
    {
        private readonly ObjectPool mItemPool = new ObjectPool();
        private readonly LinkedList<TweenItem> mTweenT = new LinkedList<TweenItem>();

        public void Update()
        {
            LinkedListNode<TweenItem> mNode = mTweenT.First;
            while (mNode != null)
            {
                TweenItem mItem = mNode.Value;
                if (mItem.toggle == false || mItem.bindObj == null)
                {
                    mNode = DoRemove(mNode);
                    continue;
                }

                if (mItem.delay > 0f)
                {
                    mItem.delay -= Time.deltaTime;
                    mNode = DoNext(mNode);
                    continue;
                }

                if (mItem.nLoopPingTong > 0)
                {
                    if (mItem.nLoopPingTong == 2)
                    {
                        mItem.time -= Time.deltaTime;
                    }
                    else
                    {
                        mItem.time += Time.deltaTime;
                    }

                    mItem.time = Mathf.Clamp(mItem.time, 0, mItem.sumTime);
                    float fTimePercent = mItem.time / mItem.sumTime;
                    mItem.UpdateFunc(fTimePercent);

                    if (mItem.nLoopPingTong == 2)
                    {
                        if (fTimePercent <= 0)
                        {
                            mItem.finishFunc?.Invoke();
                            mItem.nLoopPingTong = 1;
                            if (mItem.nLoopCount > 0)
                            {
                                mItem.nLoopCount--;

                                if (mItem.nLoopCount <= 0)
                                {
                                    mNode = DoRemove(mNode);
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (fTimePercent >= 1.0f)
                        {
                            mItem.nLoopPingTong = 2;
                        }
                    }

                }
                else
                {
                    mItem.time += Time.deltaTime;
                    mItem.time = Mathf.Clamp(mItem.time, 0, mItem.sumTime);
                    float fTimePercent = mItem.time / mItem.sumTime;
                    mItem.UpdateFunc(fTimePercent);

                    if (fTimePercent >= 1.0f)
                    {
                        mItem.finishFunc?.Invoke();
                        if (mItem.nLoopCount == -1)
                        {
                            mItem.time = 0;
                        }
                        else
                        {
                            mItem.nLoopCount--;
                            mItem.time = 0;

                            if (mItem.nLoopCount <= 0)
                            {
                                mNode = DoRemove(mNode);
                                continue;
                            }
                        }
                    }
                }

                mNode = DoNext(mNode);
            }
        }

        private LinkedListNode<TweenItem> DoNext(LinkedListNode<TweenItem> mNode)
        {
            return mNode.Next;
        }

        private LinkedListNode<TweenItem> DoRemove(LinkedListNode<TweenItem> mNode)
        {
            var mNextNode = DoNext(mNode);
            mTweenT.Remove(mNode);
            mItemPool.recycle(mNode.Value);
            return mNextNode;
        }

        public TweenItem AddTween(float time, Action<float> updateFunc = null, Action finishFunc = null)
        {
            TweenItem mItem = mItemPool.Pop();
            mItem.bindObj = KTweenMgr.Instance.gameObject;
            mItem.toggle = true;
            mItem.time = 0;
            mItem.sumTime = time;
            mItem.updateFunc = updateFunc;
            mItem.finishFunc = finishFunc;
            mTweenT.AddLast(mItem.mEntry);
            return mItem;
        }

        public TweenItem AddTween(GameObject obj, float time, Action<float> updateFunc = null, Action finishFunc = null)
        {
            TweenItem mItem = mItemPool.Pop();
            mItem.bindObj = obj;
            mItem.toggle = true;
            mItem.time = 0;
            mItem.sumTime = time;
            mItem.updateFunc = updateFunc;
            mItem.finishFunc = finishFunc;
            mTweenT.AddLast(mItem.mEntry);
            return mItem;
        }

        public void SetMaxTweenCount(int nCount)
        {
            mItemPool.SetMaxCapacity(nCount);
        }
    }
}