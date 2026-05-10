using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class DevTools
{
    public static IEnumerator Animar(Image image, float targetAlpha, float duration, AnimationCurve curve)
    {
        float startAlpha = image.color.a;
        float time = 0f;

        while (time < duration)
        {
            float t = Mathf.Clamp01(time / duration);
            float curveValue = curve.Evaluate(t);

            float alpha = Mathf.LerpUnclamped(startAlpha, targetAlpha, curveValue);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
        yield return null;
    }
    
}
