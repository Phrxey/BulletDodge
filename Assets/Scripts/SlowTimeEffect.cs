using UnityEngine;
using UnityEngine.UI;

public class SlowTimeEffect : MonoBehaviour
{
    public Image blueOverlay;
    public Image vignetteOverlay;
    public Image rippleImage;
    public Transform player; // 飞船位置

    public float targetBlueAlpha = 0.25f;
    public float targetVignetteAlpha = 0.5f;
    public float fadeSpeed = 5f;

    private bool isSlowed = false;
    private bool overlayActive = false;

    void Update()
    {
        // 修复bug：同时检测Shift键和能量槽
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        isSlowed = pm != null && pm.IsSlowed();

        // 没有慢时间时恢复画面
        if (!isSlowed)
            overlayActive = false;

        float targetBlue = overlayActive ? targetBlueAlpha : 0f;
        float targetVignette = overlayActive ? targetVignetteAlpha : 0f;

        Color bc = blueOverlay.color;
        bc.a = Mathf.Lerp(bc.a, targetBlue, Time.unscaledDeltaTime * fadeSpeed);
        blueOverlay.color = bc;

        Color vc = vignetteOverlay.color;
        vc.a = Mathf.Lerp(vc.a, targetVignette, Time.unscaledDeltaTime * fadeSpeed);
        vignetteOverlay.color = vc;
    }

    public void TriggerRipple()
    {
        StartCoroutine(RippleEffect());
    }

    System.Collections.IEnumerator RippleEffect()
    {
        if (rippleImage == null) yield break;

        rippleImage.gameObject.SetActive(true);

        overlayActive = true;

        float duration = 0.6f;
        float elapsed = 0f;

        RectTransform rt = rippleImage.rectTransform;

        // 把圆圈起点设在飞船屏幕位置
        if (player != null)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(player.position);
            rt.position = screenPos;
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float maxSize = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height) * 5f;
            float size = Mathf.Lerp(0f, maxSize, t * t);
            rt.sizeDelta = new Vector2(size, size);

            Color c = rippleImage.color;
            c.a = Mathf.Lerp(0.3f, 0f, Mathf.Max(0f, (t - 0.95f) / 0.05f));
            rippleImage.color = c;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rippleImage.gameObject.SetActive(false);
        rt.sizeDelta = new Vector2(0, 0);
    }
}