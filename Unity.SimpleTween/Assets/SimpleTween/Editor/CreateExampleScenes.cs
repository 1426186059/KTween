using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

/// <summary>
/// 一键生成所有 SimpleTween 示例场景（含可视化物体）
/// 使用: 菜单 → SimpleTween → Create Example Scenes
/// </summary>
public static class CreateExampleScenes
{
    private const string ScenesDir = "Assets/SimpleTween/Scenes";

    [MenuItem("SimpleTween/Create All Example Scenes", false, 10)]
    public static void CreateAllScenes()
    {
        if (!Directory.Exists(ScenesDir))
            Directory.CreateDirectory(ScenesDir);

        CreateScene("BasicMove", "01 - 基础移动", typeof(Example_01_BasicMove), PrimitiveType.Cube, Color.cyan);
        CreateScene("Rotation", "02 - 旋转动画", typeof(Example_02_Rotation), PrimitiveType.Cube, new Color(0f, 0.8f, 1f));
        CreateScene("ScalePulse", "03 - 缩放脉冲", typeof(Example_03_ScalePulse), PrimitiveType.Sphere, Color.magenta);
        CreateScene("ColorFade", "04 - 颜色渐变", typeof(Example_04_ColorAndAlpha), null, Color.red);
        CreateScene("DelayAndChain", "05 - 延迟与序列", typeof(Example_05_DelayAndChain), PrimitiveType.Sphere, Color.green);
        CreateScene("LoopModes", "06 - 循环模式", typeof(Example_06_LoopModes), PrimitiveType.Cube, Color.yellow);
        CreateScene("CancelAndHandle", "07 - Cancel 与 Handle", typeof(Example_07_CancelAndHandle), PrimitiveType.Cube, Color.red);
        CreateScene("DelayedCall", "08 - 延迟调用", typeof(Example_08_DelayedCall), PrimitiveType.Cube, Color.gray);
        CreateScene("MultiObject", "09 - 多物体 Tween", typeof(Example_09_MultiObject), null, Color.blue);
        CreateScene("EasingFunctions", "10 - 缓动函数对比", typeof(Example_10_EasingFunctions), PrimitiveType.Sphere, Color.white);
        CreateScene("UIFade", "11 - UI 淡入淡出", typeof(Example_11_UI_Fade), null, Color.white);
        CreateScene("Bounce", "12 - 弹跳效果", typeof(Example_12_BounceEffect), PrimitiveType.Sphere, Color.green);
        CreateScene("PathMovement", "13 - 路径运动", typeof(Example_13_PathMovement), PrimitiveType.Sphere, Color.yellow);
        CreateScene("AllInOne", "AllInOne 合集", typeof(Example_AllInOne), PrimitiveType.Cube, new Color(0.5f, 0.3f, 0.8f));
        CreateScene("AllDemos", "全部示例合集", typeof(ExampleRunner), null, Color.white);

        // ---- LT 风格示例 ----
        CreateScene("LT_Basic", "LT-GeneralBasic", typeof(LT_Style_GeneralBasic), null, Color.white);
        CreateScene("LT_Easing", "LT-EasingTypes", typeof(LT_Style_EasingTypes), null, Color.white);
        CreateScene("LT_Sequencer", "LT-Sequencer", typeof(LT_Style_Sequencer), null, Color.white);
        CreateScene("LT_CameraShake", "LT-CameraShake", typeof(LT_Style_CameraShake), null, Color.white);
        CreateScene("LT_Path", "LT-Path", typeof(LT_Style_Path), null, Color.white);
        CreateScene("LT_Following", "LT-Following", typeof(LT_Style_Following), null, Color.white);
        CreateScene("LT_AllDemos", "LT-全部合集", typeof(LT_ExampleRunner), null, Color.white);

        AssetDatabase.Refresh();
        Debug.Log($"成功创建 {ScenesDir} 目录下的所有示例场景！");
    }

