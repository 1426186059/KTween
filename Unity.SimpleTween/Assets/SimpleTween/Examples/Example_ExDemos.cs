using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SimpleTweenEx 可视化演示 — 运行时自动展示所有 Ex 扩展方法和缓动类型
/// </summary>
public class Example_ExDemos : MonoBehaviour
{
    [Header("缓动对比网格")]
    public float startX = -18f;
    public float startZ = 8f;
    public float spacingX = 5f;
    public float spacingZ = 1.5f;

    private void Start()
    {
        CreateEasingComparisonGrid();
        CreateApiDemos();
    }

    // ==============================================================
    // 缓动类型对比网格 — 每种 SimpleTweenType 一个小球来回移动
    // ==============================================================
    private void CreateEasingComparisonGrid()
    {
        var types = GetAllEaseTypes();
        int cols = 5;

        for (int i = 0; i < types.Count; i++)
        {
            int col = i % cols;
            int row = i / cols;
            Vector3 pos = new Vector3(startX + col * spacingX, 0f, startZ - row * spacingZ);

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = types[i].ToString();
            sphere.transform.position = pos;
            sphere.transform.localScale = Vector3.one * 0.6f;
            sphere.GetComponent<Renderer>().material.color = Color.Lerp(
                Color.cyan, Color.magenta, (float)i / types.Count);

            // move 来回移动展示缓动效果
            Vector3 target = pos + Vector3.right * 3.5f;
            SimpleTweenEx.move(sphere, target, 1.2f)
                .SetEase(types[i])
                .SetLoopPingPong(-1);
        }
    }

    // ==============================================================
    // 各 API 演示
    // ==============================================================
    private void CreateApiDemos()
    {
        var labelY = startZ - (Mathf.CeilToInt(GetAllEaseTypes().Count / 5f) + 1) * spacingZ;

        // ---------- move / moveX / moveY / moveZ ----------
        float y = labelY;
        Label($"move + easeOutBounce", new Vector3(-6f, y, 0f));
        var cube = CreateCube(Color.cyan, new Vector3(-6f, y - 0.5f, 0f));
        SimpleTweenEx.move(cube, new Vector3(-3f, y - 0.5f, 0f), 1.2f).SetEase(SimpleTweenType.easeOutBounce)
            .SetLoopPingPong(-1);

        Label($"moveX + easeInOutQuad", new Vector3(0f, y, 0f));
        var cubeX = CreateCube(Color.blue, new Vector3(0f, y - 0.5f, 0f));
        SimpleTweenEx.moveX(cubeX, 2.5f, 1.0f).SetEase(SimpleTweenType.easeInOutQuad)
            .SetLoopPingPong(-1);

        Label($"moveY + easeInOutCubic", new Vector3(6f, y, 0f));
        var cubeY = CreateCube(Color.green, new Vector3(6f, y - 0.5f, 0f));
        SimpleTweenEx.moveY(cubeY, y + 1.5f, 0.8f).SetEase(SimpleTweenType.easeInOutCubic)
            .SetLoopPingPong(-1);

        y -= 2.5f;

        // ---------- moveLocal + moveLocalX ----------
        Label($"moveLocal + easeOutQuad", new Vector3(-6f, y, 0f));
        var local = CreateSphere(Color.magenta, new Vector3(-6f, y - 0.5f, 0f));
        var parent = new GameObject("LocalParent");
        parent.transform.position = new Vector3(-6f, y - 0.5f, 0f);
        local.transform.SetParent(parent.transform);
        SimpleTweenEx.moveLocal(local, new Vector3(2f, 0f, 0f), 1.2f).SetEase(SimpleTweenType.easeOutQuad)
            .SetLoopPingPong(-1);

        Label($"moveLocalX + easeInOutBack", new Vector3(0f, y, 0f));
        var lx = CreateSphere(Color.yellow, new Vector3(0f, y - 0.5f, 0f));
        var p2 = new GameObject("LocalParent2");
        p2.transform.position = new Vector3(0f, y - 0.5f, 0f);
        lx.transform.SetParent(p2.transform);
        SimpleTweenEx.moveLocalX(lx, 2.5f, 1.0f).SetEase(SimpleTweenType.easeInOutBack)
            .SetLoopPingPong(-1);

        Label($"moveLocalZ + easeOutElastic", new Vector3(6f, y, 0f));
        var lz = CreateSphere(Color.cyan, new Vector3(6f, y - 0.5f, 0f));
        var p3 = new GameObject("LocalParent3");
        p3.transform.position = new Vector3(6f, y - 0.5f, 0f);
        lz.transform.SetParent(p3.transform);
        SimpleTweenEx.moveLocalZ(lz, 3f, 1.2f).SetEase(SimpleTweenType.easeOutElastic)
            .SetLoopPingPong(-1);

        y -= 2.5f;

        // ---------- scale ----------
        Label($"scale + easeInOutBack", new Vector3(-6f, y, 0f));
        var scaleObj = CreateSphere(Color.red, new Vector3(-6f, y - 0.5f, 0f));
        SimpleTweenEx.scale(scaleObj, new Vector3(2f, 2f, 2f), 1.0f).SetEase(SimpleTweenType.easeInOutBack)
            .SetLoopPingPong(-1);

        Label($"scale + easeSpring", new Vector3(0f, y, 0f));
        var springObj = CreateSphere(new Color(1f, 0.5f, 0f), new Vector3(0f, y - 0.5f, 0f));
        SimpleTweenEx.scale(springObj, new Vector3(2f, 0.5f, 0.5f), 0.8f).SetEase(SimpleTweenType.easeSpring)
            .SetLoopPingPong(-1);

        // ---------- rotateAround ----------
        Label($"rotateAround + easeLinear", new Vector3(6f, y, 0f));
        var rot = CreateCube(Color.gray, new Vector3(6f, y - 0.5f, 0f));
        SimpleTweenEx.rotateAround(rot, Vector3.up, 360f, 2.0f).SetEase(SimpleTweenType.easeLinear)
            .SetLoop(-1);

        y -= 2.5f;

        // ---------- rotateAroundLocal + color + alpha ----------
        Label($"rotateAroundLocal + easeOutQuad", new Vector3(-6f, y, 0f));
        var rotLocal = CreateCube(Color.gray, new Vector3(-6f, y - 0.5f, 0f));
        SimpleTweenEx.rotateAroundLocal(rotLocal, Vector3.right, 360f, 1.5f).SetEase(SimpleTweenType.easeOutQuad)
            .SetLoop(-1);

        Label($"color + easeInOutSine", new Vector3(0f, y, 0f));
        var colorObj = CreateCube(Color.white, new Vector3(0f, y - 0.5f, 0f));
        SimpleTweenEx.color(colorObj, Color.red, 1.0f).SetEase(SimpleTweenType.easeInOutSine)
            .SetLoopPingPong(-1);

        Label($"alpha + easeInOutSine", new Vector3(6f, y, 0f));
        var alphaObj = CreateCube(Color.white, new Vector3(6f, y - 0.5f, 0f));
        SimpleTweenEx.alpha(alphaObj, 0.2f, 1.0f).SetEase(SimpleTweenType.easeInOutSine)
            .SetLoopPingPong(-1);

        y -= 2.5f;

        // ---------- delayedCall ----------
        Label($"delayedCall chain", new Vector3(-6f, y, 0f));
        var dc = CreateSphere(Color.gray, new Vector3(-6f, y - 0.5f, 0f));
        dc.transform.localScale = Vector3.one * 0.6f;
        SimpleTween.delayedCall(dc, 1.0f, () =>
        {
            SimpleTweenEx.scale(dc, Vector3.one * 1.2f, 0.3f).SetEase(SimpleTweenType.easeOutBack);
            SimpleTween.delayedCall(dc, 0.5f, () =>
                SimpleTweenEx.scale(dc, Vector3.one * 0.6f, 0.3f).SetEase(SimpleTweenType.easeInBack));
        });

        // ---------- AppendTween ----------
        Label($"AppendTween chain", new Vector3(0f, y, 0f));
        var chain = CreateCube(new Color(0.5f, 0.3f, 0.8f), new Vector3(0f, y - 0.5f, 0f));
        var m1 = SimpleTweenEx.move(chain, new Vector3(3f, y - 0.5f, 0f), 0.8f).SetEase(SimpleTweenType.easeOutQuad);
        var m2 = SimpleTweenEx.move(chain, new Vector3(0f, y - 0.5f, 0f), 0.8f).SetEase(SimpleTweenType.easeInQuad);
        m1.AppendTween(m2).SetLoop(-1);
    }

