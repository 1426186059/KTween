using UnityEngine;

public class KTweenTest : MonoBehaviour
{
    private void Start()
    {
        KTween.Handle mHandle = new KTween.Handle();

        mHandle.AppendTween(KTween.AddTween(gameObject, 10.0f, (fTimePercent) =>
        {
            gameObject.transform.localPosition = KTweenFunc.easeLinear(Vector3.zero, Vector3.one * 10, fTimePercent);
        }).GetHandle());

        mHandle.AppendTween(KTween.AddTween(gameObject, 10.0f, (fTimePercent) =>
        {
            gameObject.transform.localPosition = KTweenFunc.easeLinear(Vector3.zero, Vector3.zero, fTimePercent);
        }).GetHandle());


    }
}