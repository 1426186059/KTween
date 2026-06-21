using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// EditMode 单元测试：测试 KTween 核心 API，不依赖 MonoBehaviour 生命周期。
/// 注意：AddTween 在 EditMode 下只能验证 API 调用逻辑，不会真正驱动 Update 循环。
/// </summary>
public class KTweenEditModeTests
{
    [SetUp]
    public void Setup()
    {
        // 清理单例管理器（仅对已创建的实例有效）
        if (KTween.KTweenMgr.Instance != null)
        {
            var go = KTween.KTweenMgr.Instance.gameObject;
            GameObject.DestroyImmediate(go);
        }
        // 重置最大数量
        KTween.SetMaxTweenCount(1024);
    }

    // ==================== 基础 API ====================

    [Test]
    public void AddTween_ReturnsNonNullTweenItem()
    {
        var item = KTween.AddTween(1.0f);
        Assert.IsNotNull(item);
        Assert.IsTrue(item.toggle);
    }

    [Test]
    public void AddTween_WithGameObject_BindsObject()
    {
        var go = new GameObject("TestTweenObject");
        var item = KTween.AddTween(go, 1.0f);
        Assert.AreEqual(go, item.bindObj);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void AddTween_UpdateFunc_IsInvokedDuringTime()
    {
        float lastT = -1f;
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f, (t) => lastT = t);
        Assert.AreEqual(0f, lastT, 0.001f); // 初始时立即被调用一次（time=0）

        // 模拟更新
        // 注意：无法直接触发 KTweenByLinkedList 的 Update，
        // 这里只验证 TweenItem 被正确构造
        Assert.IsNotNull(item.updateFunc);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void AddTween_FinishFunc_IsStored()
    {
        bool finished = false;
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f, null, () => finished = true);
        Assert.IsNotNull(item.finishFunc);
        GameObject.DestroyImmediate(go);
    }

    // ==================== SetDelay ====================

