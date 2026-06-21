using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class KTween
{
    public class KTweenByList
    {
        private ObjectPool mItemPool = new ObjectPool();
        private List<TweenItem> mTweenT = new List<TweenItem>();
        public void Update()
        {
            for (int i = mTweenT.Count - 1; i >= 0; i--)
            {
                var mItem = mTweenT[i];
                if (mItem.toggle == false || mItem.bindObj == null)
                {
                    mTweenT.RemoveAt(i);
                    mItemPool.recycle(mItem);
                    continue;
                }

                if (mItem.delay > 0f)
                {
                    mItem.delay -= Time.deltaTime;
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
                                    mTweenT.RemoveAt(i);
                                    mItemPool.recycle(mItem);
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
                                mTweenT.RemoveAt(i);
                                mItemPool.recycle(mItem);
                            }
                        }
                    }
                }
            }
        }

        public TweenItem AddTween(float time, Action<float> updateFunc = null, Action finishFunc = null)
        {
            TweenItem mItem = mItemPool.Pop();
            mItem.toggle = true;
            mItem.bindObj = KTweenMgr.Instance.gameObject;
            mItem.time = 0;
            mItem.sumTime = time;
            mItem.updateFunc = updateFunc;
            mItem.finishFunc = finishFunc;
            mTweenT.Add(mItem);
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
            mTweenT.Add(mItem);
            return mItem;
        }

        public void SetMaxTweenCount(int nCount)
        {
            mItemPool.SetMaxCapacity(nCount);
        }
    }
}