    // ==============================================================
    // 辅助
    // ==============================================================

    private static List<SimpleTweenType> GetAllEaseTypes()
    {
        return new List<SimpleTweenType>
        {
            SimpleTweenType.easeLinear,
            SimpleTweenType.easeInQuad,       SimpleTweenType.easeOutQuad,      SimpleTweenType.easeInOutQuad,
            SimpleTweenType.easeInCubic,      SimpleTweenType.easeOutCubic,     SimpleTweenType.easeInOutCubic,
            SimpleTweenType.easeInQuart,      SimpleTweenType.easeOutQuart,     SimpleTweenType.easeInOutQuart,
            SimpleTweenType.easeInQuint,      SimpleTweenType.easeOutQuint,     SimpleTweenType.easeInOutQuint,
            SimpleTweenType.easeInSine,       SimpleTweenType.easeOutSine,      SimpleTweenType.easeInOutSine,
            SimpleTweenType.easeInExpo,       SimpleTweenType.easeOutExpo,      SimpleTweenType.easeInOutExpo,
            SimpleTweenType.easeInCirc,       SimpleTweenType.easeOutCirc,      SimpleTweenType.easeInOutCirc,
            SimpleTweenType.easeInBounce,     SimpleTweenType.easeOutBounce,    SimpleTweenType.easeInOutBounce,
            SimpleTweenType.easeInBack,       SimpleTweenType.easeOutBack,      SimpleTweenType.easeInOutBack,
            SimpleTweenType.easeInElastic,    SimpleTweenType.easeOutElastic,   SimpleTweenType.easeInOutElastic,
            SimpleTweenType.easeSpring,       SimpleTweenType.easeShake,        SimpleTweenType.punch,
        };
    }

    private void Label(string text, Vector3 worldPos)
    {
        var go = new GameObject("_Label");
        go.transform.position = worldPos;
        var label = go.AddComponent<ExampleLabel>();
        label.text = text;
        label.color = Color.white;
        label.fontSize = 10;
        label.offset = Vector3.up * 0.6f;
    }

    private static GameObject CreateCube(Color color, Vector3 pos)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = pos;
        go.GetComponent<Renderer>().material.color = color;
        return go;
    }

    private static GameObject CreateSphere(Color color, Vector3 pos)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = pos;
        go.GetComponent<Renderer>().material.color = color;
        return go;
    }
}
