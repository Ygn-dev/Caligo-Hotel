using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DialogueScroll_Helper : MonoBehaviour
{
    public CanvasGroup scrollCanvasGroup;
    public GameObject content;   
    public GameObject leyenda;
    public TMP_Text leyendaText;
    public ScrollRect scrollRect;
    public Image scrollImageBackground;

    private bool isAnimating = false;
    private Coroutine currentCoroutine;
    

    public void MostrarScroll(float duration, AnimationCurve curve)
    {
        if(isAnimating) {
            // Si ya hay una animación en curso, detenerla antes de iniciar una nueva
            StopCoroutine(currentCoroutine);
            isAnimating = false;
        }
        currentCoroutine = StartCoroutine(AnimateScroll(1f, scrollCanvasGroup, duration, curve));
    }

    public IEnumerator OcultarScrollBar(float duration, AnimationCurve curve)
    {
        if(isAnimating) {
            // Si ya hay una animación en curso, detenerla antes de iniciar una nueva
            StopCoroutine(currentCoroutine);
            isAnimating = false;
        }
        currentCoroutine = StartCoroutine(AnimateScroll(0f, scrollCanvasGroup, duration, curve));
        yield return currentCoroutine;
    }


    private IEnumerator AnimateScroll(float targetAlpha, CanvasGroup canvasGroup, float duration, AnimationCurve curve)
    {
        isAnimating = true;
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        float subtime = 0;
        while(subtime < 0.5f)
        {
            subtime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float curveValue = curve.Evaluate(t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, curveValue);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        isAnimating = false;
        yield return null;
    }
}
