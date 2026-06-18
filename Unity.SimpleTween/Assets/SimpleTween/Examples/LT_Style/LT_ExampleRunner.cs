using UnityEngine;

/// <summary>
/// LeanTween 风格示例入口
/// 运行时自动创建 7 组 LeanTween 风格示例的可视化演示
/// </summary>
public class LT_ExampleRunner : MonoBehaviour
{
    private void Start()
    {
        float spacing = 6f;
        float startY = 4f;

        var demos = new (string name, System.Action create)[]
        {
            ("GeneralBasic", () => CreateBasicDemo(new Vector3(-8, startY, 0))),
            ("EasingTypes",  () => CreateEasingDemo(new Vector3(-8, startY - spacing, 0))),
            ("Sequencer",    () => CreateSequencerDemo(new Vector3(-8, startY - spacing * 2, 0))),
            ("CameraShake",  () => CreateCameraShakeDemo(new Vector3(-8, startY - spacing * 3, 0))),
            ("UI Style",     () => CreateUIDemo(new Vector3(-8, startY - spacing * 4, 0))),
            ("Path",         () => CreatePathDemo(new Vector3(-8, startY - spacing * 5, 0))),
            ("Following",    () => CreateFollowingDemo(new Vector3(-8, startY - spacing * 6, 0))),
        };

        foreach (var demo in demos)
        {
            demo.create();
        }
    }

    private void CreateBasicDemo(Vector3 origin)
    {
        var go = new GameObject("LT_GeneralBasic");
        go.transform.position = origin;
        var basic = go.AddComponent<LT_Style_GeneralBasic>();
        basic.prefabAvatar = null;
        var label = go.AddComponent<ExampleLabel>();
        label.text = "GeneralBasic";
    }

    private void CreateEasingDemo(Vector3 origin)
    {
        var go = new GameObject("LT_EasingTypes");
        go.transform.position = origin;
        go.AddComponent<LT_Style_EasingTypes>();
        var label = go.AddComponent<ExampleLabel>();
        label.text = "EasingTypes";
    }

    private void CreateSequencerDemo(Vector3 origin)
    {
        var go = new GameObject("LT_Sequencer");
        go.transform.position = origin;
        go.AddComponent<LT_Style_Sequencer>();
        var label = go.AddComponent<ExampleLabel>();
        label.text = "Sequencer";
    }

    private void CreateCameraShakeDemo(Vector3 origin)
    {
        var go = new GameObject("LT_CameraShake");
        go.transform.position = origin;
        go.AddComponent<LT_Style_CameraShake>();
        var label = go.AddComponent<ExampleLabel>();
        label.text = "CameraShake\n点击触发";
    }

    private void CreateUIDemo(Vector3 origin)
    {
        var go = new GameObject("LT_UI");
        go.transform.position = origin;
        // UI 示例需要手动的 UI 元素，这里显示提示
        var label = go.AddComponent<ExampleLabel>();
        label.text = "UI Style\n需挂载 RectTransform";
    }

    private void CreatePathDemo(Vector3 origin)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = origin;
        sphere.name = "LT_Path";
        sphere.GetComponent<Renderer>().material.color = new Color(1, 0.5f, 0);
        sphere.AddComponent<LT_Style_Path>();
        var label = sphere.AddComponent<ExampleLabel>();
        label.text = "Path";
    }

    private void CreateFollowingDemo(Vector3 origin)
    {
        var go = new GameObject("LT_Following");
        go.transform.position = origin;
        go.AddComponent<LT_Style_Following>();
        var label = go.AddComponent<ExampleLabel>();
        label.text = "Following";
    }
}
