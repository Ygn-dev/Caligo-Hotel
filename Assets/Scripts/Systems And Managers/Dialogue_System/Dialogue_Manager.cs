using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;



public class Dialogue_Manager : MonoBehaviour
{
    //SINGLETON
    public static Dialogue_Manager Instance { get; private set; }

    public AnimationCurve curvaZoom;
    public float duracionZoomCamara;
    public float velocidadEscritura;
    public float duracionDesaparicion;
    public InputActionAsset inputActions;
    public GameObject prefabDialogueScroll;
    public GameObject prefabBlackBackground;
    public AnimationCurve curvaBlackBackground;
    public float duracionAparicionCaja;
    public AnimationCurve curvaAparicionCaja;
    public AnimationCurve curvaMovimientoScroll;
    public float duracionMovimientoScroll;
    public AnimationCurve curvaAparicionLeyenda;
    public float duracionAparicionLeyenda;
    public float velocidadScroll;
    public float duracionAparicionScroll;
    public AnimationCurve curvaAparicionScroll;

    
    private Canvas canvas;  
    private int nextNodeId;
    private float zoomCamIni;
    private bool subirScroll;
    private bool bajarScroll;
    private Vector3 camPosIni;
    private int currentNodeId;
    private TextAsset jsonFile; 
    private GameObject content;
    private bool seActivoScroll;
    private TMP_Text currentText;
    private InputAction upAction;
    private ScrollRect scrollRect;
    private InputAction downAction;
    private bool isQuitandoPadding;
    private InputAction acceptAction;
    private GameObject instCajaDeDialogo;
    private Dialogue_Struct dialogueData;
    private CinemachineConfiner2D confiner;
    private VerticalLayoutGroup layoutGroup;
    private GameObject instanceDialogueScroll;
    private DialogueScroll_Helper scrollHelper;
    private GameObject instanceBlackBackground;
    private CinemachineCamera cinemachineCamera;  


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

        // Obtener referencias Globales del prefab de dialogos
        scrollHelper = instanceDialogueScroll.GetComponent<DialogueScroll_Helper>();
        content = scrollHelper.content;
        layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        scrollRect = scrollHelper.scrollRect;

        // Deterner todas las acciones
        inputActions.FindActionMap("Gameplay").Disable();
        inputActions.FindActionMap("Pause").Disable();

        //activar el mapa de dialogo
        inputActions.FindActionMap("Dialogue").Enable();
        //desactivar todas las acciones de dialogo
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
        yield return StartCoroutine(DevTools.AnimarCamaraYBackground(cinemachineCamera, zoomCamara, camPosX , camPosY, 
                                                                        duracionZoomCamara, curvaZoom, 
                                                                        instanceBlackBackground, curvaBlackBackground, 260, 0));
        
        // Asignarlo como hijo
        instanceDialogueScroll.transform.SetParent(instanceBlackBackground.transform, true);

        // Iniciar guion
        seActivoScroll = false;
        isQuitandoPadding = false;
        nextNodeId = dialogueData.startNode;
        yield return StartCoroutine(scrollHelper.OcultarScrollBar(duracionAparicionScroll, curvaAparicionScroll));
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
        yield return StartCoroutine(scrollHelper.OcultarScrollBar(duracionAparicionScroll, curvaAparicionScroll));

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
            yield return StartCoroutine(MoverScroll());
            yield return null;

            // Ajustar el padding si ya se alcanza el limite y ahora se necesita mostrar el scroll
            if(!seActivoScroll)
            {
                float alturaReal = content.GetComponent<RectTransform>().rect.height - layoutGroup.padding.top;
                if(alturaReal > 750) StartCoroutine(QuitarPadding());
            }
        }

        // Luego de instanciar el prefab, mostrar la caja de texto y escribir el dialogo
        yield return StartCoroutine(instCajaDeDialogo.GetComponent<ICaja_De_Texto_Helper>().MostrarCaja(duracionAparicionCaja, curvaAparicionCaja, acceptAction));
        yield return null;
        
        // Leer el guion, mostrando el texto poco a poco
        yield return StartCoroutine(LeerGuion());
        yield return null;

        // Si el scroll ya es lo suficientemente largo, mostrar el scrollbar
        if(seActivoScroll) scrollHelper.MostrarScroll(duracionAparicionScroll, curvaAparicionScroll);

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
        // Si se esta quitando el padding, esperar a que termine antes de mover el scroll
        if (isQuitandoPadding)
        {
            while(isQuitandoPadding)
            {
                yield return null;
            }
            yield return null;
        }


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

    private IEnumerator QuitarPadding()
    {
        isQuitandoPadding = true;
        while (layoutGroup.padding.top > 0)
        {
            layoutGroup.padding.top --; 
            scrollRect.verticalNormalizedPosition = 0f; // Mantener el scroll en la parte inferior durante la animación
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            yield return null;
        }

        isQuitandoPadding = false;
        seActivoScroll = true;
        scrollHelper.MostrarScroll(duracionAparicionScroll, curvaAparicionScroll);
        yield return null;
    }

    private IEnumerator TerminarGuion()
    {
        // Reanudar acciones
        inputActions.FindActionMap("Dialogue").Disable();

        // Ocultar el dialogue system 
        yield return StartCoroutine(DevTools.AnimarCamaraYBackground(cinemachineCamera, zoomCamIni, camPosIni.x, camPosIni.y, 
                                                                        duracionZoomCamara, curvaZoom, instanceBlackBackground, 
                                                                        curvaBlackBackground, 1660, 0));
        // Eliminarlo
        Destroy(instanceBlackBackground);
        instanceBlackBackground = null;
        
        // Desactivar confiner
        confiner.enabled = enabled;
        // vincular camara al personaje
        cinemachineCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;;

        inputActions.FindActionMap("Gameplay").Enable();
        inputActions.FindActionMap("Pause").Enable();

        yield return null;
    }

    private IEnumerator BajarScrollTodo()
    {
        if(!seActivoScroll) yield break;
        if(scrollRect.verticalNormalizedPosition < 0.01f) yield break;

        while (scrollRect.verticalNormalizedPosition > 0f)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Max(0f, scrollRect.verticalNormalizedPosition - velocidadScroll * Time.deltaTime);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
    }

    private void SubirScroll(InputAction.CallbackContext context)
    {
        subirScroll = context.ReadValueAsButton();
    }

    private void BajarScroll(InputAction.CallbackContext context)
    {
        bajarScroll = context.ReadValueAsButton();
    }
    
    private void Update()
    {
        if (subirScroll && seActivoScroll)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + velocidadScroll * Time.deltaTime);
        }

        if (bajarScroll && seActivoScroll)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - velocidadScroll * Time.deltaTime);
        }
    }
}
