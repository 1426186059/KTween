using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// PlayMode 测试：验证 KTween 在完整 MonoBehaviour 生命周期下的行为。
/// 这些测试依赖 Time.deltaTime 和 Update 循环，需要在 PlayMode 下运行。
/// </summary>
public class KTweenPlayModeTests
{
    private GameObject m_TestObject;

    [SetUp]
    public void Setup()
    {
        m_TestObject = new GameObject("PlayModeTestObject");
    }

    [TearDown]
    public void Teardown()
    {
        if (m_TestObject != null)
            GameObject.DestroyImmediate(m_TestObject);

        // 清理 KTweenMgr 单例，防止场景卸载时残留
        var mgr = GameObject.Find("KTween~");
        if (mgr != null)
            GameObject.DestroyImmediate(mgr);
    }

    /// <summary>
    /// 验证更新函数在 Tween 持续期间被调用的次数
    /// </summary>
    [Test]
    public IEnumerator UpdateFunc_IsCalledMultipleTimes()
    {
        int callCount = 0;
        KTween.AddTween(m_TestObject, 0.5f, (t) => callCount++);

        yield return new WaitForSeconds(0.6f);

        Assert.Greater(callCount, 1);
    }

    /// <summary>
    /// 验证 t 值从 0 逐渐变化到 1
    /// </summary>
    [UnityTest]
    public IEnumerator TimePercent_StartsAtZero_EndsAtOne()
    {
        float startT = 1f, endT = 0f;
        KTween.AddTween(m_TestObject, 0.3f, (t) =>
        {
            if (t < startT) startT = t;
            if (t > endT) endT = t;
        });

        yield return new WaitForSeconds(0.5f);

        Assert.AreEqual(0f, startT, 0.05f);
        Assert.AreEqual(1f, endT, 0.05f);
    }

    /// <summary>
    /// 验证 finishFunc 在 Tween 结束时被调用
    /// </summary>
    [UnityTest]
    public IEnumerator FinishFunc_IsCalledAfterDuration()
    {
        bool finished = false;
        KTween.AddTween(m_TestObject, 0.2f, null, () => finished = true);

        // 在结束前检查
        yield return new WaitForSeconds(0.1f);
        Assert.IsFalse(finished);

        // 等待结束后检查
        yield return new WaitForSeconds(0.2f);
        Assert.IsTrue(finished);
    }

    /// <summary>
    /// 验证延迟功能：在延迟期间 updateFunc 不应被调用
    /// </summary>
    [UnityTest]
    public IEnumerator SetDelay_DelaysUpdateCalls()
    {
        bool updated = false;
        KTween.AddTween(m_TestObject, 0.2f, (t) => updated = true).SetDelay(0.5f);

        // 在延迟期间检查
        yield return new WaitForSeconds(0.3f);
        Assert.IsFalse(updated, "延迟期间不应调用 updateFunc");

        // 等待延迟结束 + tween 时间
        yield return new WaitForSeconds(0.5f);
        Assert.IsTrue(updated, "延迟结束后应调用 updateFunc");
    }

    /// <summary>
    /// 验证延迟 + finish: 延迟阶段 finish 不会被调用
    /// </summary>
    [UnityTest]
    public IEnumerator SetDelay_DelaysFinishFunc()
    {
        bool finished = false;
        KTween.AddTween(m_TestObject, 0.1f, null, () => finished = true).SetDelay(0.4f);

        yield return new WaitForSeconds(0.3f);
        Assert.IsFalse(finished);

        yield return new WaitForSeconds(0.4f);
        Assert.IsTrue(finished);
    }

    /// <summary>
    /// 验证对象销毁后 Tween 自动停止（bindObj 为 null 时移除）
    /// </summary>
    [UnityTest]
    public IEnumerator Tween_StopsWhenObjectDestroyed()
    {
        int callCount = 0;
        var tempGo = new GameObject("TempTweenObject");
        KTween.AddTween(tempGo, 5.0f, (t) => callCount++);

        GameObject.DestroyImmediate(tempGo);
        yield return new WaitForSeconds(0.2f);

        // 对象销毁后不应再有新调用
        int countAfterDestroy = callCount;
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(countAfterDestroy, callCount);
    }

    /// <summary>
    /// 验证取消后更新函数不再被调用
    /// </summary>
    [UnityTest]
    public IEnumerator Cancel_StopsUpdateCalls()
    {
        int callCount = 0;
        var handle = KTween.AddTween(m_TestObject, 3.0f, (t) => callCount++).GetHandle();

        yield return new WaitForSeconds(0.2f);
        int countBefore = callCount;
        Assert.Greater(countBefore, 0);

        handle.Cancel();
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(countBefore, callCount);
    }

    /// <summary>
    /// 验证 delayedCall 在指定时间后触发
    /// </summary>
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

    /// <summary>
    /// 验证循环模式下 updateFunc 持续被调用（不会在 sumTime 后停止）
    /// </summary>
    [UnityTest]
    public IEnumerator Loop_ContinuesAfterDuration()
    {
        int callCount = 0;
        KTween.AddTween(m_TestObject, 0.1f, (t) => callCount++).SetLoop(-1);

        yield return new WaitForSeconds(0.5f);

        // 0.1s 的 Tween 在 0.5s 内应该执行约 5 次循环，每次循环有多次调用
        Assert.Greater(callCount, 10);
    }

