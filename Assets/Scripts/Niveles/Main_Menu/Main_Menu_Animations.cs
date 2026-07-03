using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Main_Menu_Animations : MonoBehaviour
{
    [Header("Referencias")]
    [Space(10)]

    public Animator boxAnimator;
    public Animator textAnimator;
    public Animator llavesAnimator;
    public Animator boxTape;
    public GameObject mainMenu;
    public GameObject logoInicial;
    public GameObject logoFinal;
    public GameObject textMenu;
    public InputActionAsset inputActionAsset;
    public Main_Menu_Initial_Button initialButtonScript;

    private InputAction accept;
    private InputActionMap mainMenuMap;

    void Awake()
    {
        //EL mapa ya está habilitado en el Main_Menu_Initiator, solo necesitamos obtener la referencia a las acciones
        mainMenuMap = inputActionAsset.FindActionMap("MainMenu");
        accept = mainMenuMap.FindAction("Accept");
    }

    public void ContinuarAnimacion(float order)
    {
        switch (order)
        {
            case 0:
                //Continuar animacion
                textAnimator.SetTrigger("Aparicion");
                accept.Enable();
                accept.performed += ContinuarCaja;
                break;
            case 1:
                SoundFX_Manager.Instance.PlaySound(SoundType.ABRIR_CAJA_DIALOGO);
                llavesAnimator.SetTrigger("Aparicion");
                StartCoroutine(EsperarYPlay());
                break;
            case 2:
                //Desactivar los objetos iniciales
                logoInicial.SetActive(false);
                boxAnimator.gameObject.SetActive(false);
                textAnimator.gameObject.SetActive(false);

                //Continuar animacion
                Music_Manager.Instance.PlayMusic(MusicType.PORTADA);
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

    private IEnumerator EsperarYPlay()
    {
        yield return new WaitForSeconds(4.5f);
        SoundFX_Manager.Instance.PlaySound(SoundType.SELLECIONADO);
    }

    private void ContinuarCaja(InputAction.CallbackContext context)
    {
        accept.performed -= ContinuarCaja;
        accept.performed += SaltarAnimacion;
        textAnimator.SetTrigger("Desaparicion");
        boxAnimator.SetTrigger("Mover");
    }

    private void SaltarAnimacion(InputAction.CallbackContext context)
    {
        accept.performed -= SaltarAnimacion;
        accept.Disable();

        //Desactivar los objetos iniciales
        logoInicial.SetActive(false);
        boxAnimator.gameObject.SetActive(false);
        textAnimator.gameObject.SetActive(false);
        boxTape.gameObject.SetActive(false);
        llavesAnimator.gameObject.SetActive(false);


        //Musica
        Music_Manager.Instance.PlayMusic(MusicType.PORTADA);

             
        //Activar el menú directamente 
        logoFinal.SetActive(true);
        textMenu.SetActive(true);
        mainMenu.SetActive(true);

         
        logoFinal.GetComponent<Animator>().Play("Idle_Visible", 0, 1f);
        textMenu.GetComponent<Animator>().Play("Visible", 0, 1f);
          
        StartCoroutine(Esperar(0.5f));
    }

    private IEnumerator Esperar(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        initialButtonScript.MostrarPrimerBoton();
        mainMenuMap.FindAction("Move").Enable();
        accept.Enable();
    }

    private IEnumerator EsperarYAparecer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        textMenu.SetActive(true);
        textMenu.GetComponent<Animator>().SetTrigger("Aparicion");
        initialButtonScript.MostrarPrimerBoton();
        mainMenuMap.FindAction("Move").Enable();
        accept.Enable();
    }
}
