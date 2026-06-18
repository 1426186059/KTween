using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LeanTween GeneralSimpleUI / GeneralUISpace 风格示例
/// 演示 SimpleTween 在 UI Canvas 元素上的动画效果
/// </summary>
public class LT_Style_UI : MonoBehaviour
{
    public RectTransform button;
    public RectTransform panel;
    public Text uiText;

    private void Start()
    {
        if (button == null)
        {
            Debug.LogWarning("请将一个 UI Button 的 RectTransform 赋给 button 变量");
            return;
        }

        // 1. 按钮位置移动
        Vector2 fromPos = button.anchoredPosition;
        Vector2 toPos = fromPos + new Vector2(200, 100);
        SimpleTween.AddTween(button.gameObject, 1f, (t) =>
        {
            button.anchoredPosition = SimpleTweenFunc.easeLinear(fromPos, toPos, t);
        });

        // 2. 按钮缩放
        Vector3 fromScale = button.localScale;
        Vector3 toScale = fromScale * 2f;
        SimpleTween.AddTween(button.gameObject, 1f, (t) =>
        {
            float eased = EaseOutElastic(t);
            button.localScale = SimpleTweenFunc.easeLinear(fromScale, toScale, eased);
        }).SetDelay(3f).SetLoopPingPong(-1);

        // 3. 按钮旋转
        Vector3 fromRot = button.eulerAngles;
        Vector3 toRot = fromRot + new Vector3(0, 0, 360f);
        SimpleTween.AddTween(button.gameObject, 0.8f, (t) =>
        {
            button.eulerAngles = SimpleTweenFunc.easeLinear(fromRot, toRot, t);
        }).SetDelay(4f);

        // 4. Panel 透明度（如果有 CanvasGroup）
        var cg = panel?.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            SimpleTween.AddTween(panel.gameObject, 1.5f, (t) =>
            {
                cg.alpha = t;
            }).SetDelay(1f).SetLoopPingPong(-1);
        }

        // 5. 文本颜色渐变
        if (uiText != null)
        {
            Color fromColor = uiText.color;
            Color toColor = new Color(133f / 255f, 145f / 255f, 223f / 255f);
            SimpleTween.AddTween(uiText.gameObject, 0.6f, (t) =>
            {
                uiText.color = Color.Lerp(fromColor, toColor, t);
            }).SetDelay(0.6f).SetLoopPingPong(-1);

            // 文字逐个显示（模拟打字效果）
            string fullText = uiText.text;
            uiText.text = "";
            SimpleTween.AddTween(gameObject, 3f, (t) =>
            {
                int len = Mathf.RoundToInt(t * fullText.Length);
                uiText.text = fullText.Substring(0, len);
            }).SetDelay(2f);
        }
    }

    private static float EaseOutElastic(float t)
    {
        t = Mathf.Clamp01(t);
        if (t <= 0) return 0;
        if (t >= 1) return 1;
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * 2.0943951f) + 1f;
    }
}
