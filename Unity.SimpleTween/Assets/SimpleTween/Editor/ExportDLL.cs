using UnityEditor;
using UnityEngine;
using System.IO;

public static class ExportDLL
{
    private const string SrcDll = "Library/ScriptAssemblies/SimpleTween.Runtime.dll";
    private const string DestDir = "AAABuild";

    [MenuItem("SimpleTween/Export DLL", false, 50)]
    public static void Export()
    {
        string root = Path.GetFullPath(".");
        string dest = Path.Combine(root, DestDir);
        Directory.CreateDirectory(dest);
        File.Copy(Path.Combine(root, SrcDll), Path.Combine(dest, "SimpleTween.Runtime.dll"), true);
        AssetDatabase.Refresh();
        Debug.Log($"已导出: {dest}/SimpleTween.Runtime.dll");
    }
}
