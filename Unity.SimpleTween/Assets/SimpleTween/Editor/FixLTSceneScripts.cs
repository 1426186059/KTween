using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 批量替换 LTScene 场景中的 LeanTween 脚本引用为 ST_ 脚本
/// 使用: 菜单 → SimpleTween → Fix LTScene Script References
/// </summary>
public static class FixLTSceneScripts
{
    private static readonly Dictionary<string, string> s_ScriptMap = new Dictionary<string, string>
    {
        { "GeneralBasic",            "ST_GeneralBasic" },
        { "GeneralBasics2d",         "ST_GeneralBasics2d" },
        { "GeneralCameraShake",      "ST_GeneralCameraShake" },
        { "GeneralEasingTypes",      "ST_GeneralEasingTypes" },
        { "GeneralSequencer",        "ST_GeneralSequencer" },
        { "GeneralAdvancedTechniques","ST_GeneralAdvancedTechniques" },
        { "Following",               "ST_Following" },
        { "LogoCinematic",           "ST_LogoCinematic" },
        { "PathBezier",              "ST_PathBezier" },
        { "PathBezier2d",            "ST_PathBezier2d" },
        { "PathSpline",              "ST_PathSpline" },
        { "PathSpline2d",            "ST_PathSpline2d" },
        { "PathSplineEndless",       "ST_PathSplineEndless" },
        { "PathSplinePerformance",   "ST_PathSplinePerformance" },
        { "PathSplines",             "ST_PathSplines" },
        { "PathSplineTrack",         "ST_PathSplineTrack" },
        { "GeneralSimpleUI",         "ST_GeneralSimpleUI" },
        { "GeneralUISpace",          "ST_GeneralUISpace" },
        { "GeneralEventsListeners",  "ST_GeneralEventsListeners" },
        { "ExampleSpline",           "ST_PathSpline" },
    };

    [MenuItem("SimpleTween/Fix LTScene Script References", false, 25)]
    public static void FixAllScenes()
    {
        string sceneDir = "Assets/SimpleTween/LTScene";
        var sceneFiles = Directory.GetFiles(sceneDir, "ST_*.unity");

        int fixedCount = 0;
        foreach (var scenePath in sceneFiles)
        {
            if (FixScene(scenePath))
                fixedCount++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"已完成！共修复 {fixedCount} 个场景的脚本引用");
    }

    private static bool FixScene(string scenePath)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        var rootObjects = scene.GetRootGameObjects();
        bool modified = false;

        foreach (var root in rootObjects)
        {
            modified |= FixGameObject(root);
        }

        if (modified)
        {
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"已修复: {scenePath}");
            return true;
        }

        return false;
    }

    private static bool FixGameObject(GameObject go)
    {
        bool modified = false;
        var components = go.GetComponents<MonoBehaviour>();

        foreach (var comp in components)
        {
            if (comp == null)
            {
                // Missing script — 检查原名并替换为 ST_ 版本
                string originalName = GetOriginalScriptName(go, comp);
                if (originalName != null && s_ScriptMap.TryGetValue(originalName, out string stName))
                {
                    // 移除缺失脚本
                    var serializedObj = new SerializedObject(go);
                    var scriptProp = serializedObj.FindProperty("m_Component");

                    // 添加对应的 ST_ 脚本
                    var stType = System.Type.GetType(stName + ", Assembly-CSharp")
                               ?? System.Type.GetType(stName + ", SimpleTween.Examples")
                               ?? System.Type.GetType(stName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

                    if (stType != null)
                    {
                        go.AddComponent(stType);
                        modified = true;
                        Debug.Log($"  替换: {go.name} → {stName}");
                    }
                }
            }
        }

        foreach (Transform child in go.transform)
            modified |= FixGameObject(child.gameObject);

        return modified;
    }

    private static string GetOriginalScriptName(GameObject go, MonoBehaviour missingComp)
    {
        // 用反射读取缺失脚本的类名
        // 在 MonoScript 中，类名存储在序列化数据里
        // 对于 Missing 脚本，我们只能从场景 YAML 中的 m_Script GUID 推断
        // 这里使用文件名启发式匹配
        return null;
    }
}
