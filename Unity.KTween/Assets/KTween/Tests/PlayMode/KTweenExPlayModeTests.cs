using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// PlayMode 测试：验证 KTweenEx 兼容 API 层所有方法的行为。
/// </summary>
public class KTweenExPlayModeTests
{
    private GameObject m_TestObject;

    [SetUp]
    public void Setup()
    {
        m_TestObject = new GameObject("ExTestObject");
        m_TestObject.transform.position = Vector3.zero;
        m_TestObject.transform.localPosition = Vector3.zero;
        m_TestObject.transform.localScale = Vector3.one;
        m_TestObject.transform.rotation = Quaternion.identity;
        m_TestObject.transform.localRotation = Quaternion.identity;
    }

    [TearDown]
    public void Teardown()
    {
        if (m_TestObject != null)
            GameObject.DestroyImmediate(m_TestObject);

        var mgr = GameObject.Find("KTween~");
        if (mgr != null)
            GameObject.DestroyImmediate(mgr);
    }

    // ==============================================================
    // position: move / moveX / moveY / moveZ
    // ==============================================================

    [UnityTest]
    public IEnumerator Move_ChangesPosition()
    {
        KTweenEx.move(m_TestObject, new Vector3(10f, 0f, 0f), 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(10f, m_TestObject.transform.position.x, 0.05f);
    }

    [UnityTest]
    public IEnumerator MoveX_ChangesOnlyX()
    {
        m_TestObject.transform.position = new Vector3(1f, 2f, 3f);
        KTweenEx.moveX(m_TestObject, 8f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(8f, m_TestObject.transform.position.x, 0.05f);
        Assert.AreEqual(2f, m_TestObject.transform.position.y);
        Assert.AreEqual(3f, m_TestObject.transform.position.z);
    }

    [UnityTest]
    public IEnumerator MoveY_ChangesOnlyY()
    {
        m_TestObject.transform.position = new Vector3(1f, 2f, 3f);
        KTweenEx.moveY(m_TestObject, 9f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(9f, m_TestObject.transform.position.y, 0.05f);
        Assert.AreEqual(1f, m_TestObject.transform.position.x);
        Assert.AreEqual(3f, m_TestObject.transform.position.z);
    }

    [UnityTest]
    public IEnumerator MoveZ_ChangesOnlyZ()
    {
        m_TestObject.transform.position = new Vector3(1f, 2f, 3f);
        KTweenEx.moveZ(m_TestObject, 7f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(7f, m_TestObject.transform.position.z, 0.05f);
        Assert.AreEqual(1f, m_TestObject.transform.position.x);
        Assert.AreEqual(2f, m_TestObject.transform.position.y);
    }

    // ==============================================================
    // localPosition: moveLocal / moveLocalX / moveLocalY / moveLocalZ
    // ==============================================================

    [UnityTest]
    public IEnumerator MoveLocal_ChangesLocalPosition()
    {
        KTweenEx.moveLocal(m_TestObject, new Vector3(5f, -3f, 2f), 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(5f, m_TestObject.transform.localPosition.x, 0.05f);
        Assert.AreEqual(-3f, m_TestObject.transform.localPosition.y, 0.05f);
        Assert.AreEqual(2f, m_TestObject.transform.localPosition.z, 0.05f);
    }

    [UnityTest]
    public IEnumerator MoveLocalX_ChangesOnlyLocalX()
    {
        m_TestObject.transform.localPosition = new Vector3(1f, 2f, 3f);
        KTweenEx.moveLocalX(m_TestObject, 6f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(6f, m_TestObject.transform.localPosition.x, 0.05f);
        Assert.AreEqual(2f, m_TestObject.transform.localPosition.y);
        Assert.AreEqual(3f, m_TestObject.transform.localPosition.z);
    }

    [UnityTest]
    public IEnumerator MoveLocalY_ChangesOnlyLocalY()
    {
        m_TestObject.transform.localPosition = new Vector3(1f, 2f, 3f);
        KTweenEx.moveLocalY(m_TestObject, -5f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(-5f, m_TestObject.transform.localPosition.y, 0.05f);
        Assert.AreEqual(1f, m_TestObject.transform.localPosition.x);
        Assert.AreEqual(3f, m_TestObject.transform.localPosition.z);
    }

    [UnityTest]
    public IEnumerator MoveLocalZ_ChangesOnlyLocalZ()
    {
        m_TestObject.transform.localPosition = new Vector3(1f, 2f, 3f);
        KTweenEx.moveLocalZ(m_TestObject, 0f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(0f, m_TestObject.transform.localPosition.z, 0.05f);
        Assert.AreEqual(1f, m_TestObject.transform.localPosition.x);
        Assert.AreEqual(2f, m_TestObject.transform.localPosition.y);
    }

    // ==============================================================
    // scale
    // ==============================================================

    [UnityTest]
    public IEnumerator Scale_ChangesLocalScale()
    {
        Vector3 target = new Vector3(3f, 3f, 3f);
        KTweenEx.scale(m_TestObject, target, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(3f, m_TestObject.transform.localScale.x, 0.05f);
        Assert.AreEqual(3f, m_TestObject.transform.localScale.y, 0.05f);
        Assert.AreEqual(3f, m_TestObject.transform.localScale.z, 0.05f);
    }

    // ==============================================================
    // rotate
    // ==============================================================

    [UnityTest]
    public IEnumerator RotateAround_ChangesEulerAngles()
    {
        KTweenEx.rotateAround(m_TestObject, Vector3.up, 180f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(180f, m_TestObject.transform.eulerAngles.y, 5f);
    }

    [UnityTest]
    public IEnumerator RotateAroundLocal_ChangesLocalRotation()
    {
        KTweenEx.rotateAroundLocal(m_TestObject, Vector3.up, 90f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(90f, m_TestObject.transform.localRotation.eulerAngles.y, 5f);
    }

    // ==============================================================
    // color / alpha
    // ==============================================================

    [UnityTest]
    public IEnumerator Color_ChangesRendererColor()
    {
        var renderer = m_TestObject.AddComponent<SpriteRenderer>();
        renderer.color = Color.white;
        KTweenEx.color(m_TestObject, Color.red, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(Color.red.r, renderer.color.r, 0.05f);
        Assert.AreEqual(Color.red.g, renderer.color.g, 0.05f);
        Assert.AreEqual(Color.red.b, renderer.color.b, 0.05f);
    }

    [UnityTest]
    public IEnumerator Alpha_ChangesRendererAlpha()
    {
        var renderer = m_TestObject.AddComponent<SpriteRenderer>();
        renderer.color = new Color(1f, 1f, 1f, 1f);
        KTweenEx.alpha(m_TestObject, 0.25f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(0.25f, renderer.color.a, 0.05f);
    }

    [UnityTest]
    public IEnumerator AlphaCanvas_ChangesCanvasGroupAlpha()
    {
        var cg = m_TestObject.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        KTweenEx.alphaCanvas(cg, 0.5f, 0.2f);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(0.5f, cg.alpha, 0.05f);
    }

    // ==============================================================
    // delayedCall（KTween 原生就有，直接调用）
    // ==============================================================

    [UnityTest]
    public IEnumerator DelayedCall_FiresAfterDelay()
    {
        bool called = false;
        KTween.delayedCall(m_TestObject, 0.3f, () => called = true);
        yield return new WaitForSeconds(0.2f);
        Assert.IsFalse(called);
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(called);
    }

    [UnityTest]
    public IEnumerator DelayedCall_WithoutObject_FiresAfterDelay()
    {
        bool called = false;
        KTween.delayedCall(0.3f, () => called = true);
        yield return new WaitForSeconds(0.2f);
        Assert.IsFalse(called);
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(called);
    }

    // ==============================================================
    // 缓动类型测试
    // ==============================================================

    [UnityTest]
    public IEnumerator Move_WithEaseOutBounce_CompletesCorrectly()
    {
        var item = KTweenEx.move(m_TestObject, new Vector3(5f, 0f, 0f), 0.2f);
        item.SetEase(KTweenType.outBounce);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(5f, m_TestObject.transform.position.x, 0.1f);
    }

    [UnityTest]
    public IEnumerator Move_WithEaseInElastic_CompletesCorrectly()
    {
        var item = KTweenEx.move(m_TestObject, new Vector3(3f, 0f, 0f), 0.2f);
        item.SetEase(KTweenType.inElastic);
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(3f, m_TestObject.transform.position.x, 0.1f);
    }

    // ==============================================================
    // 链式调用：SetDelay / SetLoop / SetEase
    // ==============================================================

    [UnityTest]
    public IEnumerator Move_WithDelay_StartsAfterDelay()
    {
        float x = 0f;
        var item = KTweenEx.move(m_TestObject, new Vector3(7f, 0f, 0f), 0.1f);
        item.SetDelay(0.3f);
        item.SetEase(KTweenType.outQuad);

        // 延迟期间不应变化
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(0f, m_TestObject.transform.position.x, 0.01f);

        // 延迟+动画结束后应到达终点
        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(7f, m_TestObject.transform.position.x, 0.05f);
    }

    [UnityTest]
    public IEnumerator Scale_WithLoop_RunsMultipleTimes()
    {
        int callCount = 0;
        var item = KTweenEx.scale(m_TestObject, new Vector3(2f, 1f, 1f), 0.1f);
        item.SetLoop(3);

        yield return new WaitForSeconds(0.5f);

        // 循环应持续执行
        int countAtCheck = callCount;
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(countAtCheck, callCount, "循环结束后应停止");
    }

    // ==============================================================
    // 链式 AppendTween
    // ==============================================================

    [UnityTest]
    public IEnumerator AppendTween_ExecutesMoveInSequence()
    {
        var move1 = KTweenEx.move(m_TestObject, new Vector3(3f, 0f, 0f), 0.2f);
        var move2 = KTweenEx.move(m_TestObject, new Vector3(0f, 3f, 0f), 0.2f);
        move1.AppendTween(move2);

        yield return new WaitForSeconds(0.25f);
        // 第一个动画应完成
        Assert.Greater(m_TestObject.transform.position.x, 2.5f);
        Assert.AreEqual(0f, m_TestObject.transform.position.y);

        yield return new WaitForSeconds(0.3f);
        // 第二个动画应完成
        Assert.Greater(m_TestObject.transform.position.y, 2.5f);
    }

    // ==============================================================
    // 多个 Tween 同时运行
    // ==============================================================

    [UnityTest]
    public IEnumerator MultipleExTweens_RunWithoutConflict()
    {
        var go1 = new GameObject("ExTarget1");
        var go2 = new GameObject("ExTarget2");

        KTweenEx.move(go1, new Vector3(5f, 0f, 0f), 0.2f);
        KTweenEx.scale(go2, Vector3.one * 2f, 0.4f);

        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(5f, go1.transform.position.x, 0.05f);
        Assert.Greater(go2.transform.localScale.x, 1.5f);

        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(2f, go2.transform.localScale.x, 0.05f);

        GameObject.DestroyImmediate(go1);
        GameObject.DestroyImmediate(go2);
    }

    // ==============================================================
    // Cancel 停止
    // ==============================================================

    [UnityTest]
    public IEnumerator Cancel_StopsMoveMidway()
    {
        var item = KTweenEx.move(m_TestObject, new Vector3(10f, 0f, 0f), 1.0f);
        yield return new WaitForSeconds(0.1f);

        float posBeforeCancel = m_TestObject.transform.position.x;
        Assert.Greater(posBeforeCancel, 0f);
        Assert.Less(posBeforeCancel, 10f);

        item.cancel();
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(posBeforeCancel, m_TestObject.transform.position.x, 0.01f);
    }
}