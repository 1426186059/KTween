using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimpleTween;

public static partial class SimpleTween
{
    public static void SetMaxTweenCount(int nCount)
    {
        SimpleTweenMgr.Instance.SetMaxTweenCount(nCount);
    }

    public static TweenItemHandle GetHandle(TweenItem mTSharePtr)
    {
        return new TweenItemHandle(mTSharePtr);
    }

    public static TweenItem AddTween(float time, Action<float> updateFunc = null, Action finishFunc = null)
    {
        return SimpleTweenMgr.Instance.AddTween(time, updateFunc, finishFunc);
    }

    public static TweenItem AddTween(GameObject obj, float time, Action<float> updateFunc = null, Action finishFunc = null)
    {
        return SimpleTweenMgr.Instance.AddTween(obj, time, updateFunc, finishFunc);
    }

    public static TweenItem delayedCall(float time, Action finishFunc = null)
    {
        return AddTween(time, null, finishFunc);
    }

    public static TweenItem delayedCall(GameObject obj, float time, Action finishFunc = null)
    {
        return AddTween(obj, time, null, finishFunc);
    }

    // ==============================================================
    // 工厂方法 — 返回 TweenItem，支持 SetEase 链式调用
    // ==============================================================

    /// <summary>位置移动到目标</summary>
    public static TweenItem move(GameObject obj, Vector3 to, float time)
    {
        Vector3 from = obj.transform.position;
        return AddTween(obj, time, t => obj.transform.position = Vector3.Lerp(from, to, t));
    }
    public static TweenItem moveX(GameObject obj, float toX, float time)
    {
        Vector3 from = obj.transform.position;
        return AddTween(obj, time, t => obj.transform.position = new Vector3(Mathf.Lerp(from.x, toX, t), from.y, from.z));
    }
    public static TweenItem moveY(GameObject obj, float toY, float time)
    {
        Vector3 from = obj.transform.position;
        return AddTween(obj, time, t => obj.transform.position = new Vector3(from.x, Mathf.Lerp(from.y, toY, t), from.z));
    }
    public static TweenItem moveZ(GameObject obj, float toZ, float time)
    {
        Vector3 from = obj.transform.position;
        return AddTween(obj, time, t => obj.transform.position = new Vector3(from.x, from.y, Mathf.Lerp(from.z, toZ, t)));
    }
    public static TweenItem moveLocal(GameObject obj, Vector3 to, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return AddTween(obj, time, t => obj.transform.localPosition = Vector3.Lerp(from, to, t));
    }
    public static TweenItem moveLocalX(GameObject obj, float toX, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return AddTween(obj, time, t => obj.transform.localPosition = new Vector3(Mathf.Lerp(from.x, toX, t), from.y, from.z));
    }
    public static TweenItem moveLocalY(GameObject obj, float toY, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return AddTween(obj, time, t => obj.transform.localPosition = new Vector3(from.x, Mathf.Lerp(from.y, toY, t), from.z));
    }
    public static TweenItem moveLocalZ(GameObject obj, float toZ, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return AddTween(obj, time, t => obj.transform.localPosition = new Vector3(from.x, from.y, Mathf.Lerp(from.z, toZ, t)));
    }

    /// <summary>缩放到目标</summary>
    public static TweenItem scale(GameObject obj, Vector3 to, float time)
    {
        Vector3 from = obj.transform.localScale;
        return AddTween(obj, time, t => obj.transform.localScale = Vector3.Lerp(from, to, t));
    }

    /// <summary>绕轴旋转</summary>
    public static TweenItem rotateAround(GameObject obj, Vector3 axis, float angle, float time)
    {
        Quaternion from = obj.transform.rotation;
        Quaternion to = from * Quaternion.AngleAxis(angle, axis.normalized);
        return AddTween(obj, time, t => obj.transform.rotation = Quaternion.Slerp(from, to, t));
    }
    public static TweenItem rotateAroundLocal(GameObject obj, Vector3 axis, float angle, float time)
    {
        Quaternion from = obj.transform.localRotation;
        Quaternion to = Quaternion.AngleAxis(angle, axis.normalized);
        return AddTween(obj, time, t => obj.transform.localRotation = Quaternion.Slerp(Quaternion.identity, to, t) * from);
    }

