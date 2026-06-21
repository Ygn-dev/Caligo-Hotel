using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;


public class Dialogue_Manager : MonoBehaviour
{
    //SINGLETON
    public static Dialogue_Manager Instance { get; private set; }

    public GameObject prefabMonologo;
    public GameObject prefabPersonaje;
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

    
    private Canvas canvas;
    private int nextNodeId;
    private float zoomCamIni;
    private int currentNodeId;
    private Vector3 camPosIni;
    private TextAsset jsonFile;
    private GameObject content;
    private ScrollRect scrollRect;
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

        // Deterner acciones
        inputActions.FindActionMap("Gameplay").Disable();
        inputActions.FindActionMap("UI").Enable();


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
        nextNodeId = dialogueData.startNode;
        scrollHelper.OcultarScroll();
        yield return AvanzarGuion();
    }

    
    private IEnumerator AvanzarGuion()
    {
        //Manejo de nodos
        if(nextNodeId == -1)
        {
            yield return StartCoroutine(TerminarGuion());
            yield break;
        }

        currentNodeId = nextNodeId;
        nextNodeId = dialogueData.nodes[currentNodeId].nextNodeId;

        
        //Settear prefab e instanciar
        yield return StartCoroutine(SetupPrefab());
        

        if(currentNodeId == 0)
        {
            //TO DO
            // MOSTRAR SIMULTANEO LA LEYENDA
            RectTransform rectPrefab = instCajaDeDialogo.GetComponent<RectTransform>();
            int alturaPrefab = Mathf.RoundToInt(rectPrefab.rect.height);
            layoutGroup.padding.top = 750-alturaPrefab;
        }
        else
        {
            yield return StartCoroutine(MoverScroll());


            float alturaReal = content.GetComponent<RectTransform>().rect.height - layoutGroup.padding.top;
            if(alturaReal > 750)
            {
                yield return StartCoroutine(QuitarPadding());
                scrollHelper.MostrarScroll();
            }
            
        }
        yield return instCajaDeDialogo.GetComponent<ICaja_De_Texto_Helper>().MostrarCaja(duracionAparicionCaja, curvaAparicionCaja);
        
    }

    private IEnumerator SetupPrefab()
    {
        //Datos del nodo
        string personaje = dialogueData.nodes[currentNodeId].personaje;
        string texto = dialogueData.nodes[currentNodeId].text;

        //TO DO
        //CALCULAR SPACIADO

        switch (personaje)
        {
            case "monologo":
                //TO DO
                //IF EL ANTERIOR TAMBIEN FUE MONOLOGO, MODIFICAR PREFAB
                instCajaDeDialogo = Instantiate(prefabMonologo,content.transform);
                break;
            case "character":
                instCajaDeDialogo = Instantiate(prefabPersonaje,content.transform);
                break;
                /*
            case:
                "recepcionista":
                instCajaDeDialogo = Instantiate(prefabRecepcionista,content.transform);
                */
            default:
                Debug.LogError("Personaje desconocido: " + personaje);
                break;
        }

        ICaja_De_Texto_Helper cajaHelper = instCajaDeDialogo.GetComponent<ICaja_De_Texto_Helper>();
        cajaHelper.SetTexto(texto);
        cajaHelper.ActualizarLayouts();
        yield return null;
    }

    private IEnumerator QuitarPadding()
    {
        while (layoutGroup.padding.top > 0)
        {
            layoutGroup.padding.top --; 
            scrollRect.verticalNormalizedPosition = 0f; // Mantener el scroll en la parte inferior durante la animación
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
            yield return null;
        }
    }

    private IEnumerator TerminarGuion()
    {
        // Ocultar el dialogue system 
        yield return StartCoroutine(DevTools.AnimarCamaraYBackground(cinemachineCamera, zoomCamIni, camPosIni.x, camPosIni.y, 
                                                                        duracionZoomCamara, curvaZoom, instanceBlackBackground, 
                                                                        curvaBlackBackground, 1660, 0));
        // Eliminarlo
        Destroy(instanceBlackBackground);
        
        // Desactivar confiner
        confiner.enabled = enabled;
        // vincular camara al personaje
        cinemachineCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;;

        // Reanudar acciones
        inputActions.FindActionMap("Gameplay").Enable();
        inputActions.FindActionMap("UI").Disable();

        yield return null;
    }

    private IEnumerator MoverScroll()
    {
        float tiempo = 0;
        float posicionInicial = scrollRect.verticalNormalizedPosition;
        float posicionFinal = 0f;

        while (tiempo < duracionMovimientoScroll)
        {
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

    /*
    private IEnumerator prueba2()
    {
        float tiempoAnimacion = 2f;
        float tiempoTranscurrido = 0f;

        float posicionInicial = scrollRect.verticalNormalizedPosition;
        float posicionFinal = 0f;

        while (tiempoTranscurrido < tiempoAnimacion)
        {
            float t = tiempoTranscurrido / tiempoAnimacion;

            scrollRect.verticalNormalizedPosition = Mathf.Lerp(posicionInicial, posicionFinal, t);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        yield return null;
    }

    */

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            StartCoroutine(AvanzarGuion());
        }
    }
}
