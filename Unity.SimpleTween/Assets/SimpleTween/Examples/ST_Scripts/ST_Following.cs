using UnityEngine;

public class ST_Following : MonoBehaviour
{
    private void Start()
    {
        var arrow = GameObject.Find("FollowArrow");
        if (arrow == null) return;

        Vector3 arrowBase = arrow.transform.position;
        Vector3 arrowScale = arrow.transform.localScale;

        // 引导箭头上下 + 颜色变化
        SimpleTween.AddTween(arrow, 3f, t =>
        {
            arrow.transform.localPosition = arrowBase + Vector3.up * Mathf.Sin(t * Mathf.PI * 2f) * 100f;
        }).SetLoop(-1);

        SimpleTween.delayedCall(arrow, 3f, () =>
        {
            arrow.transform.localScale = arrowScale * Random.Range(5f, 10f);
            var r = arrow.GetComponent<Renderer>();
            if (r != null) r.material.color = new Color(Random.value, Random.value, Random.value);
        }).SetLoop(-1);

        // 跟随者：使用 SmoothDamp
        var followers = new[] { "Dude1", "Dude2", "Dude3", "Dude4", "Dude5" };
        foreach (var name in followers)
        {
            var f = GameObject.Find(name);
            if (f != null) StartFollow(f, arrow.transform);
        }
    }

    private void StartFollow(GameObject follower, Transform target)
    {
        // 用协程模拟跟随
        System.Collections.IEnumerator Follow()
        {
            Vector3 vel = Vector3.zero;
            while (follower != null && target != null)
            {
                Vector3 targetPos = target.position;
                Vector3 currentPos = follower.transform.position;
                currentPos.y = Mathf.SmoothDamp(currentPos.y, targetPos.y, ref vel.y, 1.1f, 50f);
                follower.transform.position = currentPos;

                // 跟随颜色
                var r = follower.GetComponent<Renderer>();
                if (r != null && target.GetComponent<Renderer>() != null)
                {
                    r.material.color = Color.Lerp(r.material.color, target.GetComponent<Renderer>().material.color, Time.deltaTime * 1.1f);
                }

                // 跟随缩放
                follower.transform.localScale = Vector3.Lerp(follower.transform.localScale, target.localScale, Time.deltaTime * 1.1f);

                yield return null;
            }
        }
        StartCoroutine(Follow());
    }
}
