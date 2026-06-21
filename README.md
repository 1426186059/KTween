# KTween

**多引擎 Tween 动画库** — 目前已支持 Unity / UE5，未来可扩展至 Laya、Cocos、Godot 等任意引擎。

## 设计目标

KTween 的核心定位是 **一套 API，多引擎通用**。引擎适配层（`KTweenByLinkedList` / `KTweenByList`）与引擎解耦，移植到新引擎只需实现 2 个接口：

1. **更新驱动** — 每帧调用 `KTweenByLinkedList::Update(dt)`
2. **材质/变换 API** — 根据引擎特性实现 `KTweenEx` 扩展方法

`KTweenFunc`（缓动函数）、`KTweenItem`（动画项）、`Handle`（安全句柄）等核心全是纯逻辑，零引擎依赖。

## 特性

- **跨引擎 API 一致**：Unity C# 与 UE5 C++ 的调用方式完全相同
- **30+ 缓动函数**：LeanTween 命名风格，`ApplyEase(enum, percent) → percent` 统一入口
- **双驱动模式**：双向链表（高插入删除） / TArray（低内存碎片），预处理宏一键切换
- **Handle 安全句柄**：版本号检测，避免操作已过期的动画对象
- **链式序列**：基于延迟偏移的 `AppendTween` 动画编排
- **自动生命周期**：绑定对象销毁时自动停止动画
- **对象池零分配**：TweenItem 完全重用，运行时无 GC

## 支持引擎

| 引擎 | 语言 | 状态 | 位置 |
|------|------|------|------|
| **Unity** | C# | ✅ 已支持 | `Unity.KTween/` |
| **UE5** | C++ | ✅ 已支持（插件） | `UE5.KTween/Plugins/KTween/` |
| LayaAir | TypeScript | 🔜 规划中 | — |
| Cocos Creator | TypeScript | 🔜 规划中 | — |
| Godot | C# / GDScript | 🔜 规划中 | — |
| 小程序 | JavaScript | 🔜 规划中 | — |

## 快速开始

### Unity

```csharp
// 基础移动 — 链式调用：缓动 + 往返循环
Vector3 from = transform.position;
Vector3 to = from + Vector3.right * 5f;

KTween.AddTween(gameObject, 2.0f, (t) =>
{
    transform.position = Vector3.Lerp(from, to, t);
}).SetEase(KTweenType.easeOutBounce).SetLoopPingPong(-1);
```

### UE5

```cpp
// 基础移动 — 与 Unity 完全一致的链式调用
FVector From = Actor->GetActorLocation();
FVector To = From + FVector(0, 0, 400);

KTween::AddTween(Actor, 2.0f, [From, To](float T)
{
    Actor->SetActorLocation(FMath::Lerp(From, To, T));
})->SetEase(KTween::EaseType::easeOutBounce)->SetLoopPingPong(-1);
```

## API 参考

### 核心方法

| 方法 | 说明 | Unity | UE5 |
|------|------|-------|-----|
| `AddTween` | 添加动画 | `KTween.AddTween(obj, time, update)` | `KTween::AddTween(obj, time, update)` |
| `delayedCall` | 延迟回调 | `KTween.delayedCall(time, finish)` | `KTween::delayedCall(time, finish)` |
| `Cancel` | 取消动画 | `KTween.Cancel(obj/handle)` | `KTween::Cancel(obj/handle)` |
| `CancelAll` | 取消全部 | `KTween.CancelAll()` | `KTween::CancelAll()` |
| `GetHandle` | 安全句柄 | `item.GetHandle()` | `KTween::GetHandle(item)` |
| `GetEaseFunc` | 获取缓动函数 | `KTweenFunc.ApplyEase(type, pct)` | `KTween::EaseFunc::ApplyEase(type, pct)` |

### 链式修饰

所有修饰方法返回 `TweenItem` 自身，支持链式调用：

