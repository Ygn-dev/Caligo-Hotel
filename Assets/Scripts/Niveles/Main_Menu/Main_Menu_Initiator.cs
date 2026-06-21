using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class Main_Menu_Initiator : MonoBehaviour
{//314D79
    public GameObject menuBox;
    public GameObject logo;
    public GameObject boxTape;
    public GameObject textBox;
    public GameObject keyBox;
    public GameObject textMenu;
    public Image LogoInicial;
    public float duracionAparicion;
    public float duracionEspera;
    public float duracionDesaparicion;
    public AnimationCurve curvaAparicion;
    public InputActionAsset inputActionAsset;
    public Animator boxAnimator;
 

    void Start()
    {
        StartCoroutine(InicioDeMenu());
    }

    private IEnumerator InicioDeMenu()
    {
        yield return StartCoroutine(SetupInicial());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MostrarImagen(LogoInicial, duracionAparicion, duracionEspera, duracionDesaparicion));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MostrarCaja());
        //Lo demas sigue en Main_Menu_Animations
    }

    private IEnumerator SetupInicial()
    {
        foreach (var map in inputActionAsset.actionMaps) map.Disable();
        var mainMenuMap = inputActionAsset.FindActionMap("MainMenu");
        mainMenuMap.Enable();
        foreach (var action in mainMenuMap.actions) action.Disable();

        logo.SetActive(false);
        menuBox.SetActive(false);
        boxTape.SetActive(false);
        textMenu.SetActive(false);

        LogoInicial.gameObject.SetActive(true);
        boxAnimator.gameObject.SetActive(true);
        textBox.SetActive(true);
        keyBox.SetActive(true);

        Time.timeScale = 1f;
        yield return null;
    }

    private IEnumerator MostrarImagen(Image image, float duracionAparicion, float duracionEspera, float duracionDesaparicion)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        float targetAlpha = 1f;
        yield return DevTools.AnimarImage(image,targetAlpha,duracionAparicion,curvaAparicion);
        yield return new WaitForSeconds(duracionEspera);
        targetAlpha = 0f;
        yield return DevTools.AnimarImage(image,targetAlpha,duracionDesaparicion,curvaAparicion);
    }

    private IEnumerator MostrarCaja()
    {
        boxAnimator.SetTrigger("Aparecer");
        yield return null;
    }
}
