using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MenuPausaSystem : MonoBehaviour
{
    public InputActionAsset InputActions;
    public GameObject MenuPausa;
    public float duracionAparicion;
    public AnimationCurve curvaAparicion;
    public AnimationCurve curvaDesenfoque;
    public AnimationCurve curvaEnfoque;
    public GameObject blackScreen;
    public CanvasGroup campana;
    public AnimationCurve curvaBlackScreen;
    public VolumeProfile volumeProfile;


    private InputActionMap ActionMapUI;
    private InputActionMap ActionMapGameplay;
    private DepthOfField dof;


    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main_Menu")
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
        //Bindear Input Actions
        ActionMapUI = InputActions.FindActionMap("Pause");
        ActionMapGameplay = InputActions.FindActionMap("Gameplay");
        //Asignar eventos
        InputAction pausaAction = ActionMapGameplay.FindAction("Pause");
        InputAction DespauseAction = ActionMapUI.FindAction("Despause");

        pausaAction.performed += OnPause;
        DespauseAction.performed += OnResume;

        if (volumeProfile != null)
        {
            if (volumeProfile.TryGet(out DepthOfField depthOfField))
            {
                dof = depthOfField;
            }
            else
            {
                Debug.LogWarning("Depth of Field no está presente en el Volume Profile asignado.");
            }
        }
        else
        {
            Debug.LogWarning("VolumeProfile no está asignado en el Inspector. Se desactivará el efecto de desenfoque.");
        }
    }

    //Destructores:
    void OnDisable()
    {
        ActionMapGameplay?.Disable();
        ActionMapUI?.Disable();
    }
    void OnDestroy()
    {
        // Evita llamadas cuando el objeto ya no existe
        if (ActionMapGameplay != null)
        {
            var pausaAction = ActionMapGameplay.FindAction("Pause");
            pausaAction.performed -= OnPause;
        }

        if (ActionMapUI != null)
        {
            var despauseAction = ActionMapUI.FindAction("Despause");
            despauseAction.performed -= OnResume;
        }
    }

    void OnPause(InputAction.CallbackContext context)
    {
        StartCoroutine(Pausa());
    }

    void OnResume(InputAction.CallbackContext context)
    {
        StartCoroutine(Reanudar());
    }

    public void ReanudarJuego()
    {
        StartCoroutine(Reanudar());
    }

    private IEnumerator Pausa()
    {
        //SoundFX_Manager.Instance.PlayRandomPitch(SoundType.TIMBRE, 0.2f);
        Time.timeScale = 0;
        ActionMapGameplay.Disable();
        MenuPausa.transform.SetAsLastSibling();
        yield return StartCoroutine(Animar(new Vector3(0f, 0f, 0f), 140f, 225f, 0f, duracionAparicion));
        ActionMapUI.Enable();
        yield return null;
    }

    private IEnumerator Reanudar()
    {
        ActionMapUI.Disable();
        yield return StartCoroutine(Animar(new Vector3(0f, 1500f, 0f), 10f, 0f, 1f, duracionAparicion));
        ActionMapGameplay.Enable();
        Time.timeScale = 1;
        yield return null;
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1f;
        StartCoroutine(IrAlMenuPrincipal());
    }
    private IEnumerator IrAlMenuPrincipal()
    {
        yield return StartCoroutine(Animar(new Vector3(0f, 1500f, 0f), 10f, 0f, 1f, duracionAparicion));
        SceneManager.LoadScene("Main_Menu");
        Destroy(this.gameObject);
    }

    private IEnumerator Animar(Vector3 posicionFinal, float valorFinalDof, float valorFinalAlpha, float valorFinalCampana, float duracion)
    {
        //valores iniciales
        Vector3 posPrev = MenuPausa.GetComponent<RectTransform>().localPosition;
        float dofPrev = dof.focalLength.value;
        byte colorPrev = ((Color32)blackScreen.GetComponent<Image>().color).a;
        float campanaPrev = campana.alpha;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracion)
        {
            //Usar unscaled delta time porque el tiempo está pausado
            tiempoTranscurrido += Time.unscaledDeltaTime;

            //evaluar curvas
            float curvaDofT;
            float t = Mathf.Clamp01(tiempoTranscurrido / duracion);
            float curvaAparicionT = curvaAparicion.Evaluate(t);
            float curvaBlackScreenT = curvaBlackScreen.Evaluate(t);
            if (dofPrev == 10) curvaDofT = curvaDesenfoque.Evaluate(t);
            else curvaDofT = curvaEnfoque.Evaluate(t);

            //Aplicar valores interpolados
            dof.focalLength.value = Mathf.LerpUnclamped(dofPrev, valorFinalDof, curvaDofT);
            MenuPausa.GetComponent<RectTransform>().localPosition = Vector3.LerpUnclamped(posPrev, posicionFinal, curvaAparicionT);
            Color32 colorActual = blackScreen.GetComponent<Image>().color;
            colorActual.a = (byte)Mathf.LerpUnclamped(colorPrev, valorFinalAlpha, curvaBlackScreenT);
            blackScreen.GetComponent<Image>().color = colorActual;
            campana.alpha = Mathf.LerpUnclamped(campanaPrev, valorFinalCampana, curvaBlackScreenT);


            yield return null;
        }
        //Asegurarse de que los valores finales se apliquen
        MenuPausa.GetComponent<RectTransform>().localPosition = posicionFinal;
        dof.focalLength.value = valorFinalDof;
        Color32 finalColor = blackScreen.GetComponent<Image>().color;
        finalColor.a = (byte)valorFinalAlpha;
        blackScreen.GetComponent<Image>().color = finalColor;
        campana.alpha = valorFinalCampana;

        yield return null;
    }
}