```csharp
// Unity
KTween.AddTween(obj, time, update)
    .SetEase(KTweenType.easeOutBounce)
    .SetDelay(0.5f)
    .SetLoopPingPong(-1)
    .SetOnCompleteFunc(() => Debug.Log("Done!"));
```

```cpp
// UE5
KTween::AddTween(Actor, time, update)
    ->SetEase(KTween::EaseType::easeOutBounce)
    ->SetDelay(0.5f)
    ->SetLoopPingPong(-1)
    ->SetOnCompleteFunc([]() { UE_LOG(LogTemp, Log, TEXT("Done!")); });
```

| 方法 | 说明 | 参数 |
|------|------|------|
| `SetEase(type)` | 缓动类型 | `KTweenType.xxx` / `KTween::EaseType::xxx` |
| `SetDelay(sec)` | 延迟启动 | float |
| `SetLoop(count)` | 循环次数 | -1=无限，n=次数 |
| `SetLoopPingPong(count)` | 往返循环 | -1=无限，n=次数 |
| `SetOnStartFunc(func)` | 启动回调 | `Action` |
| `SetOnCompleteFunc(func)` | 完成回调 | `Action` |
| `AppendTween(item/handle)` | 链式序列 | `TweenItem` / `Handle` |

### Handle 安全句柄

Handle 通过版本号检测过期，避免操作已取消或已回收的 Tween：

```csharp
// Unity
KTween.Handle seq = new KTween.Handle();
seq.AppendTween(KTween.AddTween(obj, 1.0f, update1));
seq.AppendTween(KTween.AddTween(obj, 1.0f, update2));
// 3 秒后取消整个序列
KTween.delayedCall(3.0f, () => seq.Cancel());
```

```cpp
// UE5
KTween::Handle Seq;
Seq.AppendTween(KTween::AddTween(Actor, 1.0f, Update1));
Seq.AppendTween(KTween::AddTween(Actor, 1.0f, Update2));
```

### 缓动类型（共 34 种）

所有缓动通过 `ApplyEase(enum, float) → float` 统一入口调用。

| 分类 | 缓入 (In) | 缓出 (Out) | 缓入出 (InOut) |
|------|-----------|------------|----------------|
| **线性** | `linear` | — | — |
| **二次 (Quad)** | `easeInQuad` | `easeOutQuad` | `easeInOutQuad` |
| **三次 (Cubic)** | `easeInCubic` | `easeOutCubic` | `easeInOutCubic` |
| **四次 (Quart)** | `easeInQuart` | `easeOutQuart` | `easeInOutQuart` |
| **五次 (Quint)** | `easeInQuint` | `easeOutQuint` | `easeInOutQuint` |
| **正弦 (Sine)** | `easeInSine` | `easeOutSine` | `easeInOutSine` |
| **指数 (Expo)** | `easeInExpo` | `easeOutExpo` | `easeInOutExpo` |
| **圆形 (Circ)** | `easeInCirc` | `easeOutCirc` | `easeInOutCirc` |
| **弹回 (Bounce)** | `easeInBounce` | `easeOutBounce` | `easeInOutBounce` |
| **回退 (Back)** | `easeInBack` | `easeOutBack` | `easeInOutBack` |
| **弹性 (Elastic)** | `easeInElastic` | `easeOutElastic` | `easeInOutElastic` |
| **特殊** | `easeSpring` | `easeShake` | `punch` |

```csharp
// Unity — 直接使用缓动值
float eased = KTweenFunc.ApplyEase(KTweenType.easeOutBounce, 0.5f);

// 或让 SetEase 自动处理（推荐）
KTween.AddTween(obj, time, (t) => { /* t 已经是缓动后的值 */ })
    .SetEase(KTweenType.easeOutBounce);
```

### 扩展方法（KTweenEx）

为常用属性提供快捷方法：