    [MenuItem("SimpleTween/Create Example Scene...", false, 11)]
    public static void ShowSceneCreatorWindow()
    {
        var window = EditorWindow.GetWindow<ExampleSceneCreatorWindow>(true, "创建示例场景");
        window.minSize = new Vector2(300, 400);
        window.Show();
    }

    internal static void CreateScene(string fileName, string displayName, System.Type componentType,
        PrimitiveType? visualType, Color color)
    {
        string scenePath = $"{ScenesDir}/{fileName}.unity";
        if (File.Exists(scenePath))
        {
            if (!EditorUtility.DisplayDialog("场景已存在",
                $"{scenePath} 已存在。是否覆盖？", "覆盖", "跳过"))
                return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,
            NewSceneMode.Single);

        // 调整主相机
        var camGO = Camera.main?.gameObject;
        if (camGO == null)
        {
            camGO = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            camGO.tag = "MainCamera";
        }
        camGO.transform.position = new Vector3(0, 3, -8);
        camGO.transform.rotation = Quaternion.Euler(10, 0, 0);

        // -------------------- 创建可视化物体 + 挂示例脚本 --------------------

        if (componentType == typeof(ExampleRunner) || componentType == typeof(LT_ExampleRunner)
            || componentType == typeof(LT_Style_GeneralBasic)
            || componentType == typeof(LT_Style_EasingTypes)
            || componentType == typeof(LT_Style_Sequencer)
            || componentType == typeof(LT_Style_CameraShake)
            || componentType == typeof(LT_Style_Path)
            || componentType == typeof(LT_Style_Following))
        {
            var go = new GameObject(displayName);
            go.transform.position = Vector3.zero;
            go.AddComponent(componentType);
        }
        else if (componentType == typeof(Example_09_MultiObject))
        {
            var controllerGO = new GameObject(displayName);
            controllerGO.transform.position = Vector3.zero;
            var multi = controllerGO.AddComponent<Example_09_MultiObject>();

            var targets = new GameObject[5];
            for (int i = 0; i < 5; i++)
            {
                float angle = (360f / 5) * i * Mathf.Deg2Rad;
                var pos = new Vector3(Mathf.Cos(angle) * 2.5f, 0f, Mathf.Sin(angle) * 2.5f);
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = pos;
                cube.transform.SetParent(controllerGO.transform);
                cube.name = $"Target_{i}";
                cube.GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.cyan, (float)i / 5);
                targets[i] = cube;
            }
            multi.targets = targets;
        }
        else if (componentType == typeof(Example_10_EasingFunctions))
        {
            // 创建 6 个不同缓动类型的小球
            var types = new (Example_10_EasingFunctions.EasingType type, Color c)[]
            {
                (Example_10_EasingFunctions.EasingType.Linear, Color.gray),
                (Example_10_EasingFunctions.EasingType.QuadOut, Color.blue),
                (Example_10_EasingFunctions.EasingType.CubicOut, Color.green),
                (Example_10_EasingFunctions.EasingType.BounceOut, Color.yellow),
                (Example_10_EasingFunctions.EasingType.ElasticOut, Color.magenta),
                (Example_10_EasingFunctions.EasingType.SineInOut, Color.cyan),
            };
            for (int i = 0; i < types.Length; i++)
            {
                var pos = new Vector3(i * 1.5f - 3.75f, 0f, 0f);
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = pos;
                sphere.transform.localScale = Vector3.one * 0.6f;
                sphere.GetComponent<Renderer>().material.color = types[i].c;

                var easing = sphere.AddComponent<Example_10_EasingFunctions>();
                easing.easing = types[i].type;
                easing.duration = 1.2f;
                easing.moveDistance = 1.5f;
                easing.autoPlay = true;
                sphere.name = $"{displayName}_{types[i].type}";
            }
            // 调整相机视角以便看到所有小球
            camGO.transform.position = new Vector3(0, 3, -12);
        }
        else if (componentType == typeof(Example_04_ColorAndAlpha))
        {
            var go = new GameObject(displayName);
            go.transform.position = Vector3.zero;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateDebugSprite();
            sr.color = Color.red;
            go.AddComponent<Example_04_ColorAndAlpha>();
        }
        else if (componentType == typeof(Example_11_UI_Fade))
        {
            var go = new GameObject(displayName);
            go.transform.position = Vector3.zero;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateDebugSprite();
            sr.color = Color.white;
            go.AddComponent<CanvasGroup>();
            go.AddComponent<Example_11_UI_Fade>();
        }
        else if (componentType == typeof(Example_05_DelayAndChain))
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = displayName;
            go.transform.position = Vector3.zero;
            go.AddComponent<Example_05_DelayAndChain>();

