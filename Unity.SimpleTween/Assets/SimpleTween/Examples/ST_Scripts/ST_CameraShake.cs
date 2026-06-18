using UnityEngine;

public class ST_CameraShake : MonoBehaviour
{
    private void Start()
    {
        var avatarBig = GameObject.Find("AvatarBig");
        if (avatarBig != null) BigGuyJump(avatarBig);
    }

    private void BigGuyJump(GameObject avatar)
    {
        Vector3 basePos = avatar.transform.position;
        float height = Random.Range(2f, 4f);

        SimpleTween.AddTween(avatar, 1f, t =>
        {
            avatar.transform.position = SimpleTweenFunc.easeLinear(basePos, basePos + Vector3.up * height, EaseInOutQuad(t));
        });

        SimpleTween.delayedCall(avatar, 1f, () =>
        {
            SimpleTween.AddTween(avatar, 0.27f, t =>
            {
                avatar.transform.position = SimpleTweenFunc.easeLinear(basePos + Vector3.up * height, basePos, t * t);
            });

            SimpleTween.delayedCall(avatar, 0.3f, () =>
            {
                DoCameraShake(height);
                SimpleTween.delayedCall(gameObject, 2f, () =>
                {
                    if (avatar != null) BigGuyJump(avatar);
                });
            });
        });
    }

    private void DoCameraShake(float height)
    {
        if (Camera.main == null) return;
        float amount = height * 0.2f;
        Vector3 startRot = Camera.main.transform.eulerAngles;

        SimpleTween.AddTween(Camera.main.gameObject, 0.42f, t =>
        {
            float decay = 1f - t * 0.5f;
            float offset = Mathf.Sin(t * Mathf.PI * 8f) * amount * decay;
            Camera.main.transform.eulerAngles = startRot + new Vector3(offset, 0, 0);
        }).SetLoopPingPong(3);
    }

    private static float EaseInOutQuad(float t) => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
}