```csharp
// Unity
KTweenEx.move(gameObject, to, time).SetEase(KTweenType.easeOutQuad);
KTweenEx.moveX(gameObject, x, time);
KTweenEx.moveY(gameObject, y, time);
KTweenEx.moveZ(gameObject, z, time);
KTweenEx.moveLocal(gameObject, to, time);
KTweenEx.scale(gameObject, to, time);
KTweenEx.rotateAround(gameObject, axis, angle, time);
KTweenEx.rotateAroundLocal(gameObject, axis, angle, time);
KTweenEx.color(gameObject, to, time);
KTweenEx.alpha(gameObject, to, time);
KTweenEx.alphaCanvas(canvasGroup, to, time);
```

```cpp
// UE5（UMG 组件）
KTweenEx::UMG_MoveLocal_RenderPos(Widget, To, Time)->SetEase(KTween::EaseType::easeOutQuad);
KTweenEx::UMG_MoveLocal_SlotPos(Widget, To, Time);
KTweenEx::UMG_Opacity(Widget, To, Time);
KTweenEx::UMG_Scale(Widget, To, Time);
```

## 项目结构

### Unity

```
Unity.KTween/Assets/KTween/
├── Runtime/                          # 核心运行时（跨引擎可移植）
│   ├── KTween.cs                     # 主 API + Handle + TweenItem + 对象池
│   ├── KTweenFunc.cs                 # 缓动函数核心（零引擎依赖）
│   ├── KTweenEx.cs                   # 引擎扩展方法（Transform / UI）
│   ├── KTweenByLinkedList.cs         # 双向链表驱动
│   └── KTweenByList.cs               # 数组驱动
├── Editor/
│   └── KTweenMgrEditor.cs
├── Examples/                         # 13 个示例脚本
│   ├── Example_AllInOne.cs           # 完整 API 演示
│   ├── Example_ExDemos.cs            # KTweenEx 扩展演示
│   └── ...
└── Tests/
    ├── EditMode/
    └── PlayMode/
```

### UE5 插件

```
UE5.KTween/Plugins/KTween/
├── KTween.uplugin                    # 插件描述
└── Source/KTween/
    ├── KTweenModule.h/cpp            # 模块入口
    └── Public/                       # 核心运行时（跨引擎可移植）
        ├── KTween.h                  # 主 API
        ├── KTweenFunc.h              # 缓动函数核心（零引擎依赖）
        ├── KTweenEx.h                # UMG 扩展方法
        └── KTweenHead.h              # 数据结构 + AKTweenMgr
        └── Private/
            ├── KTweenByLinkedList.cpp # 链表驱动
            └── KTweenMgr.cpp          # 引擎适配层
```

> **跨引擎移植提示**：`KTweenFunc`（缓动函数）、`KTweenItem`（动画项）、`Handle`（安全句柄）、`ObjectPool`（对象池）完全不依赖引擎 API，可直接移植到 Laya / Cocos / Godot 等平台。只需适配更新循环和渲染对象 API。

## 性能数据

| 场景 | 物体数 | FPS | GC Alloc |
|------|--------|-----|----------|
| 100+ 物体同步动画 | 150 | 60 稳定 | 0 |
| 200+ 物体混合动画 | 234 | 60 稳定 | 0 |

## 编译要求

| 引擎 | 版本 | 备注 |
|------|------|------|
| Unity | 2020.3+ | .NET Standard 2.0 |
| UE5 | 5.3+ | 作为插件引入，`Build.cs` 添加 `"KTween"` 依赖 |

## 从 LeanTween 迁移

| LeanTween | KTween |
|-----------|--------|
| `LeanTween.move(obj, to, time)` | `KTweenEx.move(obj, to, time)` |
| `.setEase(LeanTweenType.easeOutBounce)` | `.SetEase(KTweenType.easeOutBounce)` |
| `.setDelay(0.5f)` | `.SetDelay(0.5f)` |
| `.loopCount = -1` | `.SetLoop(-1)` |
| `.setOnComplete(func)` | `.SetOnCompleteFunc(func)` |
| `LTDescr` 句柄 | `KTween.Handle` / `KTween::Handle` |

## 许可证

MIT
