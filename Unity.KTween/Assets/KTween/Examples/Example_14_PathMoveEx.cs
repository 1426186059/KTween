using UnityEngine;

/// <summary>
/// 示例 14 — KTweenEx 路径数组移动演示 (XY平面)
/// move(path[])、moveBezier(path[])、moveLocal(path[])、moveLocalBezier(path[])
/// </summary>
public class Example_14_PathMoveEx : MonoBehaviour
{
    public float totalTime = 4f;
    public bool autoPlay = true;

    private void Start()
    {
        if (autoPlay) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        Vector3 o = transform.position;

        // ===== 1. move(path) — 菱形闭环, easeInOutQuad =====
        {
            float rowY = 2f;
            var path = new Vector3[] {
                new Vector3(-2.2f, rowY,      0f),
                new Vector3( 0f,   rowY + 1.8f, 0f),
                new Vector3( 2.2f, rowY,      0f),
                new Vector3( 0f,   rowY - 1.8f, 0f),
                new Vector3(-2.2f, rowY,      0f),
            };
            MakePathMarkers(path, Color.cyan, o);
            var ball = MakeBall(Color.cyan, o + path[0], "菱");
            KTweenEx.move(ball, path, totalTime).SetEase(KTweenType.easeInOutQuad).SetLoop(-1);
        }

        // ===== 2. move(path) — 锯齿波 zigzag, easeOutBounce =====
        {
            float rowY = 2f;
            var path = new Vector3[] {
                new Vector3(-2f, rowY - 0.6f, 0f),
                new Vector3(-1f, rowY + 0.8f, 0f),
                new Vector3( 0f, rowY - 0.6f, 0f),
                new Vector3( 1f, rowY + 0.8f, 0f),
                new Vector3( 2f, rowY - 0.6f, 0f),
            };
            MakePathMarkers(path, Color.yellow, o);
            var ball = MakeBall(Color.yellow, o + path[0], "锯齿");
            KTweenEx.move(ball, path, 2.5f).SetEase(KTweenType.easeOutBounce).SetLoopPingPong(-1);
        }

        // ===== 3. move(path) — 正八边形拟圆, linear =====
        {
            float rowY = 0f;
            int n = 8;
            float r = 1.6f;
            var path = new Vector3[n + 1];
            for (int i = 0; i <= n; i++)
            {
                float angle = Mathf.PI * 2f * i / n - Mathf.PI / 2f;
                path[i] = new Vector3(Mathf.Cos(angle) * r, rowY + Mathf.Sin(angle) * r, 0f);
            }
            MakePathMarkers(path, Color.blue, o);
            var ball = MakeBall(Color.blue, o + path[0], "圆");
            KTweenEx.move(ball, path, totalTime).SetEase(KTweenType.linear).SetLoop(-1);
        }

        // ===== 4. moveBezier(path) — 单段 U 型贝塞尔, 4点 =====
        {
            float rowY = 0f;
            var path = new Vector3[] {
                new Vector3(-2f, rowY - 0.8f, 0f),  // P0
                new Vector3(-2f, rowY + 1.2f, 0f),  // C1  (垂直向上控制)
                new Vector3( 2f, rowY + 1.2f, 0f),  // C2
                new Vector3( 2f, rowY - 0.8f, 0f),  // P1
            };
            MakeBezierMarkers(path, o);
            var ball = MakeBall(Color.magenta, o + path[0], "U贝");
            KTweenEx.moveBezier(ball, path, 3f).SetEase(KTweenType.linear).SetLoopPingPong(-1);
        }

        // ===== 5. moveBezier(path) — S 曲线, 7点 2段 =====
        {
            float rowY = -2.5f;
            var path = new Vector3[] {
                new Vector3(-2.5f, rowY,       0f),  // P0
                new Vector3(-1f,   rowY - 1.5f, 0f),  // C1
                new Vector3( 0.5f, rowY - 1f,   0f),  // C2
                new Vector3( 0.5f, rowY,        0f),  // P1 (中点)
                new Vector3( 0.5f, rowY + 1f,   0f),  // C1
                new Vector3( 2f,   rowY + 1.5f, 0f),  // C2
                new Vector3( 2.5f, rowY,        0f),  // P2
            };
            MakeBezierMarkers(path, o);
            var ball = MakeBall(Color.red, o + path[0], "S贝");
            KTweenEx.moveBezier(ball, path, totalTime).SetEase(KTweenType.linear).SetLoopPingPong(-1);
        }

        // ===== 6. moveBezier(path) — 闭环贝塞尔花形, 7点 2段首尾相接 =====
        {
            float rowY = -2.5f;
            float rx = 2f, ry = 1.2f; // 椭圆
            var path = new Vector3[] {
                new Vector3(-rx, rowY,        0f),  // left
                new Vector3(-rx, rowY + ry,   0f),
                new Vector3( rx, rowY + ry,   0f),
                new Vector3( rx, rowY,        0f),  // right
                new Vector3( rx, rowY - ry,   0f),
                new Vector3(-rx, rowY - ry,   0f),
                new Vector3(-rx, rowY,        0f),  // back to left
            };
            MakeBezierMarkers(path, o);
            var ball = MakeBall(new Color(1f, 0.5f, 0f), o + path[0], "花贝");
            KTweenEx.moveBezier(ball, path, totalTime + 1f).SetEase(KTweenType.linear).SetLoop(-1);
        }

        // ===== 7. moveLocal(path) — 菱形 + 旋转父节点 =====
        {
            float rowY = -5f;
            var parent = new GameObject("LocalMove");
            parent.transform.position = o + new Vector3(0f, rowY, 0f);
            KTweenEx.rotateAround(parent, Vector3.forward, 360f, 6f).SetEase(KTweenType.linear).SetLoop(-1);

            var path = new Vector3[] {
                new Vector3(-1.8f,  0f,   0f),
                new Vector3( 0f,    1.6f, 0f),
                new Vector3( 1.8f,  0f,   0f),
                new Vector3( 0f,   -1.6f, 0f),
                new Vector3(-1.8f,  0f,   0f),
            };
            MakeLocalMarkers(parent.transform, path, Color.green);
            var ball = MakeChildBall(parent.transform, Color.green, path[0], "局菱");
            KTweenEx.moveLocal(ball, path, totalTime).SetEase(KTweenType.easeInOutQuad).SetLoop(-1);
        }

        // ===== 8. moveLocalBezier(path) — 闭环贝塞尔 + 反向旋转父节点 =====
        {
            float rowY = -5f;
            var parent = new GameObject("LocalBez");
            parent.transform.position = o + new Vector3(0f, rowY, 0f);
            KTweenEx.rotateAround(parent, Vector3.forward, -360f, 7f).SetEase(KTweenType.linear).SetLoop(-1);

            var path = new Vector3[] {
                new Vector3(-2f,  0f,   0f),
                new Vector3(-0.5f, 1.2f, 0f),
                new Vector3( 0.5f, 1.2f, 0f),
                new Vector3( 2f,   0f,   0f),
                new Vector3( 0.5f,-1.2f, 0f),
                new Vector3(-0.5f,-1.2f, 0f),
                new Vector3(-2f,  0f,   0f),
            };
            MakeLocalBezierMarkers(parent.transform, path);
            var ball = MakeChildBall(parent.transform, Color.white, path[0], "局贝");
            KTweenEx.moveLocalBezier(ball, path, totalTime + 1f).SetEase(KTweenType.linear).SetLoop(-1);
        }
    }

