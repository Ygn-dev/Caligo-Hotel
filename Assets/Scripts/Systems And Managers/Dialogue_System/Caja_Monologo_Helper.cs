using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class Caja_Monologo_Helper : MonoBehaviour, ICaja_De_Texto_Helper
{
    public RectTransform rectPrefab;
    public RectTransform rectCaja;
    public RectTransform rectTexto;
    public CanvasGroup canvasGroup;
    public TMP_Text textoTMP;

    public void SetTexto(string texto)
    {
        textoTMP.text = texto;
    }

    public void ActualizarLayouts()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTexto);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectCaja);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectPrefab);
    }

    public IEnumerator MostrarCaja(float duracion, AnimationCurve curva, InputAction acceptAction)
    {
        float tiempo = 0;
        while (tiempo < duracion)
        {
            if(acceptAction.triggered) break;

            float valor = curva.Evaluate(tiempo / duracion);
            canvasGroup.alpha = valor;
            tiempo += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public TMP_Text GetTextoComponent()
    {
        return textoTMP;
    }
}
