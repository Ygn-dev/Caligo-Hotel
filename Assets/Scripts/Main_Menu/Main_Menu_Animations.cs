using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Main_Menu_Animations : MonoBehaviour
{
    public Animator boxAnimator;
    public Animator textAnimator;
    public Animator llavesAnimator;
    public Animator boxTape;
    public GameObject mainMenu;
    public GameObject logoInicial;
    public GameObject logoFinal;
    public GameObject textMenu;
    public InputActionAsset inputActionAsset;
    public Initial_Button initialButtonScript;

    private InputAction accept;

    public void ContinuarAnimacion(float order)
    {
        switch (order)
        {
            case 0:
                textAnimator.SetTrigger("Aparicion");
                var gameplayMap = inputActionAsset.FindActionMap("MainMenu");
                accept = gameplayMap.FindAction("Accept");
                accept.Enable();
                accept.performed += ContinuarCaja;
                break;
            case 1:
                llavesAnimator.SetTrigger("Aparicion");
                break;
            case 2:
                //Desactivar los objetos iniciales
                logoInicial.SetActive(false);
                boxAnimator.gameObject.SetActive(false);
                textAnimator.gameObject.SetActive(false);

                //Continuar animacion
                boxTape.gameObject.SetActive(true);
                mainMenu.SetActive(true);
                boxTape.SetTrigger("Desaparicion");
                llavesAnimator.SetTrigger("Desaparicion");
                break;
            case 3:
                //Desactivar los objetos iniciales
                boxTape.gameObject.SetActive(false);
                llavesAnimator.gameObject.SetActive(false);

                logoFinal.SetActive(true);
                logoFinal.GetComponent<Animator>().SetTrigger("Aparicion");
                StartCoroutine(EsperarYAparecer(1f));
                break;
        }
    }

    private void ContinuarCaja(InputAction.CallbackContext context)
    {
        accept.performed -= ContinuarCaja;
        accept.Disable();
        textAnimator.SetTrigger("Desaparicion");
        boxAnimator.SetTrigger("Mover");
    }

    private IEnumerator EsperarYAparecer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        textMenu.SetActive(true);
        textMenu.GetComponent<Animator>().SetTrigger("Aparicion");
        initialButtonScript.MostrarPrimerBoton();
        var gameplayMap = inputActionAsset.FindActionMap("MainMenu");
        gameplayMap.FindAction("Move").Enable();
        accept.Enable();
    }
}
