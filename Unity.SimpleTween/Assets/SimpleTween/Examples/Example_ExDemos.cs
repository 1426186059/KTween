using UnityEngine;

/// <summary>
/// SimpleTweenEx 可视化演示 — 运行时自动展示所有 Ex 扩展方法
/// </summary>
public class Example_ExDemos : MonoBehaviour
{
    private void Start()
    {
        // ---------- move / moveX / moveY / moveZ ----------
        var cube = CreateCube(Color.cyan, new Vector3(-6f, 3f, 0f));
        SimpleTweenEx.move(cube, new Vector3(-3f, 3f, 0f), 1.2f)
            .SetEase(SimpleTweenType.easeOutBounce)
            .SetLoopPingPong(-1);

        var cubeX = CreateCube(Color.blue, new Vector3(-6f, 1f, 0f));
        SimpleTweenEx.moveX(cubeX, -3f, 1.0f, SimpleTweenType.easeInOutQuad)
            .SetLoopPingPong(-1);

        var cubeY = CreateCube(Color.green, new Vector3(-6f, -1f, 0f));
        SimpleTweenEx.moveY(cubeY, 1f, 0.8f, SimpleTweenType.easeInOutCubic)
            .SetLoopPingPong(-1);

        var cubeZ = CreateCube(Color.yellow, new Vector3(-6f, -3f, 0f));
        SimpleTweenEx.moveZ(cubeZ, 4f, 1.0f, SimpleTweenType.easeOutElastic)
            .SetLoopPingPong(-1);

        // ---------- moveLocal ----------
        var local = CreateSphere(Color.magenta, new Vector3(0f, 3f, 0f));
        var parent = new GameObject("MoveLocalParent");
        parent.transform.position = new Vector3(0f, 3f, 0f);
        local.transform.SetParent(parent.transform);
        SimpleTweenEx.moveLocal(local, new Vector3(2f, 0f, 0f), 1.2f, SimpleTweenType.easeOutQuad)
            .SetLoopPingPong(-1);

        // ---------- scale ----------
        var scaleObj = CreateSphere(Color.red, new Vector3(0f, 0f, 0f));
        SimpleTweenEx.scale(scaleObj, new Vector3(2f, 2f, 2f), 1.0f, SimpleTweenType.easeInOutBack)
            .SetLoopPingPong(-1);

        // ---------- rotateAround ----------
        var rot = CreateCube(new Color(1f, 0.5f, 0f), new Vector3(0f, -3f, 0f));
        SimpleTweenEx.rotateAround(rot, Vector3.up, 360f, 2.0f, SimpleTweenType.easeLinear)
            .SetLoop(-1);

        // ---------- rotateAroundLocal ----------
        var rotLocal = CreateCube(Color.gray, new Vector3(5f, 3f, 0f));
        SimpleTweenEx.rotateAroundLocal(rotLocal, Vector3.right, 360f, 1.5f, SimpleTweenType.easeOutQuad)
            .SetLoop(-1);

        // ---------- color ----------
        var colorObj = CreateCube(Color.white, new Vector3(5f, 0f, 0f));
        SimpleTweenEx.color(colorObj, Color.red, 1.0f, SimpleTweenType.easeInOutSine)
            .SetLoopPingPong(-1);

        // ---------- alpha ----------
        var alphaObj = CreateCube(Color.white, new Vector3(5f, -3f, 0f));
        SimpleTweenEx.alpha(alphaObj, 0.2f, 1.0f, SimpleTweenType.easeInOutSine)
            .SetLoopPingPong(-1);

        // ---------- delayedCall ----------
        var dc = CreateSphere(Color.gray, new Vector3(-3f, -3f, 0f));
        dc.transform.localScale = Vector3.one * 0.6f;
        SimpleTween.delayedCall(dc, 1.0f, () =>
        {
            SimpleTweenEx.scale(dc, Vector3.one * 1.2f, 0.3f, SimpleTweenType.easeOutBack)
                .SetEase(SimpleTweenType.easeOutBack);
            SimpleTween.delayedCall(dc, 0.5f, () =>
                SimpleTweenEx.scale(dc, Vector3.one * 0.6f, 0.3f).SetEase(SimpleTweenType.easeInBack));
        });

        // ---------- 链式调用 ----------
        var chain = CreateCube(new Color(0.5f, 0.3f, 0.8f), new Vector3(-3f, 0f, 0f));
        var m1 = SimpleTweenEx.move(chain, new Vector3(0f, 0f, 0f), 0.8f, SimpleTweenType.easeOutQuad);
        var m2 = SimpleTweenEx.move(chain, new Vector3(-3f, 0f, 0f), 0.8f, SimpleTweenType.easeInQuad);
        m1.AppendTween(m2).SetLoop(-1);
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
