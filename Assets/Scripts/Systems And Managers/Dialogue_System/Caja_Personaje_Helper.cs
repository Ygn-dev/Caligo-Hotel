using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Caja_Personaje_Helper : MonoBehaviour, ICaja_De_Texto_Helper
{
    public RectTransform rectPrefab;
    public RectTransform rectTexto;
    public CanvasGroup canvasGroup;
    public TMP_Text textoTMP;

    public TMP_Text GetTextoComponent()
    {
        return textoTMP;
    }

    public void SetTexto(string texto)
    {
        textoTMP.text = texto;
    }

    public void ActualizarLayouts()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTexto);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectPrefab);
    }

    public IEnumerator MostrarCaja(float duracion, AnimationCurve curva)
    {
        float tiempo = 0;
        while (tiempo < duracion)
        {
            float valor = curva.Evaluate(tiempo / duracion);
            canvasGroup.alpha = valor;
            
            tiempo += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

}
