using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public interface ICaja_De_Texto_Helper
{
    public void SetTexto(string texto);
    public void ActualizarLayouts();
    public IEnumerator MostrarCaja(float duracion, AnimationCurve curva, InputAction acceptAction);
    public TMP_Text GetTextoComponent();
}
