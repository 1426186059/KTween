using UnityEngine;

public class ST_Sequencer : MonoBehaviour
{
    private void Start()
    {
        var avatar = GameObject.Find("Avatar1");
        if (avatar == null) return;
        var star = GameObject.Find("Star");

        Vector3 basePos = avatar.transform.position;
        Vector3 peakPos = basePos + Vector3.up * 6f;

        var jump = SimpleTween.AddTween(avatar, 1f, t =>
        {
            avatar.transform.position = SimpleTweenFunc.easeLinear(basePos, peakPos, EaseOutQuad(t));
        });

        var spin = SimpleTween.AddTween(avatar, 0.6f, t =>
        {
            avatar.transform.eulerAngles = SimpleTweenFunc.easeLinear(Vector3.zero, new Vector3(0, 0, 360f), BackIn(t));
        });

        var land = SimpleTween.AddTween(avatar, 1f, t =>
        {
            avatar.transform.position = SimpleTweenFunc.easeLinear(peakPos, basePos, t * t);
        });

        jump.AppendTween(spin);
        spin.AppendTween(land);

        if (star != null)
        {
            Vector3 sScale = star.transform.localScale;
            Color sColor = star.GetComponent<Renderer>().material.color;
            SimpleTween.AddTween(star, 1f, t =>
            {
                star.transform.localScale = SimpleTweenFunc.easeLinear(sScale, sScale * 3f, t);
                star.GetComponent<Renderer>().material.color = new Color(sColor.r, sColor.g, sColor.b, 1f - t);
            });
            SimpleTween.AddTween(star, 0.5f, t =>
            {
                star.transform.localScale = SimpleTweenFunc.easeLinear(sScale * 3f, sScale, t);
                star.GetComponent<Renderer>().material.color = new Color(sColor.r, sColor.g, sColor.b, t);
            }).SetDelay(1f);
        }
    }

    private static float EaseOutQuad(float t) => t * (2f - t);
    private static float BackIn(float t) { const float c1 = 1.70158f; return (c1 + 1f) * t * t * t - c1 * t * t; }
}
