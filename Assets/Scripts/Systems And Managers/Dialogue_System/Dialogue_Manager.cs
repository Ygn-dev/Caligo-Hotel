using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Dialogue_Manager : MonoBehaviour
{
    //==================================================
    // SINGLETON
    //==================================================
    public static Dialogue_Manager Instance { get; private set; }


    //==================================================
    // CÁMARA
    //==================================================
    public float duracionZoomCamara;
    public AnimationCurve curvaZoomCamara;
    public AnimationCurve curvaBlackBackground;


    private float zoomCamIni;
    private Vector3 camPosIni;
    private CinemachineConfiner2D confiner;
    private GameObject instanceBlackBackground;
    private CinemachineCamera cinemachineCamera;


    //==================================================
    // CAJA DE DIÁLOGOS
    //==================================================
    public float duracionAparicionRetrato;
    public float duracionMovimientoScroll;
    public float duracionAparicionLeyenda;
    public float duracionAparicionScrollBar;
    public AnimationCurve curvaAparicionRetrato;
    public float retardoAparicionScrollBar;
    public AnimationCurve curvaAparicionScroll;
    public AnimationCurve curvaMovimientoScroll;
    public AnimationCurve curvaAparicionLeyenda;
    public float velocidadMovimientoAutoScroll;
    public float velocidadMovimientoManualScroll;
    public float velocidadMovimientoAutoRegresarAbajo;
    
    

    private TMP_Text currentText;
    private GameObject instCajaDeDialogo;
    private bool scrollSubiendo;
    private bool scrollBajando;
    private GameObject content;
    private bool seActivoScroll;
    private ScrollRect scrollRect;
    private VerticalLayoutGroup layoutGroup;
    private DialogueScroll_Helper scrollHelper;
    private GameObject instanceDialogueScroll;
    

    //==================================================
    // DIÁLOGO
    //==================================================
    public float velocidadEscritura;


    private int nextNodeId;
    private int currentNodeId;
    private TextAsset jsonFile;
    private Dialogue_Struct dialogueData;    


    //==================================================
    // INPUT
    //==================================================
    private InputAction upAction;
    private InputAction downAction;
    private InputAction acceptAction;


    //==================================================
    // REFERENCIAS
    //==================================================
    public InputActionAsset inputActions;
    public GameObject prefabDialogueScroll;
    public GameObject prefabBlackBackground;


    private Canvas canvas;
    private bool estaTerminando = false;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
        acceptAction = inputActions.FindActionMap("Dialogue").FindAction("Accept");
        upAction = inputActions.FindActionMap("Dialogue").FindAction("Up");
        downAction = inputActions.FindActionMap("Dialogue").FindAction("Down");
    }

    //METODOS DE INICIO DE DIALOGO

    // LLAMADA CON POSICION DE CAMARA, PARA CASOS ESPECIALES
    public void StartDialogue(string jsonFileName, float zoomCamara, float camPosX, float camPosY)
    {
        StartCoroutine(StartDialogueCoroutine(jsonFileName, zoomCamara, camPosX, camPosY));
    }
    
    public IEnumerator StartDialogueCoroutine(string jsonFileName, float zoomCamara, float camPosX, float camPosY)
    {
        yield return StartCoroutine(IniciarDialogo(jsonFileName, zoomCamara, camPosX, camPosY));
    }


    // LLAMADA SIN POSICION DE CAMARA, LA MAS COMUN
    public void StartDialogue(string jsonFileName, float zoomCamara)
    {
        StartCoroutine(StartDialogueCoroutine(jsonFileName, zoomCamara));
    }

    public IEnumerator StartDialogueCoroutine(string jsonFileName, float zoomCamara)
    {
        // Cuando no se tiene posicion de camara, se calcula segun el personaje
        // Determinar posicion de camara segun personaje
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        SpriteRenderer spriteRenderer = character.GetComponentInChildren<SpriteRenderer>();
        Vector3 puntoPersonaje = spriteRenderer.bounds.center;
        float altoMundo = zoomCamara * 2f;
        float anchoMundo = altoMundo * (16f / 9f);
        Vector2 posicionEnPantalla = new Vector2(-480f, 0f);
        Vector2 resolucionReferencia = new Vector2(1920f, 1080f);
        float offsetMundoX = posicionEnPantalla.x / resolucionReferencia.x * anchoMundo;
        float offsetMundoY = posicionEnPantalla.y / resolucionReferencia.y * altoMundo;
        float camPosX = puntoPersonaje.x - offsetMundoX;
        float camPosY = puntoPersonaje.y - offsetMundoY;
       
        yield return StartCoroutine(IniciarDialogo(jsonFileName, zoomCamara, camPosX, camPosY));
        
    }


    // INICIO DE DIALOGO
    private IEnumerator IniciarDialogo(string jsonFileName, float zoomCamara, float camPosX, float camPosY)
    {
        if(estaTerminando) yield break;

        // Cargar Json
        jsonFile = Resources.Load<TextAsset>("Dialogues/" + jsonFileName);
        dialogueData = JsonUtility.FromJson<Dialogue_Struct>(jsonFile.text);

        // Referencias
        CinemachineBrain cinemachineBrain = FindAnyObjectByType<CinemachineBrain>(); 
        Transform cameraTransform = cinemachineBrain.transform;

        // Asignar Variables Globales
        canvas = FindAnyObjectByType<Canvas>();
        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        camPosIni = cameraTransform.transform.position;
        zoomCamIni = cinemachineCamera.Lens.OrthographicSize;
        confiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();

        // Instanciar prefabs
        instanceBlackBackground = Instantiate(prefabBlackBackground, canvas.transform);
        instanceDialogueScroll = Instantiate(prefabDialogueScroll, canvas.transform);

        // Obtener referencias globales del prefab de dialogos
        scrollHelper = instanceDialogueScroll.GetComponent<DialogueScroll_Helper>();
        content = scrollHelper.content;
        layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        scrollRect = scrollHelper.scrollRect;

        // Deterner todas las acciones
        inputActions.FindActionMap("Gameplay").Disable();
        inputActions.FindActionMap("Pause").Disable();

        // Activar el mapa de dialogo
        inputActions.FindActionMap("Dialogue").Enable();
        // Desactivar todas las acciones de dialogo
        acceptAction.Enable();


        // Desvincular camara del personaje
        cinemachineCamera.Follow = null;
        // Desactivar el update momentateanamente
        cinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;
        // Desactivar confiner
        confiner.enabled = false;
        // No borrar esta linea rompe todo xd
        cinemachineCamera.transform.position = cameraTransform.position;
        // Reactivar update para que tome la nueva posicion
        cinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.SmartUpdate;

        
        // Zoom a la camara y background negro al mismo tiempo
        SoundFX_Manager.Instance.PlaySound(SoundType.ABRIR_CAJA_DIALOGO);
        yield return StartCoroutine(DevTools.AnimarCamaraYBackground(cinemachineCamera, zoomCamara, camPosX , camPosY, 
                                                                        duracionZoomCamara, curvaZoomCamara, 
                                                                        instanceBlackBackground, curvaBlackBackground, 260, 0));
        
        // Asignarlo como hijo
        instanceDialogueScroll.transform.SetParent(instanceBlackBackground.transform, true);

        // Iniciar guion
        seActivoScroll = false;
        nextNodeId = dialogueData.startNode;
        yield return StartCoroutine(AvanzarGuion());
    }

    private IEnumerator AvanzarGuion()
    {
        // Manejo de nodos
        if(nextNodeId == -1)
        {
            yield return StartCoroutine(TerminarGuion());
            yield break;
        }
        currentNodeId = nextNodeId;
        nextNodeId = dialogueData.nodes[currentNodeId].nextNodeId;

        // Desactivar acciones de dialogo para que no se pueda avanzar mientras se escribe el texto
        upAction.started -= SubirScroll;
        upAction.canceled -= SubirScroll;
        downAction.started -= BajarScroll;
        downAction.canceled -= BajarScroll;

        // Ocultar ScrollBar
        if(seActivoScroll) scrollHelper.OcultarScrollBar(duracionAparicionScrollBar, curvaAparicionScroll);

        // Si el scroll no esta arriba, bajarlo primero
        yield return StartCoroutine(BajarScrollTodo());
        
        // Settear prefab e instanciar
        yield return StartCoroutine(SetupPrefab());

        if(currentNodeId == 0) // Si es el primer nodo, ajustar el padding para que quede en la parte inferior
        {
            RectTransform rectPrefab = instCajaDeDialogo.GetComponent<RectTransform>();
            int alturaPrefab = Mathf.RoundToInt(rectPrefab.rect.height);
            layoutGroup.padding.top = 750-alturaPrefab;
        }
        else // En todos los demas nodos, mover el scroll hacia abajo para mostrar el nuevo dialogo
        { 
            // Mover el scroll      
            SoundFX_Manager.Instance.PlaySound(SoundType.PASAR_HOJA);
            yield return StartCoroutine(MoverScroll());
            yield return null;

            // Ajustar el padding si ya se alcanza el limite y ahora se necesita mostrar el scroll
            if(!seActivoScroll)
            {
                float alturaReal = content.GetComponent<RectTransform>().rect.height - layoutGroup.padding.top;
                if(alturaReal > 750) 
                {
                    layoutGroup.padding.top = 0;
                    Canvas.ForceUpdateCanvases();
                    LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
                    scrollRect.verticalNormalizedPosition = 0f;
                    seActivoScroll = true;
                }
            }
        }

        // Luego de instanciar el prefab, mostrar la caja de texto y escribir el dialogo
        yield return StartCoroutine(instCajaDeDialogo.GetComponent<ICaja_De_Texto_Helper>().MostrarCaja(duracionAparicionRetrato, curvaAparicionRetrato, acceptAction));
        yield return null;
        
        // Leer el guion, mostrando el texto poco a poco
        yield return StartCoroutine(LeerGuion());
        yield return null;

        if(dialogueData.dialogueId == "dialogo_recepcionista" || dialogueData.dialogueId == "nivel3_llave") 
        {
            if(currentNodeId == 3 || currentNodeId == 12) 
            {
                SoundFX_Manager.Instance.PlaySound(SoundType.COGER_LLAVE);
            }
            if(dialogueData.dialogueId == "nivel3_llave" && currentNodeId == 3) 
            {
                Save_Manager.Instance.data.tieneLlaveN2 = true;
                Save_Manager.Instance.SaveData();
            }
        }

        // Letra de desplazar
        if(nextNodeId == -1) instanceDialogueScroll.GetComponent<DialogueScroll_Helper>().leyendaText.text = "Fin";
        else instanceDialogueScroll.GetComponent<DialogueScroll_Helper>().leyendaText.text = "Siguiente";

        // Si el scroll ya es lo suficientemente largo, mostrar el scrollbar
        if(seActivoScroll) scrollHelper.MostrarScrollBar(duracionAparicionScrollBar, curvaAparicionScroll, retardoAparicionScrollBar);

        // Recien ahora se puede avanzar al siguiente nodo o subir/bajar el scroll
        acceptAction.performed += AvanzarAccion;
        upAction.started += SubirScroll;
        upAction.canceled += SubirScroll;
        downAction.started += BajarScroll;
        downAction.canceled += BajarScroll;


        // Si es el primer nodo, mostrar la leyenda despues de un tiempo
        if(currentNodeId == 0)
        {
            CanvasGroup canvasGroup = instanceDialogueScroll.GetComponent<DialogueScroll_Helper>().leyenda.GetComponent<CanvasGroup>();
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(DevTools.AnimarCanvasGroup(canvasGroup, 1, duracionAparicionLeyenda, curvaAparicionLeyenda));
        }

        yield return null;  
    }

    private void AvanzarAccion(InputAction.CallbackContext context)
    {
        acceptAction.performed -= AvanzarAccion;
        StartCoroutine(AvanzarGuion());
    }

    private IEnumerator MoverScroll()
    {
        float tiempo = 0;
        float posicionInicial = scrollRect.verticalNormalizedPosition;
        float posicionFinal = 0f;

        while (tiempo < duracionMovimientoScroll)
        {
            // Skip
            if (acceptAction.triggered)
            {
                scrollRect.verticalNormalizedPosition = posicionFinal;
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
                yield break;
            }

            float t = tiempo / duracionMovimientoScroll;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(posicionInicial, posicionFinal, curvaMovimientoScroll.Evaluate(t));
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            tiempo += Time.deltaTime;
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        yield return null;
    }

    private IEnumerator LeerGuion()
    {
        TMP_Text text = instCajaDeDialogo.GetComponent<ICaja_De_Texto_Helper>().GetTextoComponent();
        TMP_TextInfo textInfo = text.textInfo;

        int currentVisibleCharacterIndex = 0;
        float delay = (1/velocidadEscritura);

        while (currentVisibleCharacterIndex < textInfo.characterCount)
        {
            // Si se presiona aceptar, mostrar todo el texto inmediatamente
            if (acceptAction.triggered)
            {
                text.maxVisibleCharacters = textInfo.characterCount;
                yield break;
            }

            currentVisibleCharacterIndex++;
            text.maxVisibleCharacters = currentVisibleCharacterIndex;


            int resto = 3;
            if(velocidadEscritura > 15f) {
                resto = 3;
            }else{
                resto = Random.value < 0.75f ? 2 : 3;
            }

            if (currentVisibleCharacterIndex % resto == 0)
            {
                SoundFX_Manager.Instance.PlaySound(SoundType.TYPEWRITER);
            }

            // Esperar el delay, pero revisando el input cada frame
            float timer = 0f;
            while (timer < delay)
            {
                if (acceptAction.triggered)
                {
                    text.maxVisibleCharacters = textInfo.characterCount;
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator SetupPrefab()
    {
        //Datos del nodo
        string personaje = dialogueData.nodes[currentNodeId].personaje;
        string texto = dialogueData.nodes[currentNodeId].text;

        //TO DO
        //CALCULAR SPACIADO

        // Cargar prefab segun personaje
        string prefabPath = "Prefabs/Dialogue_System/Caja_" + personaje + "_Prefab";
        instCajaDeDialogo = Instantiate(Resources.Load<GameObject>(prefabPath), content.transform);

        // Setear texto
        ICaja_De_Texto_Helper cajaHelper = instCajaDeDialogo.GetComponent<ICaja_De_Texto_Helper>();
        currentText = cajaHelper.GetTextoComponent();
        currentText.maxVisibleCharacters = 0;
        cajaHelper.SetTexto(texto);
        cajaHelper.ActualizarLayouts();
        yield return null;
    }

    private IEnumerator TerminarGuion()
    {
        // 
        estaTerminando = true;

        // Reanudar acciones
        inputActions.FindActionMap("Dialogue").Disable();

        inputActions.FindActionMap("Gameplay").Enable();
        inputActions.FindActionMap("Pause").Enable();

        SoundFX_Manager.Instance.PlaySound(SoundType.ABRIR_CAJA_DIALOGO);

        // Ocultar el dialogue system 
        yield return StartCoroutine(DevTools.AnimarCamaraYBackground(cinemachineCamera, zoomCamIni, camPosIni.x, camPosIni.y, 
                                                                        duracionZoomCamara, curvaZoomCamara, instanceBlackBackground, 
                                                                        curvaBlackBackground, 1660, 0));
        // Eliminarlo
        Destroy(instanceBlackBackground);
        instanceBlackBackground = null;
        
        // Desactivar confiner
        confiner.enabled = enabled;
        // vincular camara al personaje
        cinemachineCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;

        // 
        estaTerminando = false;
        yield return null;
    }

    private IEnumerator BajarScrollTodo()
    {
        if(!seActivoScroll) yield break;
        if(scrollRect.verticalNormalizedPosition < 0.01f) yield break;

        Debug.Log("Bajando scroll");

        while (scrollRect.verticalNormalizedPosition > 0f)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Max(0f, scrollRect.verticalNormalizedPosition - velocidadMovimientoAutoScroll * Time.deltaTime);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
    }

    private void SubirScroll(InputAction.CallbackContext context)
    {
        scrollSubiendo = context.ReadValueAsButton();
    }

    private void BajarScroll(InputAction.CallbackContext context)
    {
        scrollBajando = context.ReadValueAsButton();
    }
    
    private void Update()
    {
        if (scrollSubiendo && seActivoScroll)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + velocidadMovimientoManualScroll * Time.deltaTime);
        }

        if (scrollBajando && seActivoScroll)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - velocidadMovimientoManualScroll * Time.deltaTime);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Dialogue_Manager))] // Cambia TuScript por el nombre de tu clase
