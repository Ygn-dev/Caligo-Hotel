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

    private Scrollbar scrollbarRef;
    private Coroutine mostrarCoroutine;
    private Coroutine ocultarCoroutine;

    private void Awake()
    {
        scrollbarRef = scrollRect.verticalScrollbar;
    }

    public void MostrarScrollBar(float duration, AnimationCurve curve, float delay)
    {
        // Reinicia el tiempo de espera
        if (mostrarCoroutine != null) StopCoroutine(mostrarCoroutine);          
        mostrarCoroutine = StartCoroutine(Mostrar(duration, curve, delay));
    }

    public void OcultarScrollBar(float duration, AnimationCurve curve)
    {
        // Cancela cualquier mostrar pendiente
        if (mostrarCoroutine != null)
        {
            StopCoroutine(mostrarCoroutine);
            mostrarCoroutine = null;
        }

        // Reinicia la animación de ocultar
        if (ocultarCoroutine != null) StopCoroutine(ocultarCoroutine);
        ocultarCoroutine = StartCoroutine(Ocultar(duration, curve));
    }

    private IEnumerator Mostrar(float duration, AnimationCurve curve, float delay)
    {
        yield return new WaitForSeconds(delay);

        scrollRect.verticalScrollbar = scrollbarRef;

        float elapsed = 0f;
        float startAlpha = scrollCanvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            scrollCanvasGroup.alpha = Mathf.Lerp(startAlpha,1f,curve.Evaluate(t));
            yield return null;
        }

        scrollCanvasGroup.alpha = 1f;
        mostrarCoroutine = null;
    }

    private IEnumerator Ocultar(float duration, AnimationCurve curve)
    {
        scrollRect.verticalScrollbar = null;

        float elapsed = 0f;
        float startAlpha = scrollCanvasGroup.alpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            scrollCanvasGroup.alpha = Mathf.Lerp(startAlpha,0f,curve.Evaluate(t));
            yield return null;
        }

        scrollCanvasGroup.alpha = 0f;
        ocultarCoroutine = null;
    }
}