    // ==============================================================
    // 工厂方法
    // ==============================================================

    private static void MakePathMarkers(Vector3[] pts, Color c, Vector3 origin)
    {
        for (int i = 0; i < pts.Length; i++)
        {
            var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m.name = $"P_{i}";
            m.transform.position = origin + pts[i];
            m.transform.localScale = Vector3.one * 0.15f;
            m.GetComponent<Renderer>().material.color = i == pts.Length - 1 ? Color.grey : c;
            Object.Destroy(m.GetComponent<Collider>());
        }
    }

    private static void MakeBezierMarkers(Vector3[] pts, Vector3 origin)
    {
        for (int i = 0; i < pts.Length; i++)
        {
            bool isAnchor = (i % 3 == 0);
            var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m.name = $"B_{i}";
            m.transform.position = origin + pts[i];
            m.transform.localScale = Vector3.one * (isAnchor ? 0.2f : 0.1f);
            m.GetComponent<Renderer>().material.color = isAnchor ? Color.yellow : Color.grey;
            Object.Destroy(m.GetComponent<Collider>());
        }
    }

    private static void MakeLocalMarkers(Transform parent, Vector3[] pts, Color c)
    {
        for (int i = 0; i < pts.Length; i++)
        {
            var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m.name = $"LP_{i}";
            m.transform.SetParent(parent);
            m.transform.localPosition = pts[i];
            m.transform.localScale = Vector3.one * 0.15f;
            m.GetComponent<Renderer>().material.color = i == pts.Length - 1 ? Color.grey : c;
            Object.Destroy(m.GetComponent<Collider>());
        }
    }

    private static void MakeLocalBezierMarkers(Transform parent, Vector3[] pts)
    {
        for (int i = 0; i < pts.Length; i++)
        {
            bool isAnchor = (i % 3 == 0);
            var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m.name = $"LB_{i}";
            m.transform.SetParent(parent);
            m.transform.localPosition = pts[i];
            m.transform.localScale = Vector3.one * (isAnchor ? 0.2f : 0.1f);
            m.GetComponent<Renderer>().material.color = isAnchor ? Color.yellow : Color.grey;
            Object.Destroy(m.GetComponent<Collider>());
        }
    }

    private static GameObject MakeBall(Color c, Vector3 pos, string name)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * 0.35f;
        go.GetComponent<Renderer>().material.color = c;
        Object.Destroy(go.GetComponent<Collider>());
        return go;
    }

    private static GameObject MakeChildBall(Transform parent, Color c, Vector3 localPos, string name)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;
        go.transform.localScale = Vector3.one * 0.35f;
        go.GetComponent<Renderer>().material.color = c;
        Object.Destroy(go.GetComponent<Collider>());
        return go;
    }
}
