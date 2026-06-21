using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 示例场景主控制器：运行时生成可视化演示场景，展示所有 KTween 用法
/// </summary>
public class ExampleRunner : MonoBehaviour
{
    [Header("场景设置")]
    public float spacingX = 4.5f;
    public float spacingZ = 4.5f;
    public float startX = -8f;
    public float startZ = 8f;
    public bool autoGenerate = true;

    private readonly List<GameObject> m_SpawnedObjects = new List<GameObject>();

    // ============================================================
    // 示例注册表：每个示例的创建信息
    // ============================================================

    private struct DemoEntry
    {
        public string Name;
        public System.Action<GameObject, Vector3> CreateAction;
    }

    private List<DemoEntry> GetDemos()
    {
        return new List<DemoEntry>
        {
            new DemoEntry { Name = "01 - 基础移动", CreateAction = CreateDemo_BasicMove },
            new DemoEntry { Name = "02 - 旋转动画", CreateAction = CreateDemo_Rotation },
            new DemoEntry { Name = "03 - 缩放脉冲", CreateAction = CreateDemo_ScalePulse },
            new DemoEntry { Name = "04 - 颜色渐变动画", CreateAction = CreateDemo_ColorFade },
            new DemoEntry { Name = "05 - 路径运动 + 序列", CreateAction = CreateDemo_PathSequence },
            new DemoEntry { Name = "06 - 循环模式 (PingPong)", CreateAction = CreateDemo_LoopPingPong },
            new DemoEntry { Name = "07 - 循环模式 (Normal)", CreateAction = CreateDemo_LoopNormal },
            new DemoEntry { Name = "08 - Cancel/Handle", CreateAction = CreateDemo_CancelHandle },
            new DemoEntry { Name = "09 - DelayedCall", CreateAction = CreateDemo_DelayedCall },
            new DemoEntry { Name = "10 - 多物体同时 Tween", CreateAction = CreateDemo_MultiObject },
            new DemoEntry { Name = "11 - 缓动函数对比", CreateAction = CreateDemo_EasingComparison },
            new DemoEntry { Name = "12 - 弹跳效果", CreateAction = CreateDemo_Bounce },
            new DemoEntry { Name = "13 - UI 淡入淡出", CreateAction = CreateDemo_UI_Fade },
            new DemoEntry { Name = "14 - 组合动画", CreateAction = CreateDemo_Combined },
        };
    }

    // ============================================================
    // MonoBehaviour Lifecycle
    // ============================================================

    private void Start()
    {
        if (Application.isPlaying && autoGenerate)
        {
            GenerateAllDemos();
        }
    }

    [ContextMenu("重新生成所有示例")]
    public void Regenerate()
    {
        ClearGenerated();
        GenerateAllDemos();
    }

    [ContextMenu("清除所有示例")]
    public void ClearGenerated()
    {
        for (int i = m_SpawnedObjects.Count - 1; i >= 0; i--)
        {
            if (m_SpawnedObjects[i] != null)
            {
                if (Application.isPlaying)
                    Destroy(m_SpawnedObjects[i]);
                else
                    DestroyImmediate(m_SpawnedObjects[i]);
            }
        }
        m_SpawnedObjects.Clear();
    }

    // ============================================================
    // 生成所有示例
    // ============================================================

    private void GenerateAllDemos()
    {
        var demos = GetDemos();
        int cols = Mathf.CeilToInt(Mathf.Sqrt(demos.Count));
        int rows = Mathf.CeilToInt((float)demos.Count / cols);

        for (int i = 0; i < demos.Count; i++)
        {
            int col = i % cols;
            int row = i / cols;
            Vector3 pos = new Vector3(startX + col * spacingX, 0f, startZ - row * spacingZ);

            var demoGo = new GameObject(demos[i].Name);
            demoGo.transform.position = pos;

            var label = demoGo.AddComponent<ExampleLabel>();
            label.text = demos[i].Name;

            demos[i].CreateAction(demoGo, pos);

            m_SpawnedObjects.Add(demoGo);
        }
    }

    // ============================================================
    // 单个示例创建函数
    // ============================================================

    private void CreateDemo_BasicMove(GameObject parent, Vector3 origin)
    {
        var cube = CreatePrimitive(parent, PrimitiveType.Cube, origin, Color.cyan);
        cube.AddComponent<Example_01_BasicMove>();
    }

