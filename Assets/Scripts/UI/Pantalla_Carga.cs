using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pantalla_Carga : MonoBehaviour
{
    [SerializeField] private AnimationCurve curva;
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
    }
    public IEnumerator FadeIn(float duracion = 2.5f)
    {
        yield return DevTools.Animar(fadeImage, 1f, duracion, curva);
    }
    public IEnumerator FadeOut(float duracion = 2.5f)
    {
        yield return DevTools.Animar(fadeImage, 0f, duracion, curva);
    }
}
