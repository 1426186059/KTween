using UnityEngine;

public class ST_GeneralBasics2D : MonoBehaviour
{
    private void Start()
    {
        var avatarRotate = GameObject.Find("avatarRotate") ?? CreateSprite("avatarRotate", new Vector3(-2.5f, 2, 0));
        var avatarScale = GameObject.Find("avatarScale") ?? CreateSprite("avatarScale", new Vector3(2.5f, 2, 0));
        var avatarMove = GameObject.Find("avatarMove") ?? CreateSprite("avatarMove", new Vector3(-3f, 0, 0));

        Vector3 rStart = avatarRotate.transform.eulerAngles;
        SimpleTween.AddTween(avatarRotate, 5f, t => avatarRotate.transform.eulerAngles = SimpleTweenFunc.easeLinear(rStart, rStart + new Vector3(0, 0, -360f), t)).SetLoop(-1);

        Vector3 sScale = avatarScale.transform.localScale;
        Vector3 sPos = avatarScale.transform.position;
        SimpleTween.AddTween(avatarScale, 5f, t => { float e = EaseOutBounce(t); avatarScale.transform.localScale = SimpleTweenFunc.easeLinear(sScale, sScale * 1.7f, e); avatarScale.transform.position = SimpleTweenFunc.easeLinear(sPos, sPos + Vector3.right * 1f, e); });

        Vector3 mPos = avatarMove.transform.position;
        SimpleTween.AddTween(avatarMove, 2f, t => avatarMove.transform.position = SimpleTweenFunc.easeLinear(mPos, mPos + new Vector3(1.7f, 0, 0), t * t));
        SimpleTween.AddTween(avatarMove, 2f, t => avatarMove.transform.position = SimpleTweenFunc.easeLinear(mPos + new Vector3(1.7f, 0, 0), mPos + new Vector3(3.7f, -1f, 0), t * t)).SetDelay(3f);

        SimpleTween.AddTween(avatarScale, 1f, t => avatarScale.transform.localScale = SimpleTweenFunc.easeLinear(sScale * 1.7f, Vector3.one * 0.2f, EaseInOutCirc(t))).SetDelay(7f).SetLoopPingPong(3);
    }

    private GameObject CreateSprite(string name, Vector3 pos)
    {
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = new Color(0, 0.71f, 1);
        go.transform.position = pos;
        return go;
    }

    private static float EaseOutBounce(float t) { t = Mathf.Clamp01(t); const float n1 = 7.5625f, d1 = 2.75f; if (t < 1f / d1) return n1 * t * t; if (t < 2f / d1) { t -= 1.5f / d1; return n1 * t * t + 0.75f; } if (t < 2.5f / d1) { t -= 2.25f / d1; return n1 * t * t + 0.9375f; } t -= 2.625f / d1; return n1 * t * t + 0.984375f; }
    private static float EaseInOutCirc(float t) => t < 0.5f ? (1f - Mathf.Sqrt(1f - 4f * t * t)) / 2f : (Mathf.Sqrt(1f - (-2f * t + 2f) * (-2f * t + 2f)) + 1f) / 2f;
}
