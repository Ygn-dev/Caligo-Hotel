using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Main_Menu_Initiator : MonoBehaviour
{
    public GameObject logo;
    public GameObject keyBox;
    public Image logoInicial;
    public GameObject menuBox;
    public GameObject boxTape;
    public GameObject textBox;
    public GameObject textMenu;
    public float duracionEspera;
    public Animator boxAnimator;
    public float duracionAparicionLogo;
    public float duracionDesaparicionLogo;
    public AnimationCurve curvaAparicionLogo;
    public InputActionAsset inputActionAsset;


    private InputActionMap mainMenuMap;
    private InputAction accept;

    void Start()
    {
        StartCoroutine(InicioDeMenu());
    }

    private IEnumerator InicioDeMenu()
    {
        yield return StartCoroutine(SetupInicial());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(MostrarImagen(logoInicial, duracionAparicionLogo, duracionEspera, duracionDesaparicionLogo));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MostrarCaja());
        //Lo demas sigue en Main_Menu_Animations
    }

    private IEnumerator SetupInicial()
    {
        // Deshabilitar todos los mapas de acción y acciones para evitar conflictos
        foreach (var map in inputActionAsset.actionMaps) map.Disable();
        

        // Habilitar el mapa de acción específico para el menú principal, pero mantener las acciones deshabilitadas
        mainMenuMap = inputActionAsset.FindActionMap("MainMenu");
        mainMenuMap.Enable();
        foreach (var action in mainMenuMap.actions) action.Disable();

        // Obtener referencia a la acción "Accept" para habilitarla más tarde
        accept = mainMenuMap.FindAction("Accept");

        // Configuración inicial de la interfaz del menú
        logo.SetActive(false);
        menuBox.SetActive(false);
        boxTape.SetActive(false);
        textMenu.SetActive(false);
        logoInicial.gameObject.SetActive(true);
        boxAnimator.gameObject.SetActive(true);
        textBox.SetActive(true);
        keyBox.SetActive(true);

        Time.timeScale = 1f;
        yield return null;
    }

    private IEnumerator MostrarImagen(Image image, float duracionAparicion, float duracionEspera, float duracionDesaparicion)
    {
        bool saltar = false;
        accept.Enable();

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        float targetAlpha = 1f;
        yield return DevTools.AnimarImageConInterrupcion(image,targetAlpha,duracionAparicion,curvaAparicionLogo,
        () =>  {
            if (accept.WasPressedThisFrame())
            {
                saltar = true;
                return true;
            }
            return false;
        });

        targetAlpha = 0f;
        if (!saltar) yield return new WaitForSeconds(duracionEspera);
        accept.Disable();
        yield return DevTools.AnimarImage(image,targetAlpha,duracionDesaparicion,curvaAparicionLogo);
    }

    private IEnumerator MostrarCaja()
    {
        Music_Manager.Instance.PlayMusic(MusicType.PREPORTADA);
        mainMenuMap.FindAction("Accept").Enable();

        boxAnimator.SetTrigger("Aparecer");
        StartCoroutine(EsperarYSonido(0.5f));
        yield return null;

        while (true)
        {
            // Si precionas Accept se salta la animacion
            if(mainMenuMap.FindAction("Accept").WasPressedThisFrame())
            {
                boxAnimator.Play("Aparicion", 0, 1f);
                break;
            }

            // Romper el break cuando la animacion termine
            if (boxAnimator.GetCurrentAnimatorStateInfo(0).IsName("Aparicion") && boxAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                break;
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator EsperarYSonido(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SoundFX_Manager.Instance.PlaySound(SoundType.APARECE_CAJA);
    }
    
}


#if UNITY_EDITOR
[CustomEditor(typeof(Main_Menu_Initiator))]
public class Main_Menu_InitiatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle tituloStyle = new GUIStyle(EditorStyles.boldLabel);
        tituloStyle.fontSize = 14;

        // ============================
        // PARÁMETROS EDITABLES
        // ============================
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Parametros Ajustables", tituloStyle);
        EditorGUILayout.Space(4);

        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("duracionAparicionLogo"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duracionDesaparicionLogo"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duracionEspera"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("curvaAparicionLogo"));

        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // ============================
        // REFERENCIAS NO EDITABLES
        // ============================
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Referencias", tituloStyle);
        EditorGUILayout.Space(4);

        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("logo"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("keyBox"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("logoInicial"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("menuBox"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("boxTape"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textBox"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("textMenu"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("boxAnimator"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inputActionAsset"));

        EditorGUI.indentLevel--;

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