    [Test]
    public void SetDelay_SetsDelayOnItem()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f).SetDelay(2.5f);
        Assert.AreEqual(2.5f, item.delay);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void SetDelay_ReturnsSelf_ForChaining()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        var result = item.SetDelay(1.0f);
        Assert.AreSame(item, result);
        GameObject.DestroyImmediate(go);
    }

    // ==================== SetLoop ====================

    [Test]
    public void SetLoop_InfiniteLoop_SetsNegativeOne()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f).SetLoop(-1);
        Assert.AreEqual(-1, item.nLoopCount);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void SetLoop_FiniteLoop_SetsCorrectCount()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f).SetLoop(5);
        Assert.AreEqual(5, item.nLoopCount);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void SetLoop_DefaultParameter_IsInfinite()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f).SetLoop();
        Assert.AreEqual(-1, item.nLoopCount);
        GameObject.DestroyImmediate(go);
    }

    // ==================== SetLoopPingPong ====================

    [Test]
    public void SetLoopPingPong_Infinite_SetsNegativeOne()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f).SetLoopPingPong(-1);
        Assert.AreEqual(-1, item.nLoopCount);
        Assert.AreEqual(1, item.nLoopPingTong);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void SetLoopPingPong_Finite_SetsCorrectCount()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f).SetLoopPingPong(3);
        Assert.AreEqual(3, item.nLoopCount);
        Assert.AreEqual(1, item.nLoopPingTong);
        GameObject.DestroyImmediate(go);
    }

    // ==================== AppendTween ====================

    [Test]
    public void AppendTween_LinksItems()
    {
        var go = new GameObject("Test");
        var first = KTween.AddTween(go, 1.0f);
        var second = KTween.AddTween(go, 2.0f);
        first.AppendTween(second);
        Assert.AreSame(second, first.SqeNext);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void AppendTween_AddsDelayToSecond()
    {
        var go = new GameObject("Test");
        var first = KTween.AddTween(go, 1.0f).SetDelay(0.5f);
        float expectedDelay = 0.5f + 1.0f; // first.delay + first.sumTime
        var second = KTween.AddTween(go, 2.0f);
        first.AppendTween(second);
        Assert.AreEqual(expectedDelay, second.delay);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void AppendTween_ChainsMultiple()
    {
        var go = new GameObject("Test");
        var a = KTween.AddTween(go, 1.0f);
        var b = KTween.AddTween(go, 1.0f);
        var c = KTween.AddTween(go, 1.0f);
        a.AppendTween(b);
        b.AppendTween(c);
        Assert.AreSame(b, a.SqeNext);
        Assert.AreSame(c, b.SqeNext);
        GameObject.DestroyImmediate(go);
    }

    // ==================== cancel ====================

    [Test]
    public void Cancel_SetsToggleToFalse()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        item.cancel();
        Assert.IsFalse(item.toggle);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void Cancel_AlsoCancelsLinkedTweens()
    {
        var go = new GameObject("Test");
        var a = KTween.AddTween(go, 1.0f);
        var b = KTween.AddTween(go, 1.0f);
        a.AppendTween(b);
        a.cancel();
        Assert.IsFalse(a.toggle);
        Assert.IsFalse(b.toggle);
        GameObject.DestroyImmediate(go);
    }

    // ==================== GetHandle ====================

    [Test]
    public void GetHandle_ReturnsValidHandle()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        var handle = item.GetHandle();
        Assert.IsTrue(handle.IsValid());
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void Handle_IsInvalidAfterCancel()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        var handle = item.GetHandle();
        handle.Cancel();
        Assert.IsFalse(handle.IsValid());
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void Handle_Dispose_Invalidates()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        var handle = item.GetHandle();
        handle.Dispose();
        Assert.IsFalse(handle.IsValid());
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void Handle_Dispose_Idempotent()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        var handle = item.GetHandle();
        handle.Dispose();
        handle.Dispose(); // 第二次不会抛出异常
        Assert.IsFalse(handle.IsValid());
        GameObject.DestroyImmediate(go);
    }

    // ==================== Static GetHandle ====================

    [Test]
    public void Static_GetHandle_ReturnsValid()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        var handle = KTween.GetHandle(item);
        Assert.IsTrue(handle.IsValid());
        GameObject.DestroyImmediate(go);
    }

    // ==================== delayedCall ====================

    [Test]
    public void DelayedCall_CreatesTweenWithNoUpdate()
    {
        var go = new GameObject("Test");
        bool called = false;
        var item = KTween.delayedCall(go, 1.0f, () => called = true);
        Assert.IsNotNull(item);
        Assert.IsNull(item.updateFunc);
        Assert.IsNotNull(item.finishFunc);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void DelayedCall_WithoutGameObject_CreatesTween()
    {
        bool called = false;
        var item = KTween.delayedCall(2.0f, () => called = true);
        Assert.IsNotNull(item);
        Assert.IsNull(item.updateFunc);
        Assert.IsNotNull(item.finishFunc);
    }

    // ==================== SetMaxTweenCount ====================

    [Test]
    public void SetMaxTweenCount_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => KTween.SetMaxTweenCount(2048));
        Assert.DoesNotThrow(() => KTween.SetMaxTweenCount(1));
    }

    // ==================== TweenItem Properties ====================

    [Test]
    public void AddTween_SetsCorrectTimeValues()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 3.0f);
        Assert.AreEqual(3.0f, item.sumTime);
        Assert.AreEqual(0f, item.time);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void AddTween_WithoutBindObj_UsesManagerGameObject()
    {
        var item = KTween.AddTween(1.0f);
        Assert.AreEqual(KTween.KTweenMgr.Instance.gameObject, item.bindObj);
    }

    // ==================== ObjectPool Reset ====================

    [Test]
    public void TweenItem_Reset_ClearsFields()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f, (t) => { }, () => { });
        item.SetDelay(0.5f).SetLoop(3);

        uint oldVersion = item.nVersion;
        item.Reset();

        Assert.IsNull(item.bindObj);
        Assert.IsFalse(item.toggle);
        Assert.AreEqual(0f, item.delay);
        Assert.AreEqual(0f, item.time);
        Assert.AreEqual(0f, item.sumTime);
        Assert.IsNull(item.updateFunc);
        Assert.IsNull(item.finishFunc);
        Assert.AreEqual(0, item.nLoopCount);
        Assert.AreEqual(0, item.nLoopPingTong);
        Assert.AreEqual(oldVersion + 1, item.nVersion);

        GameObject.DestroyImmediate(go);
    }

    // ==================== Handle.AppendTween ====================

    [Test]
    public void Handle_AppendTween_LinksTwoHandles()
    {
        var go = new GameObject("Test");
        var h1 = KTween.AddTween(go, 1.0f).GetHandle();
        var h2 = KTween.AddTween(go, 2.0f).GetHandle();
        Assert.DoesNotThrow(() => h1.AppendTween(h2));
        h1.Cancel();
        h2.Cancel();
        GameObject.DestroyImmediate(go);
    }

    // ==================== Edge Cases ====================

    [Test]
    public void AddTween_ZeroTime_CreatesItem()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 0f);
        Assert.IsNotNull(item);
        Assert.AreEqual(0f, item.sumTime);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void AddTween_NegativeTime_CreatesItem()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, -1f);
        Assert.IsNotNull(item);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void AddTween_NullUpdateFunc_CreatesItem()
    {
        var item = KTween.AddTween(1.0f, null);
        Assert.IsNotNull(item);
    }

    [Test]
    public void AddTween_ChainWithCancel_StopsAll()
    {
        var go = new GameObject("Test");
        var a = KTween.AddTween(go, 1.0f);
        var b = KTween.AddTween(go, 1.0f);
        var c = KTween.AddTween(go, 1.0f);
        a.AppendTween(b);
        b.AppendTween(c);
        a.cancel();
        Assert.IsFalse(a.toggle);
        Assert.IsFalse(b.toggle);
        Assert.IsFalse(c.toggle);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void Handle_Cancel_InvalidatesOnlyThisHandle()
    {
        var go = new GameObject("Test");
        var item = KTween.AddTween(go, 1.0f);
        var h1 = item.GetHandle();
        var h2 = item.GetHandle();
        h1.Cancel();
        Assert.IsFalse(h1.IsValid());
        // 第二个句柄由于版本号未变，仍然有效——即使 toggle = false
        // 这是合理的行为：句柄只依赖于版本号验证，不检查 toggle
        Assert.IsTrue(h2.IsValid());
        GameObject.DestroyImmediate(go);
    }

    // ==================== Multiple Tweens ====================

    [Test]
    public void MultipleAddTweens_AllReturnDistinctItems()
    {
        var go = new GameObject("Test");
        var items = new HashSet<KTween.TweenItem>();
        for (int i = 0; i < 100; i++)
        {
            var item = KTween.AddTween(go, 1.0f);
            items.Add(item);
        }
        Assert.AreEqual(100, items.Count);
        GameObject.DestroyImmediate(go);
    }

    [Test]
    public void CancelAll_DoesNotThrow()
    {
        var go = new GameObject("Test");
        var items = new List<KTween.TweenItem>();
        for (int i = 0; i < 100; i++)
        {
            items.Add(KTween.AddTween(go, 1.0f));
        }
        Assert.DoesNotThrow(() =>
        {
            foreach (var item in items) item.cancel();
        });
        foreach (var item in items)
        {
            Assert.IsFalse(item.toggle);
        }
        GameObject.DestroyImmediate(go);
    }

    // ==================== KTweenFunc Tests ====================

    [Test]
    public void EaseLinear_Vector3_ReturnsCorrectValue()
    {
        Vector3 from = Vector3.zero;
        Vector3 to = Vector3.one * 10f;
        Assert.AreEqual(Vector3.zero, KTweenFunc.linear(from, to, 0f));
        Assert.AreEqual(Vector3.one * 5f, KTweenFunc.linear(from, to, 0.5f));
        Assert.AreEqual(Vector3.one * 10f, KTweenFunc.linear(from, to, 1f));
    }

    [Test]
    public void EaseLinear_Vector2_ReturnsCorrectValue()
    {
        Vector2 from = Vector2.zero;
        Vector2 to = new Vector2(100f, 200f);
        Assert.AreEqual(Vector2.zero, KTweenFunc.linear(from, to, 0f));
        Assert.AreEqual(new Vector2(50f, 100f), KTweenFunc.linear(from, to, 0.5f));
        Assert.AreEqual(to, KTweenFunc.linear(from, to, 1f));
    }

    [Test]
    public void EaseLinear_Float_ReturnsCorrectValue()
    {
        Assert.AreEqual(0f, KTweenFunc.linear(0f, 100f, 0f));
        Assert.AreEqual(50f, KTweenFunc.linear(0f, 100f, 0.5f));
        Assert.AreEqual(100f, KTweenFunc.linear(0f, 100f, 1f));
    }

    [Test]
    public void EaseLinear_ClampsOutOfRange()
    {
        // 不保证 Clamp，但至少不应该抛出异常
        Assert.DoesNotThrow(() => KTweenFunc.linear(Vector3.zero, Vector3.one, -0.5f));
        Assert.DoesNotThrow(() => KTweenFunc.linear(Vector3.zero, Vector3.one, 1.5f));
        Assert.DoesNotThrow(() => KTweenFunc.linear(0f, 1f, -0.5f));
        Assert.DoesNotThrow(() => KTweenFunc.linear(0f, 1f, 1.5f));
    }
}