    private void CreateDemo_Rotation(GameObject parent, Vector3 origin)
    {
        var cube = CreatePrimitive(parent, PrimitiveType.Cube, origin, new Color(0f, 0.8f, 1f));
        // 使用 Transform 直接旋转
        cube.AddComponent<Example_02_Rotation>();
    }

    private void CreateDemo_ScalePulse(GameObject parent, Vector3 origin)
    {
        var sphere = CreatePrimitive(parent, PrimitiveType.Sphere, origin, Color.magenta);
        sphere.AddComponent<Example_03_ScalePulse>();
    }

    private void CreateDemo_ColorFade(GameObject parent, Vector3 origin)
    {
        var go = new GameObject("ColorFadeElement");
        go.transform.SetParent(parent.transform);
        go.transform.position = origin;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateDebugSprite(64, 64, Color.white, Color.red);
        sr.color = Color.red;
        go.AddComponent<Example_04_ColorAndAlpha>();
    }

    private void CreateDemo_PathSequence(GameObject parent, Vector3 origin)
    {
        var sphere = CreatePrimitive(parent, PrimitiveType.Sphere, origin, Color.green);

        // 创建路径点（在 origin 附近画一个菱形）
        var points = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            var p = new GameObject($"PathPoint_{i}");
            p.transform.SetParent(parent.transform);
            p.transform.position = origin + new Vector3(Mathf.Cos(angle) * 2f, 0f, Mathf.Sin(angle) * 2f);
            if (!Application.isPlaying) p.tag = "EditorOnly";
            points[i] = p;
        }