public class Dialogue_Manager_Editor : Editor
{
    //==================================================
    // CÁMARA
    //==================================================
    SerializedProperty duracionZoomCamara;
    SerializedProperty curvaZoomCamara;
    SerializedProperty curvaBlackBackground;

    //==================================================
    // CAJA DE DIÁLOGOS
    //==================================================
    SerializedProperty duracionAparicionRetrato;
    SerializedProperty duracionAparicionLeyenda;
    SerializedProperty duracionMovimientoScroll;
    SerializedProperty duracionAparicionScrollBar;
    SerializedProperty curvaAparicionRetrato;
    SerializedProperty retardoAparicionScrollBar;
    SerializedProperty curvaAparicionScroll;
    SerializedProperty curvaMovimientoScroll;
    SerializedProperty curvaAparicionLeyenda;
    SerializedProperty velocidadMovimientoManualScroll;
    SerializedProperty velocidadMovimientoAutoScroll;
    SerializedProperty velocidadMovimientoAutoRegresarAbajo;

    //==================================================
    // DIÁLOGO
    //==================================================
    SerializedProperty velocidadEscritura;

    //==================================================
    // REFERENCIAS
    //==================================================
    SerializedProperty inputActions;
    SerializedProperty prefabDialogueScroll;
    SerializedProperty prefabBlackBackground;

