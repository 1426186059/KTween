using UnityEngine;

public class ST_LogoCinematic : MonoBehaviour
{
    private void Start()
    {
        var tween = GameObject.Find("Tween");
        var lean = GameObject.Find("Lean");

        if (tween != null)
        {
            Vector3 tweenStart = tween.transform.localPosition;
            tween.transform.localPosition = tweenStart - Vector3.right * 15f;
            SimpleTween.AddTween(tween, 0.4f, t =>
            {
                tween.transform.localPosition = SimpleTweenFunc.easeLinear(tweenStart - Vector3.right * 15f, tweenStart, t);
            });

            Vector3 tweenRotStart = tween.transform.eulerAngles;
            tween.transform.RotateAround(tween.transform.position, Vector3.forward, -30f);
            SimpleTween.AddTween(tween, 0.4f, t =>
            {
                tween.transform.eulerAngles = SimpleTweenFunc.easeLinear(tweenRotStart - new Vector3(0, 0, 30f), tweenRotStart, EaseInQuad(t));
            }).SetDelay(0.4f);
        }

        if (lean != null)
        {
            Vector3 leanBase = lean.transform.position;
            lean.transform.position = leanBase + Vector3.up * 5.1f;
            SimpleTween.AddTween(lean, 0.6f, t =>
            {
                lean.transform.position = SimpleTweenFunc.easeLinear(leanBase + Vector3.up * 5.1f, leanBase, EaseInQuad(t));
            }).SetDelay(0.6f);
        }
    }

    private static float EaseInQuad(float t) => t * t;
}
