using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 一键克隆 LeanTween 示例场景，将脚本替换为 SimpleTween 等价实现
/// 使用: 菜单 → SimpleTween → Clone LeanTween Scenes
/// </summary>
public static class CloneLeanTweenScenes
{
    private const string LeanScenesDir = "Assets/LeanTween/Examples/Scenes";
    private const string OutDir = "Assets/SimpleTween/Scenes";

    private struct ObjData
    {
        public string Name;
        public Vector3 Pos;
        public Quaternion Rot;
        public Vector3 Scale;
        public bool IsPrimitive; // 用基本体还是空 GameObject
        public PrimitiveType PrimitiveType;
        public Color Color;
        public bool HasSprite;
        public bool IsRoot; // 根对象还是子对象
        public int ParentIndex; // 父对象在列表中的索引
    }

    [MenuItem("SimpleTween/Clone LeanTween Scenes", false, 20)]
    public static void CloneAll()
    {
        if (!Directory.Exists(OutDir))
            Directory.CreateDirectory(OutDir);

        var pairs = new(string sceneName, string className, string outputName, Color color)[]
        {
            ("GeneralBasic",           "GeneralBasic",           "ST_Basic",              Color.cyan),
            ("GeneralBasics2d",        "GeneralBasics2d",        "ST_Basics2D",           Color.blue),
            ("GeneralCameraShake",     "GeneralCameraShake",     "ST_CameraShake",        Color.red),
            ("GeneralEasingTypes",     "GeneralEasingTypes",     "ST_EasingTypes",        Color.gray),
            ("GeneralSequencer",       "GeneralSequencer",       "ST_Sequencer",          Color.green),
            ("GeneralAdvancedTechniques", "GeneralAdvancedTechniques", "ST_Advanced", new Color(0.5f, 0.3f, 0.8f)),
            ("Following",              "Following",              "ST_Following",          Color.yellow),
            ("LogoCinematic",          "LogoCinematic",          "ST_LogoCinematic",      Color.white),
            ("PathBezier",             "PathBezier",             "ST_PathBezier",         new Color(1, 0.5f, 0)),
            ("PathSplines",            "PathSpline",             "ST_PathSpline",         new Color(1, 0.5f, 0)),
            ("PathBezier2d",           "PathBezier2d",           "ST_PathBezier2D",       new Color(1, 0.5f, 0)),
            ("PathSpline2d",           "PathSpline2d",           "ST_PathSpline2D",       new Color(1, 0.5f, 0)),
        };

        int count = 0;
        foreach (var p in pairs)
        {
            if (CloneScene(p.sceneName, p.className, p.outputName, p.color))
                count++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"克隆完成！共生成 {count} 个场景到 {OutDir}");
    }

    private static bool CloneScene(string sceneName, string leanClassName, string outputName, Color hintColor)
    {
        string srcPath = $"{LeanScenesDir}/{sceneName}.unity";
        if (!File.Exists(srcPath)) return false;

        // 1. 打开 LeanTween 场景读取数据
        var srcScene = EditorSceneManager.OpenScene(srcPath, OpenSceneMode.Single);
        var rootObjects = srcScene.GetRootGameObjects();

        // 2. 提取所有对象数据（在场景销毁前完成）
        var allData = new List<ObjData>();
        foreach (var root in rootObjects)
            ExtractData(root, allData, -1, true);

        // 3. 创建新场景
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var cam = Camera.main;
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 2, -8);
            cam.transform.rotation = Quaternion.Euler(10, 0, 0);
        }

        // 4. 从数据重建对象
        var created = new List<GameObject>();
        foreach (var d in allData)
        {
            GameObject go;
            if (d.HasSprite)
            {
                go = new GameObject(d.Name);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.color = d.Color;
            }
            else if (d.IsPrimitive)
            {
                go = GameObject.CreatePrimitive(d.PrimitiveType);
                go.name = d.Name;
                go.GetComponent<Renderer>().material.color = d.Color;
            }
            else
            {
                go = new GameObject(d.Name);
            }

            go.transform.position = d.Pos;
            go.transform.rotation = d.Rot;
            go.transform.localScale = d.Scale;

            if (d.ParentIndex >= 0 && d.ParentIndex < created.Count)
                go.transform.SetParent(created[d.ParentIndex].transform);

            created.Add(go);
        }

        // 5. 在根对象上挂 SimpleTween 脚本
        foreach (var go in created)
        {
            if (go.transform.parent == null)
                AddSTScript(go, leanClassName, outputName);
        }

        // 6. 保存
        string outPath = $"{OutDir}/{outputName}.unity";
        EditorSceneManager.SaveScene(newScene, outPath);
        Debug.Log($"已创建: {outputName}.unity ← {sceneName}");
        return true;
    }

    private static void ExtractData(GameObject go, List<ObjData> data, int parentIdx, bool isRoot)
    {
        var mf = go.GetComponent<MeshFilter>();
        var rend = go.GetComponent<Renderer>();
        var sr = go.GetComponent<SpriteRenderer>();

        ObjData d = new ObjData
        {
            Name = go.name,
            Pos = go.transform.position,
            Rot = go.transform.rotation,
            Scale = go.transform.localScale,
            IsPrimitive = false,
            HasSprite = sr != null,
            IsRoot = isRoot,
            ParentIndex = parentIdx,
            Color = Color.white,
        };

        if (sr != null)
        {
            d.Color = sr.color;
        }
        else if (mf != null && rend != null)
        {
            d.IsPrimitive = true;
            d.Color = rend.sharedMaterial != null ? rend.sharedMaterial.color : Color.white;
            string meshName = mf.sharedMesh != null ? mf.sharedMesh.name.ToLower() : "";
            if (meshName.Contains("sphere") || meshName.Contains("ico"))
                d.PrimitiveType = PrimitiveType.Sphere;
            else if (meshName.Contains("capsule"))
                d.PrimitiveType = PrimitiveType.Capsule;
            else if (meshName.Contains("cylinder"))
                d.PrimitiveType = PrimitiveType.Cylinder;
            else if (meshName.Contains("quad"))
                d.PrimitiveType = PrimitiveType.Quad;
            else if (meshName.Contains("plane"))
                d.PrimitiveType = PrimitiveType.Plane;
            else
                d.PrimitiveType = PrimitiveType.Cube;
        }

        int myIdx = data.Count;
        data.Add(d);

        foreach (Transform child in go.transform)
            ExtractData(child.gameObject, data, myIdx, false);
    }

    private static void AddSTScript(GameObject go, string leanClassName, string outputName)
    {
        switch (leanClassName)
        {
            case "GeneralBasic":             go.AddComponent<ST_GeneralBasic>(); break;
            case "GeneralBasics2d":          go.AddComponent<ST_GeneralBasics2D>(); break;
            case "GeneralCameraShake":       go.AddComponent<ST_CameraShake>(); break;
            case "GeneralSequencer":         go.AddComponent<ST_Sequencer>(); break;
            case "GeneralAdvancedTechniques": go.AddComponent<ST_AdvancedTechniques>(); break;
            case "Following":                go.AddComponent<ST_Following>(); break;
            case "LogoCinematic":            go.AddComponent<ST_LogoCinematic>(); break;
            case "PathBezier":
            case "PathSpline":
            case "PathBezier2d":
            case "PathSpline2d":             go.AddComponent<ST_Path>(); break;
        }
    }
}