    bool mostrarCamara = true;
    bool mostrarCaja = true;
    bool mostrarDialogo = true;
    bool mostrarReferencias = true;

    void OnEnable()
    {
        // Cámara
        duracionZoomCamara = serializedObject.FindProperty("duracionZoomCamara");
        curvaZoomCamara = serializedObject.FindProperty("curvaZoomCamara");
        curvaBlackBackground = serializedObject.FindProperty("curvaBlackBackground");

        // Caja de diálogo
        duracionAparicionRetrato = serializedObject.FindProperty("duracionAparicionRetrato");
        duracionAparicionLeyenda = serializedObject.FindProperty("duracionAparicionLeyenda");
        duracionMovimientoScroll = serializedObject.FindProperty("duracionMovimientoScroll");
        velocidadMovimientoManualScroll = serializedObject.FindProperty("velocidadMovimientoManualScroll");
        velocidadMovimientoAutoScroll = serializedObject.FindProperty("velocidadMovimientoAutoScroll");
        velocidadMovimientoAutoRegresarAbajo = serializedObject.FindProperty("velocidadMovimientoAutoRegresarAbajo");

        duracionAparicionScrollBar = serializedObject.FindProperty("duracionAparicionScrollBar");
        curvaAparicionRetrato = serializedObject.FindProperty("curvaAparicionRetrato");
        retardoAparicionScrollBar = serializedObject.FindProperty("retardoAparicionScrollBar");
        curvaAparicionScroll = serializedObject.FindProperty("curvaAparicionScroll");
        curvaMovimientoScroll = serializedObject.FindProperty("curvaMovimientoScroll");
        curvaAparicionLeyenda = serializedObject.FindProperty("curvaAparicionLeyenda");

        // Diálogo
        velocidadEscritura = serializedObject.FindProperty("velocidadEscritura");

        // Referencias
        inputActions = serializedObject.FindProperty("inputActions");
        prefabDialogueScroll = serializedObject.FindProperty("prefabDialogueScroll");
        prefabBlackBackground = serializedObject.FindProperty("prefabBlackBackground");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //==================================================
        // CÁMARA
        //==================================================
        mostrarCamara = EditorGUILayout.BeginFoldoutHeaderGroup(mostrarCamara, "Cámara");
        if (mostrarCamara)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(duracionZoomCamara);
            EditorGUILayout.PropertyField(curvaZoomCamara);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(curvaBlackBackground);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        //==================================================
        // CAJA DE DIÁLOGOS
        //==================================================
        mostrarCaja = EditorGUILayout.BeginFoldoutHeaderGroup(mostrarCaja, "Caja de Diálogos");
        if (mostrarCaja)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(duracionAparicionRetrato);
            EditorGUILayout.PropertyField(curvaAparicionRetrato);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(velocidadMovimientoManualScroll);
            EditorGUILayout.PropertyField(velocidadMovimientoAutoScroll);
            EditorGUILayout.PropertyField(velocidadMovimientoAutoRegresarAbajo);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(duracionMovimientoScroll);
            EditorGUILayout.PropertyField(curvaMovimientoScroll);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(duracionAparicionScrollBar);
            EditorGUILayout.PropertyField(retardoAparicionScrollBar);
            EditorGUILayout.PropertyField(curvaAparicionScroll);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(duracionAparicionLeyenda);
            EditorGUILayout.PropertyField(curvaAparicionLeyenda);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        //==================================================
        // DIÁLOGO
        //==================================================
        mostrarDialogo = EditorGUILayout.BeginFoldoutHeaderGroup(mostrarDialogo, "Diálogo");
        if (mostrarDialogo)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(velocidadEscritura);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        //==================================================
        // REFERENCIAS
        //==================================================
        mostrarReferencias = EditorGUILayout.BeginFoldoutHeaderGroup(mostrarReferencias, "Referencias");
        if (mostrarReferencias)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(inputActions);
            EditorGUILayout.PropertyField(prefabDialogueScroll);
            EditorGUILayout.PropertyField(prefabBlackBackground);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif