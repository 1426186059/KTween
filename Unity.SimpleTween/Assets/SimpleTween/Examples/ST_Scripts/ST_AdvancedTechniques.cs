using UnityEngine;

public class ST_AdvancedTechniques : MonoBehaviour
{
    public Transform[] movePts;

    private void Start()
    {
        // Alpha recursive
        var avatarRecursive = GameObject.Find("AvatarRecursive");
        if (avatarRecursive != null)
        {
            Renderer[] allRenderers = avatarRecursive.GetComponentsInChildren<Renderer>();
            foreach (var r in allRenderers)
            {
                Color c = r.material.color;
                SimpleTween.AddTween(r.gameObject, 1f, t =>
                {
                    r.material.color = new Color(c.r, c.g, c.b, 1f - t);
                }).SetLoopPingPong(-1);
            }
        }

        // Move along path points
        if (movePts != null && movePts.Length > 1)
        {
            var avatarMove = GameObject.Find("AvatarMove");
            if (avatarMove != null)
            {
                SimpleTween.AddTween(avatarMove, 5f, t =>
                {
                    float val = t * (movePts.Length - 1);
                    int first = Mathf.FloorToInt(val);
                    int next = Mathf.Min(first + 1, movePts.Length - 1);
                    float diff = val - first;
                    avatarMove.transform.position = Vector3.Lerp(movePts[first].position, movePts[next].position, diff);
                }).SetLoopPingPong(-1);
            }

            // Move path points themselves
            for (int i = 0; i < movePts.Length; i++)
            {
                int idx = i;
                Vector3 basePos = movePts[i].position;
                SimpleTween.AddTween(movePts[i].gameObject, 1f, t =>
                {
                    movePts[idx].position = SimpleTweenFunc.easeLinear(basePos, basePos + Vector3.up * 1.5f, t);
                }).SetDelay(i * 0.2f).SetLoopPingPong(-1);
            }
        }
    }
}
