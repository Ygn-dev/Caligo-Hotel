using UnityEngine;
using System.Collections;
public class UIAnimatePanel : MonoBehaviour
{
    public AnimationCurve curvaAparicion;
    public Vector3 posicionOculto = new Vector3(0f, 1500f, 0f);
    public Vector3 posicionVisible = new Vector3(0f, 0f, 0f);
    public float duracion = 0.5f;
    private RectTransform rectTransform;
    private Coroutine corrutinaActual;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = posicionOculto;
    }

    void Start()
    {

    }

    public void Desplegar()
    {
        transform.SetAsLastSibling();
        if (corrutinaActual != null)
        {
            StopCoroutine(corrutinaActual);
        }
        corrutinaActual = StartCoroutine(Animar(posicionVisible));
    }
    public void Ocultar()
    {
        if (corrutinaActual != null)
        {
            StopCoroutine(corrutinaActual);
        }
        corrutinaActual = StartCoroutine(Animar(posicionOculto));
    }
    private IEnumerator Animar(Vector3 posicionFinal)
    {
        //valores iniciales
        Vector3 posPrev = rectTransform.localPosition;
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.unscaledDeltaTime;   //Usar unscaled delta time porque el tiempo está pausado
            float t = Mathf.Clamp01(tiempoTranscurrido / duracion);
            float curvaT = curvaAparicion.Evaluate(t);
            rectTransform.localPosition = Vector3.LerpUnclamped(posPrev, posicionFinal, curvaT);
            yield return null;
        }
        rectTransform.localPosition = posicionFinal;
    }
}
