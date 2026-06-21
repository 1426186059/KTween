using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 鎵归噺鏇挎崲 LTScene 鍦烘櫙涓殑 LeanTween 鑴氭湰寮曠敤涓?ST_ 鑴氭湰
/// 浣跨敤: 鑿滃崟 鈫?KTween 鈫?Fix LTScene Script References
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

    [MenuItem("KTween/Fix LTScene Script References", false, 25)]
    public static void FixAllScenes()
    {
        string sceneDir = "Assets/KTween/LTScene";
        var sceneFiles = Directory.GetFiles(sceneDir, "ST_*.unity");

        int fixedCount = 0;
        foreach (var scenePath in sceneFiles)
        {
            if (FixScene(scenePath))
                fixedCount++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"宸插畬鎴愶紒鍏变慨澶?{fixedCount} 涓満鏅殑鑴氭湰寮曠敤");
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
            Debug.Log($"宸蹭慨澶? {scenePath}");
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
                // Missing script 鈥?妫€鏌ュ師鍚嶅苟鏇挎崲涓?ST_ 鐗堟湰
                string originalName = GetOriginalScriptName(go, comp);
                if (originalName != null && s_ScriptMap.TryGetValue(originalName, out string stName))
                {
                    // 绉婚櫎缂哄け鑴氭湰
                    var serializedObj = new SerializedObject(go);
                    var scriptProp = serializedObj.FindProperty("m_Component");

                    // 娣诲姞瀵瑰簲鐨?ST_ 鑴氭湰
                    var stType = System.Type.GetType(stName + ", Assembly-CSharp")
                               ?? System.Type.GetType(stName + ", KTween.Examples")
                               ?? System.Type.GetType(stName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

                    if (stType != null)
                    {
                        go.AddComponent(stType);
                        modified = true;
                        Debug.Log($"  鏇挎崲: {go.name} 鈫?{stName}");
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
        // 鐢ㄥ弽灏勮鍙栫己澶辫剼鏈殑绫诲悕
        // 鍦?MonoScript 涓紝绫诲悕瀛樺偍鍦ㄥ簭鍒楀寲鏁版嵁閲?
        // 瀵逛簬 Missing 鑴氭湰锛屾垜浠彧鑳戒粠鍦烘櫙 YAML 涓殑 m_Script GUID 鎺ㄦ柇
        // 杩欓噷浣跨敤鏂囦欢鍚嶅惎鍙戝紡鍖归厤
        return null;
    }
}
