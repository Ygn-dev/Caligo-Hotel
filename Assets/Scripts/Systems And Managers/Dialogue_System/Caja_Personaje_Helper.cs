using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class Caja_Personaje_Helper : MonoBehaviour, ICaja_De_Texto_Helper
{
    public RectTransform rectPrefab;
    public RectTransform rectTexto;
    public Image characterImage;
    public TMP_Text textoTMP;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

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

    public IEnumerator MostrarCaja(float duracion, AnimationCurve curva, InputAction acceptAction)
    {
        SoundFX_Manager.Instance.PlaySound(SoundType.ABRIR_DIALOGO);
        animator.SetTrigger("Mostrar");

        float tiempo = 0;
        while (tiempo < duracion)
        {
            if(acceptAction.triggered)
            {
                characterImage.color = new Color(1, 1, 1, 1);
                animator.Play("Text_Box_Open",0,1f);
                break;
            }

            float valor = curva.Evaluate(tiempo / duracion);
            characterImage.color = new Color(1, 1, 1, valor);
            tiempo += Time.deltaTime;
            yield return null;
        }
    }

}