        var pathDemo = sphere.AddComponent<Example_13_PathMovement>();
        pathDemo.pathPoints = new Transform[5];
        for (int i = 0; i < 5; i++)
            pathDemo.pathPoints[i] = points[i].transform;
        pathDemo.moveDurationPerSegment = 0.8f;
        pathDemo.loopPath = true;
    }

    private void CreateDemo_LoopPingPong(GameObject parent, Vector3 origin)
    {
        var cube = CreatePrimitive(parent, PrimitiveType.Cube, origin, Color.yellow);
        var loopDemo = cube.AddComponent<Example_06_LoopModes>();
        loopDemo.loopType = Example_06_LoopModes.LoopDemoType.PingPongInfinite;
        loopDemo.moveDistance = 3f;
    }

    private void CreateDemo_LoopNormal(GameObject parent, Vector3 origin)
    {
        var cube = CreatePrimitive(parent, PrimitiveType.Cube, origin, new Color(1f, 0.5f, 0f));
        var loopDemo = cube.AddComponent<Example_06_LoopModes>();
        loopDemo.loopType = Example_06_LoopModes.LoopDemoType.NormalLoopFinite;
        loopDemo.finiteCount = 5;
        loopDemo.moveDistance = 3f;
        loopDemo.duration = 0.5f;
    }

    private void CreateDemo_CancelHandle(GameObject parent, Vector3 origin)
    {
        var cube = CreatePrimitive(parent, PrimitiveType.Cube, origin, Color.red);
        cube.AddComponent<Example_07_CancelAndHandle>();
    }

    private void CreateDemo_DelayedCall(GameObject parent, Vector3 origin)
    {
        var cube = CreatePrimitive(parent, PrimitiveType.Cube, origin, new Color(0.4f, 0.4f, 0.4f));

        cube.AddComponent<Example_08_DelayedCall>();
    }

    private void CreateDemo_MultiObject(GameObject parent, Vector3 origin)
    {
        int count = 5;
        var targets = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i * Mathf.Deg2Rad;
            Vector3 pos = origin + new Vector3(Mathf.Cos(angle) * 2.5f, 0f, Mathf.Sin(angle) * 2.5f);
            var cube = CreatePrimitive(parent, PrimitiveType.Cube, pos, Color.Lerp(Color.blue, Color.cyan, (float)i / count));
            targets[i] = cube;
        }

        var controller = parent.AddComponent<Example_09_MultiObject>();
        controller.targets = targets;
        controller.duration = 1.5f;
    }

    private void CreateDemo_EasingComparison(GameObject parent, Vector3 origin)
    {
        var types = new (Example_10_EasingFunctions.EasingType type, Color color)[]
        {
            (Example_10_EasingFunctions.EasingType.Linear, Color.gray),
            (Example_10_EasingFunctions.EasingType.QuadOut, Color.blue),
            (Example_10_EasingFunctions.EasingType.QuadInOut, Color.cyan),
            (Example_10_EasingFunctions.EasingType.CubicOut, Color.green),
            (Example_10_EasingFunctions.EasingType.BounceOut, Color.yellow),
            (Example_10_EasingFunctions.EasingType.ElasticOut, Color.magenta),
        };

        for (int i = 0; i < types.Length; i++)
        {
            Vector3 pos = origin + new Vector3(i * 1.2f - 3f, 0f, 0f);
            var sphere = CreatePrimitive(parent, PrimitiveType.Sphere, pos, types[i].color);
            sphere.transform.localScale = Vector3.one * 0.6f;

            var easing = sphere.AddComponent<Example_10_EasingFunctions>();
            easing.easing = types[i].type;
        }
    }

    private void CreateDemo_Bounce(GameObject parent, Vector3 origin)
    {
        var sphere = CreatePrimitive(parent, PrimitiveType.Sphere, origin, Color.green);
        sphere.AddComponent<Example_12_BounceEffect>();
    }

    private void CreateDemo_UI_Fade(GameObject parent, Vector3 origin)
    {
        // 模拟 UI 元素，用 SpriteRenderer 代替
        var go = new GameObject("FadeElement");
        go.transform.SetParent(parent.transform);
        go.transform.position = origin;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CreateDebugSprite(64, 64, Color.white, Color.white);
        sr.color = Color.white;
        sr.transform.localScale = Vector3.one * 2f;

        var cg = go.AddComponent<CanvasGroup>();
        var fade = go.AddComponent<Example_11_UI_Fade>();
        fade.autoPlay = true;
    }

    private void CreateDemo_Combined(GameObject parent, Vector3 origin)
    {
        // 组合动画：同时移动 + 旋转 + 缩放
        var cube = CreatePrimitive(parent, PrimitiveType.Cube, origin, new Color(0.5f, 0.3f, 0.8f));

        Vector3 startPos = origin;
        Vector3 endPos = origin + Vector3.right * 3f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * 1.5f;

        KTween.AddTween(cube, 1.5f, (t) =>
        {
            cube.transform.position = KTweenFunc.linear(startPos, endPos, t);
            cube.transform.localScale = KTweenFunc.linear(startScale, endScale, t);
            cube.transform.eulerAngles = KTweenFunc.linear(Vector3.zero, new Vector3(0f, 0f, 360f), t);
        }).SetLoopPingPong(-1);
    }

    // ============================================================
    // 辅助方法
    // ============================================================

    private GameObject CreatePrimitive(GameObject parent, PrimitiveType type, Vector3 position, Color color)
    {
        var go = GameObject.CreatePrimitive(type);
        go.transform.SetParent(parent.transform);
        go.transform.position = position;
        go.name = $"{type}_{color}";

        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit")
                                  ?? Shader.Find("Standard")
                                  ?? Shader.Find("Sprites/Default"));
            mat.color = color;
            renderer.material = mat;
        }

        return go;
    }

    private static Sprite CreateDebugSprite(int w, int h, Color border, Color fill)
    {
        var tex = new Texture2D(w, h);
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                tex.SetPixel(x, y, x == 0 || x == w - 1 || y == 0 || y == h - 1 ? border : fill);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
    }
}

/// <summary>
/// 辅助组件：在物体上方显示文本标签
/// </summary>
public class ExampleLabel : MonoBehaviour
{
    public string text = "";
    public Color color = Color.white;
    public Vector3 offset = Vector3.up * 1.8f;
    public int fontSize = 12;
    public bool show = true;

    private void OnGUI()
    {
        if (!show || string.IsNullOrEmpty(text)) return;

        Vector3 worldPos = transform.position + offset;
        Vector3 screenPos = Camera.main != null
            ? Camera.main.WorldToScreenPoint(worldPos)
            : Vector3.zero;

        if (screenPos.z < 0) return;

        GUI.color = color;
        var style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = color;

        Rect rect = new Rect(screenPos.x - 100f, Screen.height - screenPos.y - 15f, 200f, 30f);
        GUI.Label(rect, text, style);
    }

    private void OnDrawGizmos()
    {
        if (!show || string.IsNullOrEmpty(text)) return;
        Gizmos.color = color;
        Vector3 pos = transform.position + offset;
        Gizmos.DrawIcon(pos, "d_UnityEditor.ConsoleWindow", true);
    }
}