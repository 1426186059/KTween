using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LeanTween 兼容 API 层 — 内部调用 SimpleTween 实现
/// 所有方法返回 TweenItem，支持链式调用
/// </summary>
public static class SimpleTweenEx
{
    public static float tau => Mathf.PI * 2f;

    public static void init(int maxTweenCount = 1300) => SimpleTween.SetMaxTweenCount(maxTweenCount);

    // ==============================================================
    // 位置移动
    // ==============================================================
    public static SimpleTween.TweenItem move(GameObject obj, Vector3 to, float time)
        => TweenPosition(obj, obj.transform.position, to, time);
    public static SimpleTween.TweenItem moveX(GameObject obj, float x, float time, SimpleTweenType nType)
    { Vector3 f = obj.transform.position; return TweenPosition(obj, f, new Vector3(x, f.y, f.z), time); }
    public static SimpleTween.TweenItem moveY(GameObject obj, float y, float time)
    { Vector3 f = obj.transform.position; return TweenPosition(obj, f, new Vector3(f.x, y, f.z), time); }
    public static SimpleTween.TweenItem moveZ(GameObject obj, float z, float time)
    { Vector3 f = obj.transform.position; return TweenPosition(obj, f, new Vector3(f.x, f.y, z), time); }
    public static SimpleTween.TweenItem moveLocal(GameObject obj, Vector3 to, float time)
        => TweenLocalPosition(obj, obj.transform.localPosition, to, time);
    public static SimpleTween.TweenItem moveLocalX(GameObject obj, float x, float time)
    { Vector3 f = obj.transform.localPosition; return TweenLocalPosition(obj, f, new Vector3(x, f.y, f.z), time); }
    public static SimpleTween.TweenItem moveLocalY(GameObject obj, float y, float time)
    { Vector3 f = obj.transform.localPosition; return TweenLocalPosition(obj, f, new Vector3(f.x, y, f.z), time); }
    public static SimpleTween.TweenItem moveLocalZ(GameObject obj, float z, float time)
    { Vector3 f = obj.transform.localPosition; return TweenLocalPosition(obj, f, new Vector3(f.x, f.y, z), time); }

    // ==============================================================
    // 缩放
    // ==============================================================
    public static SimpleTween.TweenItem scale(GameObject obj, Vector3 to, float time)
    {
        Vector3 from = obj.transform.localScale;
        return Create(obj, time, t => obj.transform.localScale = Vector3.Lerp(from, to, t));
    }

    // ==============================================================
    // 旋转
    // ==============================================================
    public static SimpleTween.TweenItem rotateAround(GameObject obj, Vector3 axis, float angle, float time)
    {
        Vector3 startEuler = obj.transform.eulerAngles;
        Vector3 delta = axis.normalized * angle;
        return Create(obj, time, t => obj.transform.eulerAngles = startEuler + delta * t);
    }
    public static SimpleTween.TweenItem rotateAroundLocal(GameObject obj, Vector3 axis, float angle, float time)
    {
        Quaternion startRot = obj.transform.localRotation;
        Quaternion delta = Quaternion.AngleAxis(angle, axis.normalized);
        return Create(obj, time, t => obj.transform.localRotation = Quaternion.Slerp(Quaternion.identity, delta, t) * startRot);
    }

    // ==============================================================
    // 颜色 & 透明度
    // ==============================================================
    public static SimpleTween.TweenItem color(GameObject obj, Color to, float time)
    {
        var r = obj.GetComponent<Renderer>();
        if (r == null) return Create(obj, 0f, null);
        Color from = r.material.color;
        return Create(obj, time, t => r.material.color = Color.Lerp(from, to, t));
    }
    public static SimpleTween.TweenItem alpha(GameObject obj, float to, float time)
    {
        var r = obj.GetComponent<Renderer>();
        if (r == null) return Create(obj, 0f, null);
        return Create(obj, time, t => {
            Color c = r.material.color;
            r.material.color = new Color(c.r, c.g, c.b, Mathf.Lerp(c.a, to, t));
        });
    }
    public static SimpleTween.TweenItem alphaCanvas(CanvasGroup cg, float to, float time)
    {
        float from = cg.alpha;
        return Create(cg.gameObject, time, t => cg.alpha = Mathf.Lerp(from, to, t));
    }

    // ==============================================================
    // 通用值渐变
    // ==============================================================
    public static SimpleTween.TweenItem value(GameObject obj, float from, float to, float time)
        => Create(obj, time, t => { });
    public static SimpleTween.TweenItem value(GameObject obj, Vector2 from, Vector2 to, float time)
        => Create(obj, time, t => { });
    public static SimpleTween.TweenItem value(GameObject obj, Vector3 from, Vector3 to, float time)
        => Create(obj, time, t => { });

    // ==============================================================
    // 延时调用
    // ==============================================================
    public static SimpleTween.TweenItem delayedCall(GameObject obj, float time, Action callback)
        => SimpleTween.delayedCall(obj, time, callback);
    public static SimpleTween.TweenItem delayedCall(float time, Action callback)
        => SimpleTween.delayedCall(time, callback);

    // ==============================================================
    // 内部实现
    // ==============================================================
    private static SimpleTween.TweenItem TweenPosition(GameObject obj, Vector3 from, Vector3 to, float time)
        => Create(obj, time, t => obj.transform.position = Vector3.Lerp(from, to, t));
    private static SimpleTween.TweenItem TweenLocalPosition(GameObject obj, Vector3 from, Vector3 to, float time)
        => Create(obj, time, t => obj.transform.localPosition = Vector3.Lerp(from, to, t));

    public static SimpleTween.TweenItem Create(GameObject obj, float time, Action<float> update, Action onComplete = null)
    {
        return SimpleTween.AddTween(obj, time, t => {
            update?.Invoke(t);
        }, onComplete);
    }
}
