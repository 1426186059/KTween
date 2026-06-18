using UnityEngine;

public class SimpleTweenTest : MonoBehaviour
{
    private void Start()
    {
        var mTweenItem1 = SimpleTween.AddTween(gameObject, 10.0f, (fTimePercent) =>
        {
            gameObject.transform.localPosition = SimpleTweenFunc.easeLinear(Vector3.zero, Vector3.one * 10, fTimePercent);
        }).GetHandle();
    }
}