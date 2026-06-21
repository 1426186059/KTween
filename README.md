# KTween

跨引擎（Unity / UE5）、跨语言（C# / C++）、跨平台，高性能易拓展的 Tween 动画库。

## 特性

- 轻量级：仅 6 个核心源文件，无第三方依赖
- 高性能：双向链表驱动（可用 TArray/LinkedList 切换），对象池重用
- 跨引擎同 API：Unity C# 与 UE5 C++ 接口完全一致
- 30+ 缓动函数：来自 LeanTween 风格命名，与标准缓动曲线对齐
- 链式调用：`AddTween(...)->SetEase(...)->SetLoop(...)` 
- Handle 安全句柄：可检测 Tween 是否过期，支持序列化动画组
- 自动生命周期管理：绑定对象销毁时自动停止
- 对象池零分配：TweenItem 重用，避免 GC

## 快速开始

### Unity

```csharp
// 基础移动
Vector3 from = transform.position;
Vector3 to = from + Vector3.right * 5f;

KTween.AddTween(gameObject, 2.0f, (t) =>
{
    transform.position = Vector3.Lerp(from, to, t);
}).SetEase(KTweenType.easeOutBounce).SetLoopPingPong(-1);
```

### UE5

```cpp
// 基础移动
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
| AddTween | 添加动画 | `KTween.AddTween(obj, time, update)` | `KTween::AddTween(obj, time, update)` |
| delayedCall | 延迟回调 | `KTween.delayedCall(time, finish)` | `KTween::delayedCall(time, finish)` |
| Cancel | 取消动画 | `KTween.Cancel(obj/handle)` | `KTween::Cancel(obj/handle)` |
| CancelAll | 取消全部 | `KTween.CancelAll()` | `KTween::CancelAll()` |
| GetHandle | 获取安全句柄 | `item.GetHandle()` | `KTween::GetHandle(item)` |

### 链式修饰

| 方法 | 说明 | 示例 |
|------|------|------|
| SetEase(type) | 设置缓动类型 | `.SetEase(KTweenType.easeOutBounce)` |
| SetDelay(sec) | 延迟启动 | `.SetDelay(0.5f)` |
| SetLoop(count) | 循环次数(-1 无限) | `.SetLoop(5)` |
| SetLoopPingPong(count) | 往返循环 | `.SetLoopPingPong(-1)` |
| SetOnComplete(func) | 完成回调 | `.SetOnCompleteFunc(() => ...)` |
| AppendTween(item) | 链式序列 | `t1.AppendTween(t2)` |

### Handle 序列

```csharp
// Unity
KTween.Handle seq = new KTween.Handle();
seq.AppendTween(KTween.AddTween(obj, 1.0f, (t) => { ... }));
seq.AppendTween(KTween.AddTween(obj, 1.0f, (t) => { ... }));
```

```cpp
// UE5
KTween::Handle Seq;
Seq.AppendTween(KTween::AddTween(Actor, 1.0f, [](float T) { ... }));
Seq.AppendTween(KTween::AddTween(Actor, 1.0f, [](float T) { ... }));
```

### 缓动类型（LeanTween 命名风格）

| 线性 | 二次 | 三次 | 四次 | 五次 | 正弦 | 指数 | 圆形 | 弹回 | 回退 | 弹性 | 特殊 |
|------|------|------|------|------|------|------|------|------|------|------|------|
| linear | easeOutQuad | easeOutCubic | easeOutQuart | easeOutQuint | easeOutSine | easeOutExpo | easeOutCirc | easeOutBounce | easeOutBack | easeOutElastic | easeSpring |
| | easeInQuad | easeInCubic | easeInQuart | easeInQuint | easeInSine | easeInExpo | easeInCirc | easeInBounce | easeInBack | easeInElastic | easeShake |
| | easeInOutQuad | easeInOutCubic | easeInOutQuart | easeInOutQuint | easeInOutSine | easeInOutExpo | easeInOutCirc | easeInOutBounce | easeInOutBack | easeInOutElastic | punch |

### 扩展方法（KTweenEx）

```csharp
// Unity
KTweenEx.move(obj, to, time).SetEase(KTweenType.easeOutQuad);
KTweenEx.moveX(obj, x, time);
KTweenEx.scale(obj, to, time);
KTweenEx.rotateAround(obj, axis, angle, time);
KTweenEx.color(obj, to, time);
KTweenEx.alpha(obj, to, time);
```

```cpp
// UE5 (UMG 组件)
KTweenEx::UMG_MoveLocal_RenderPos(Widget, to, time)->SetEase(KTween::EaseType::easeOutQuad);
KTweenEx::UMG_Opacity(Widget, to, time);
KTweenEx::UMG_Scale(Widget, to, time);
```

## 项目结构

### Unity (`Unity.KTween/`)

```
Assets/KTween/
├── Runtime/
│   ├── KTween.cs          # 主 API + Handle + TweenItem + 对象池
│   ├── KTweenFunc.cs      # 缓动函数（ApplyEase 核心）
│   ├── KTweenEx.cs        # 扩展方法
│   ├── KTweenByLinkedList.cs  # 链表驱动
│   └── KTweenByList.cs    # 数组驱动
├── Editor/
│   └── KTweenMgrEditor.cs
├── Examples/              # 13 个示例脚本
└── Tests/                 # 单元测试
```

### UE5 插件 (`Plugins/KTween/`)

```
Source/KTween/
├── KTweenModule.h/cpp     # 模块入口
├── Public/
│   ├── KTween.h           # 主 API
│   ├── KTweenFunc.h       # 缓动函数（ApplyEase 核心）
│   ├── KTweenEx.h         # UMG 扩展方法
│   └── KTweenHead.h       # 数据结构 + AKTweenMgr
└── Private/
    ├── KTweenByLinkedList.cpp
    └── KTweenMgr.cpp
```

## 从 LeanTween 迁移

| LeanTween | KTween |
|-----------|--------|
| `LeanTween.move(obj, to, time)` | `KTweenEx.move(obj, to, time)` |
| `.setEase(LeanTweenType.easeOutBounce)` | `.SetEase(KTweenType.easeOutBounce)` |
| `.setDelay(0.5f)` | `.SetDelay(0.5f)` |
| `.loopCount = -1` | `.SetLoop(-1)` |
| `.setOnComplete(func)` | `.SetOnCompleteFunc(func)` |
| `LTDescr` 句柄 | `KTween.Handle` / `KTween::Handle` |

## 编译要求

- **Unity**: 2020.3+（.NET Standard 2.0）
- **UE5**: UE 5.3+（C++17）