            // 额外画几个路径点标记
            for (int i = 0; i < 4; i++)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.transform.localScale = Vector3.one * 0.15f;
                marker.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.4f);
                marker.name = $"Waypoint_{i}";
            }
        }
        else if (componentType == typeof(Example_12_BounceEffect))
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = displayName;
            go.transform.position = Vector3.zero;
            go.AddComponent<Example_12_BounceEffect>();
        }
        else if (componentType == typeof(Example_13_PathMovement))
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = displayName;
            go.transform.position = Vector3.zero;
            var path = go.AddComponent<Example_13_PathMovement>();

            // 创建路径点
            var points = new GameObject[5];
            for (int i = 0; i < 5; i++)
            {
                float angle = i * 90f * Mathf.Deg2Rad;
                var p = new GameObject($"PathPoint_{i}");
                p.transform.position = new Vector3(Mathf.Cos(angle) * 2f, 0f, Mathf.Sin(angle) * 2f);
                points[i] = p;
            }
            path.pathPoints = new Transform[5];
            for (int i = 0; i < 5; i++)
                path.pathPoints[i] = points[i].transform;
            path.moveDurationPerSegment = 0.8f;
            path.loopPath = true;
        }
        else if (componentType == typeof(Example_07_CancelAndHandle))
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = displayName;
            go.transform.position = Vector3.zero;
            go.GetComponent<Renderer>().material.color = Color.red;
            go.AddComponent<Example_07_CancelAndHandle>();
        }
        else if (componentType == typeof(Example_08_DelayedCall))
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = displayName;
            go.transform.position = Vector3.zero;
            go.GetComponent<Renderer>().material.color = Color.gray;
            go.AddComponent<Example_08_DelayedCall>();
        }
        else
        {
            // 通用：创建指定类型的基本体 + 上色
            PrimitiveType type = visualType ?? PrimitiveType.Cube;
            var go = GameObject.CreatePrimitive(type);
            go.name = displayName;
            go.transform.position = Vector3.zero;
            go.GetComponent<Renderer>().material.color = color;
            go.AddComponent(componentType);
        }

        // 保存场景
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log($"已创建场景: {displayName} → {scenePath}");
    }

    private static Sprite CreateDebugSprite()
    {
        var tex = new Texture2D(64, 64);
        for (int x = 0; x < 64; x++)
            for (int y = 0; y < 64; y++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 100f);
    }
}

/// <summary>
/// 示例场景创建器对话框 — 选择单个示例生成场景
/// </summary>
public class ExampleSceneCreatorWindow : EditorWindow
{
    private Vector2 m_ScrollPos;

    private struct ExampleEntry
    {
        public string Name;
        public string FileName;
        public System.Type ComponentType;
        public string Description;
    }