    /// <summary>颜色渐变</summary>
    public static TweenItem color(GameObject obj, Color to, float time)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return AddTween(obj, 0f, null);
        Color from = r.material.color;
        return AddTween(obj, time, t => r.material.color = Color.Lerp(from, to, t));
    }

    /// <summary>透明度渐变</summary>
    public static TweenItem alpha(GameObject obj, float to, float time)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return AddTween(obj, 0f, null);
        return AddTween(obj, time, t =>
        {
            Color c = r.material.color;
            r.material.color = new Color(c.r, c.g, c.b, Mathf.Lerp(c.a, to, t));
        });
    }

    /// <summary>值渐变（泛用回调）</summary>
    public static TweenItem value(float from, float to, float time, Action<float> onUpdate)
    {
        return AddTween(time, t => onUpdate?.Invoke(Mathf.Lerp(from, to, t)));
    }

    public struct TweenItemHandle : IDisposable
    {
        private uint nVersion;
        private TweenItem mInnerPtr;

        public TweenItemHandle(TweenItem mItem)
        {
            this.mInnerPtr =  mItem;
            this.nVersion = mItem.nVersion;
        }

        public bool IsValid()
        {
            return mInnerPtr != null && mInnerPtr.nVersion == nVersion;
        }

        public void AppendTween(TweenItemHandle mOtherTween)
        {
            if (IsValid() && mOtherTween.IsValid())
            {
                mInnerPtr.AppendTween(mOtherTween.mInnerPtr);
            }
        }

        public void Cancel()
        {
            if (IsValid())
            {
                mInnerPtr.cancel();
            }
            
            mInnerPtr = null;
            nVersion = 0;
        }

        public void Dispose()
        {
            Cancel();
        }
    };


    public class SimpleTweenMgr : MonoBehaviour
    {
        private SimpleTweenByLinkedList mManager = new SimpleTweenByLinkedList();
        private static SimpleTweenMgr m_Instance;

        public static SimpleTweenMgr Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject go = new GameObject("SimpleTween~", typeof(SimpleTweenMgr));
                    DontDestroyOnLoad(go);
                    m_Instance = go.GetComponent<SimpleTweenMgr>();
                }
                return m_Instance;
            }
        }

        private void Update()
        {
            mManager.Update();
        }

        public void SetMaxTweenCount(int nCount)
        {
            mManager.SetMaxTweenCount(nCount);
        }

        public TweenItem AddTween(float time, Action<float> updateFunc = null, Action finishFunc = null)
        {
            return mManager.AddTween(time, updateFunc, finishFunc);
        }

        public TweenItem AddTween(GameObject obj, float time, Action<float> updateFunc = null, Action finishFunc = null)
        {
            return mManager.AddTween(obj, time, updateFunc, finishFunc);
        }

        public TweenItem delayedCall(float time, Action finishFunc = null)
        {
            return AddTween(time, null, finishFunc);
        }

        public TweenItem delayedCall(GameObject obj, float time, Action finishFunc = null)
        {
            return AddTween(obj, time, null, finishFunc);
        }
    }

    public class TweenItem
    {
        public readonly LinkedListNode<TweenItem> mEntry;
        public TweenItem SqeNext;

        public uint nVersion;
        public GameObject bindObj;
        public bool toggle;
        public float delay;
        public float time = 0f;
        public float sumTime = 0f;
        public int nLoopCount = 0;
        public int nLoopPingTong = 0;
        public SimpleTweenType nType = SimpleTweenType.linear;
        public Action<float> updateFunc = null;
        public Action finishFunc = null;

        public TweenItem()
        {
            mEntry = new LinkedListNode<TweenItem>(this);
            Reset();
        }

        public void Reset()
        {
            SqeNext = null;
            nVersion++;
            bindObj = null;
            toggle = false;

            delay = 0f;
            time = 0f;
            sumTime = 0f;
            updateFunc = null;
            finishFunc = null;

            nLoopCount = 0;
            nLoopPingTong = 0;
            nType = SimpleTweenType.linear;
        }

        public TweenItemHandle GetHandle()
        {
            return new TweenItemHandle(this);
        }

        public TweenItem cancel()
        {
            if (toggle)
            {
                toggle = false;

                var mSqeNext = SqeNext;
                while (mSqeNext != null)
                {
                    mSqeNext.toggle = false;
                    mSqeNext = mSqeNext.SqeNext;
                }
            }
            return this;
        }

        public TweenItem SetDelay(float fTime)
        {
            this.delay = fTime;
            return this;
        }

        public TweenItem SetLoop(int nLoopCount = -1)
        {
            this.nLoopCount = nLoopCount;
            return this;
        }

        public TweenItem SetLoopPingPong(int nLoopCount = -1)
        {
            this.nLoopCount = nLoopCount;
            nLoopPingTong = 1;
            return this;
        }

        public TweenItem AppendTween(TweenItem mItem)
        {
            float mTweenSumTime = this.delay + this.sumTime;
            mItem.delay += mTweenSumTime;
            SqeNext = mItem;
            return this;
        }

        public TweenItem SetOnCompleteFunc(Action mFunc)
        {
            this.finishFunc = mFunc;
            return this;
        }

        public TweenItem SetOnUpdateFunc(Action<float> mFunc)
        {
            this.updateFunc = mFunc;
            return this;
        }

        public TweenItem SetEase(SimpleTweenType easeType)
        {
            this.nType = easeType;
            return this;
        }
    }

    private class ObjectPool
    {
        readonly Stack<TweenItem> mObjectPool = new Stack<TweenItem>();
        int nMaxCapacity = 1024;

        public void SetMaxCapacity(int nCount)
        {
            this.nMaxCapacity = nCount;
        }

        public TweenItem Pop()
        {
            if (mObjectPool.Count > 0)
            {
                return mObjectPool.Pop();
            }
            else
            {
                return new TweenItem();
            }
        }

        public void recycle(TweenItem t)
        {
            t.Reset();
            if (mObjectPool.Count < nMaxCapacity)
            {
                mObjectPool.Push(t);
            }
        }
    }
}
