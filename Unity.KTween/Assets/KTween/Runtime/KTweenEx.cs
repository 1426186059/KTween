using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LeanTween 兼容 API 层 — 内部调用 KTween 实现
/// 所有方法返回 TweenItem，支持链式调用
/// </summary>
public static class KTweenEx
{
    public static KTween.TweenItem move(GameObject obj, Vector3 to, float time)
    {
        Vector3 from = obj.transform.position;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.position = Vector3.Lerp(from, to, fPercent);
        });
    }

    public static KTween.TweenItem moveX(GameObject obj, float x, float time)
    {
        Vector3 from = obj.transform.position;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.position = new Vector3(
                Mathf.Lerp(from.x, x, fPercent),
                from.y, from.z);
        });
    }

    public static KTween.TweenItem moveY(GameObject obj, float y, float time)
    {
        Vector3 from = obj.transform.position;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.position = new Vector3(
                from.x,
                Mathf.Lerp(from.y, y, fPercent),
                from.z);
        });
    }

    public static KTween.TweenItem moveZ(GameObject obj, float z, float time)
    {
        Vector3 from = obj.transform.position;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.position = new Vector3(
                from.x, from.y,
                Mathf.Lerp(from.z, z, fPercent));
        });
    }

    public static KTween.TweenItem moveLocal(GameObject obj, Vector3 to, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.localPosition = Vector3.Lerp(from, to, fPercent);
        });
    }

    public static KTween.TweenItem moveLocalX(GameObject obj, float x, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.localPosition = new Vector3(
                Mathf.Lerp(from.x, x, fPercent),
                from.y, from.z);
        });
    }

    public static KTween.TweenItem moveLocalY(GameObject obj, float y, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.localPosition = new Vector3(
                from.x,
                Mathf.Lerp(from.y, y, fPercent),
                from.z);
        });
    }

    public static KTween.TweenItem moveLocalZ(GameObject obj, float z, float time)
    {
        Vector3 from = obj.transform.localPosition;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.localPosition = new Vector3(
                from.x, from.y,
                Mathf.Lerp(from.z, z, fPercent));
        });
    }

    public static KTween.TweenItem scale(GameObject obj, Vector3 to, float time)
    {
        Vector3 from = obj.transform.localScale;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.localScale = Vector3.Lerp(from, to, fPercent);
        });
    }

    public static KTween.TweenItem rotateAround(GameObject obj, Vector3 axis, float angle, float time)
    {
        Vector3 startEuler = obj.transform.eulerAngles;
        Vector3 delta = axis.normalized * angle;
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.eulerAngles = startEuler + delta * fPercent;
        });
    }

    public static KTween.TweenItem rotateAroundLocal(GameObject obj, Vector3 axis, float angle, float time)
    {
        Quaternion startRot = obj.transform.localRotation;
        Quaternion delta = Quaternion.AngleAxis(angle, axis.normalized);
        return KTween.AddTween(obj, time, fPercent =>
        {
            obj.transform.localRotation = Quaternion.Slerp(
                Quaternion.identity, delta, fPercent) * startRot;
        });
    }

    public static KTween.TweenItem color(GameObject obj, Color to, float time)
    {
        var r = obj.GetComponent<Renderer>();
        var from = r.material.color;
        return KTween.AddTween(obj, time, fPercent =>
        {
            r.material.color = Color.Lerp(from, to, fPercent);
        });
    }

    public static KTween.TweenItem color(Graphic obj, Color to, float time)
    {
        var from = obj.color;
        return KTween.AddTween(obj.gameObject, time, fPercent =>
        {
            obj.color = Color.Lerp(from, to, fPercent);
        });
    }

    public static KTween.TweenItem alpha(GameObject obj, float to, float time)
    {
        var r = obj.GetComponent<Renderer>();
        return KTween.AddTween(obj, time, fPercent =>
        {
            Color c = r.material.color;
            r.material.color = new Color(c.r, c.g, c.b,
                Mathf.Lerp(c.a, to, fPercent));
        });
    }

    public static KTween.TweenItem alpha(Graphic obj, float to, float time)
    {
        var from = obj.color;
        return KTween.AddTween(obj.gameObject, time, fPercent =>
        {
            Color c = obj.color;
            obj.color = new Color(c.r, c.g, c.b,
                Mathf.Lerp(c.a, to, fPercent));
        });
    }

    public static KTween.TweenItem alphaCanvas(CanvasGroup cg, float to, float time)
    {
        float from = cg.alpha;
        return KTween.AddTween(cg.gameObject, time, fPercent =>
        {
            cg.alpha = Mathf.Lerp(from, to, fPercent);
        });
    }
}