    /// <summary>
    /// 验证有限次循环在指定次数后停止
    /// </summary>
    [UnityTest]
    public IEnumerator Loop_Finite_StopsAfterCount()
    {
        int callCount = 0;
        KTween.AddTween(m_TestObject, 0.1f, (t) => callCount++).SetLoop(3);

        yield return new WaitForSeconds(0.6f);

        // 3 次循环 + 可能在非整数时间点的额外一次
        // 每次循环约 N 帧，最多约 3 * (0.1/0.02) = 15 次
        Assert.Greater(callCount, 5);
        // 验证最终确实停止了（不会无限持续）
        int countAtEnd = callCount;
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(countAtEnd, callCount);
    }

    /// <summary>
    /// 验证 PingPong 模式在时间结束时会反向
    /// </summary>
    [UnityTest]
    public IEnumerator PingPong_ReversesDirection()
    {
        float lastT = -1f;
        KTween.AddTween(m_TestObject, 0.2f, (t) => lastT = t).SetLoopPingPong(2);

        yield return new WaitForSeconds(0.25f);

        // 第一次正向结束时应接近 1
        Assert.Greater(lastT, 0.9f);

        yield return new WaitForSeconds(0.25f);

        // 反向结束时接近 0
        Assert.Less(lastT, 0.1f);
    }

    /// <summary>
    /// 验证 AppendTween 按顺序执行
    /// </summary>
    [UnityTest]
    public IEnumerator AppendTween_ExecutesInSequence()
    {
        int executionOrder = 0;
        int firstExec = 0, secondExec = 0;

        var first = KTween.AddTween(m_TestObject, 0.2f, (t) => { },
            () => { firstExec = ++executionOrder; });

        var second = KTween.AddTween(m_TestObject, 0.2f, (t) => { },
            () => { secondExec = ++executionOrder; });

        first.AppendTween(second);

        yield return new WaitForSeconds(0.7f);

        Assert.AreEqual(1, firstExec);
        Assert.AreEqual(2, secondExec);
    }

    /// <summary>
    /// 验证句柄 AppendTween 链接两个句柄
    /// </summary>
    [UnityTest]
    public IEnumerator Handle_AppendTween_Sequences()
    {
        int order = 0;
        int first = 0, second = 0;

        var h1 = KTween.AddTween(m_TestObject, 0.2f, null,
            () => first = ++order).GetHandle();
        var h2 = KTween.AddTween(m_TestObject, 0.2f, null,
            () => second = ++order).GetHandle();

        h1.AppendTween(h2);

        yield return new WaitForSeconds(0.7f);

        Assert.AreEqual(1, first);
        Assert.AreEqual(2, second);
    }

    /// <summary>
    /// 验证同时运行多个 Tween 互不干扰
    /// </summary>
    [UnityTest]
    public IEnumerator MultipleTweens_RunIndependently()
    {
        float pos1 = 0f, pos2 = 0f;
        var go1 = new GameObject("TweenTarget1");
        var go2 = new GameObject("TweenTarget2");

        KTween.AddTween(go1, 0.3f, (t) => pos1 = t, () => pos1 = 1f);
        KTween.AddTween(go2, 0.6f, (t) => pos2 = t, () => pos2 = 1f);

        yield return new WaitForSeconds(0.4f);

        // go1 应该已完成，go2 应该还在进行中
        Assert.AreEqual(1f, pos1, 0.05f);
        Assert.Greater(pos2, 0.5f);
        Assert.Less(pos2, 1f);

        yield return new WaitForSeconds(0.3f);
        Assert.AreEqual(1f, pos2, 0.05f);

        GameObject.DestroyImmediate(go1);
        GameObject.DestroyImmediate(go2);
    }

    /// <summary>
    /// 验证延迟 0 的 Tween 立即开始
    /// </summary>
    [UnityTest]
    public IEnumerator ZeroDelay_StartsImmediately()
    {
        bool started = false;
        KTween.AddTween(m_TestObject, 0.1f, (t) => { if (t > 0) started = true; });

        yield return new WaitForSeconds(0.05f);
        Assert.IsTrue(started);
    }

    /// <summary>
    /// 验证大量 Tween 同时运行时性能正常（无崩溃/异常）
    /// </summary>
    [UnityTest]
    public IEnumerator ManyTweens_DoesNotCrash()
    {
        var gos = new GameObject[500];
        for (int i = 0; i < 500; i++)
        {
            gos[i] = new GameObject($"BulkTween_{i}");
            KTween.AddTween(gos[i], UnityEngine.Random.Range(0.1f, 0.5f), (t) => { });
        }

        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < 500; i++)
            GameObject.DestroyImmediate(gos[i]);
    }

    /// <summary>
    /// 验证 cancel 在链式 Tween 上正确停止所有后续
    /// </summary>
    [UnityTest]
    public IEnumerator CancelChain_StopsAllLinked()
    {
        int calls = 0;
        var a = KTween.AddTween(m_TestObject, 5.0f, (t) => calls++);
        var b = KTween.AddTween(m_TestObject, 5.0f, (t) => calls++);
        var c = KTween.AddTween(m_TestObject, 5.0f, (t) => calls++);
        a.AppendTween(b);
        b.AppendTween(c);

        yield return new WaitForSeconds(0.1f);

        a.cancel();

        int callsAfterCancel = calls;
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(callsAfterCancel, calls);
    }
}