    private static readonly ExampleEntry[] s_Examples = new[]
    {
        new ExampleEntry { Name = "01 - 基础移动", FileName = "BasicMove", ComponentType = typeof(Example_01_BasicMove), Description = "在 X 轴上平移运动" },
        new ExampleEntry { Name = "02 - 旋转动画", FileName = "Rotation", ComponentType = typeof(Example_02_Rotation), Description = "物体绕 Z 轴 360 度旋转" },
        new ExampleEntry { Name = "03 - 缩放脉冲", FileName = "ScalePulse", ComponentType = typeof(Example_03_ScalePulse), Description = "呼吸式 PingPong 缩放" },
        new ExampleEntry { Name = "04 - 颜色渐变", FileName = "ColorFade", ComponentType = typeof(Example_04_ColorAndAlpha), Description = "SpriteRenderer 颜色 PingPong" },
        new ExampleEntry { Name = "05 - 延迟与序列", FileName = "DelayAndChain", ComponentType = typeof(Example_05_DelayAndChain), Description = "AppendTween 链式串联" },
        new ExampleEntry { Name = "06 - 循环模式", FileName = "LoopModes", ComponentType = typeof(Example_06_LoopModes), Description = "Loop / PingPong 对比" },
        new ExampleEntry { Name = "07 - Cancel 与 Handle", FileName = "CancelAndHandle", ComponentType = typeof(Example_07_CancelAndHandle), Description = "按空格取消 Tween" },
        new ExampleEntry { Name = "08 - 延迟调用", FileName = "DelayedCall", ComponentType = typeof(Example_08_DelayedCall), Description = "delayedCall 定时回调" },
        new ExampleEntry { Name = "09 - 多物体 Tween", FileName = "MultiObject", ComponentType = typeof(Example_09_MultiObject), Description = "多个物体同步动画" },
        new ExampleEntry { Name = "10 - 缓动函数", FileName = "EasingFunctions", ComponentType = typeof(Example_10_EasingFunctions), Description = "12 种缓动曲线对比" },
        new ExampleEntry { Name = "11 - UI 淡入淡出", FileName = "UIFade", ComponentType = typeof(Example_11_UI_Fade), Description = "CanvasGroup 透明度动画" },
        new ExampleEntry { Name = "12 - 弹跳效果", FileName = "Bounce", ComponentType = typeof(Example_12_BounceEffect), Description = "多 Tween 组合弹跳" },
        new ExampleEntry { Name = "13 - 路径运动", FileName = "PathMovement", ComponentType = typeof(Example_13_PathMovement), Description = "沿多点路径运动" },
        new ExampleEntry { Name = "AllInOne 合集", FileName = "AllInOne", ComponentType = typeof(Example_AllInOne), Description = "20 种用法合集" },
        new ExampleEntry { Name = "全部示例合集", FileName = "AllDemos", ComponentType = typeof(ExampleRunner), Description = "所有示例可视化展示" },
        // ---- LT 风格 ----
        new ExampleEntry { Name = "LT-GeneralBasic", FileName = "LT_Basic", ComponentType = typeof(LT_Style_GeneralBasic), Description = "旋转/缩放/移动/延迟" },
        new ExampleEntry { Name = "LT-EasingTypes", FileName = "LT_Easing", ComponentType = typeof(LT_Style_EasingTypes), Description = "10 种缓动曲线" },
        new ExampleEntry { Name = "LT-Sequencer", FileName = "LT_Sequencer", ComponentType = typeof(LT_Style_Sequencer), Description = "AppendTween 序列动画" },
        new ExampleEntry { Name = "LT-CameraShake", FileName = "LT_CameraShake", ComponentType = typeof(LT_Style_CameraShake), Description = "相机震动" },
        new ExampleEntry { Name = "LT-Path", FileName = "LT_Path", ComponentType = typeof(LT_Style_Path), Description = "路径运动" },
        new ExampleEntry { Name = "LT-Following", FileName = "LT_Following", ComponentType = typeof(LT_Style_Following), Description = "5 种跟随模式" },
        new ExampleEntry { Name = "LT-全部合集", FileName = "LT_AllDemos", ComponentType = typeof(LT_ExampleRunner), Description = "LT 风格全部示例" },
    };

    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("选择要创建场景的示例:", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

        foreach (var entry in s_Examples)
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(entry.Name, GUILayout.Width(140));
            EditorGUILayout.LabelField(entry.Description, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("创建场景", GUILayout.Width(80)))
            {
                CreateSingleScene(entry);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("场景将保存到 Assets/SimpleTween/Scenes/", MessageType.Info);
    }

    private static void CreateSingleScene(ExampleEntry entry)
    {
        CreateExampleScenes.CreateScene(entry.FileName, entry.Name, entry.ComponentType, PrimitiveType.Cube, Color.white);
    }
}
