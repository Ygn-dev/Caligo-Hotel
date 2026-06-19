using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class Dialogue_Manager : MonoBehaviour
{
    //SINGLETON
    public static Dialogue_Manager Instance { get; private set; }

    public GameObject pruebaPrefab;
    public AnimationCurve curvaZoom;
    public float duracionZoomCamara;
    public float velocidadEscritura;
    public float duracionDesaparicion;
    public InputActionAsset inputActions;
    public GameObject prefabDialogueScroll;
    public GameObject prefabBlackBackground;
    public AnimationCurve curvaBlackBackground;
    /*
    
    public float duracionImageCharacter; 

    public AnimationCurve curvaDesaparicion;
    public AnimationCurve curvaDesaparicionCharacter;
    public GameObject prefab_TextBox;
    public GameObject prefab_TextBox2;
    
    
    


    
    private CinemachineCamera cinemachineCamera;
    private CinemachineBrain cinemachineBrain;
    private GameObject InstanceBlackBackground;
    private GameObject InstanceDialogueScroll;
    
    private List<GameObject> listaCajas;

    */
    private int indexLine;
    private Canvas canvas;
    private TextAsset csvFile;
    private string[] arregloLines;
    private GameObject instanceDialogueScroll;
    

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    //METODOS DE INICIO DE DIALOGO

    // LLAMADA CON POSICION DE CAMARA, PARA CASOS ESPECIALES
    public void StartDialogue(string csvFileName, float zoomCamara, float camPosX, float camPosY)
    {
        StartCoroutine(StartDialogueCoroutine(csvFileName, zoomCamara, camPosX, camPosY));
    }
    
    public IEnumerator StartDialogueCoroutine(string csvFileName, float zoomCamara, float camPosX, float camPosY)
    {
        // Cargar CSV
        csvFile = Resources.Load<TextAsset>("Dialogues/" + csvFileName);
        yield return StartCoroutine(IniciarDialogo(csvFile, zoomCamara, camPosX, camPosY));
    }

    // LLAMADA SIN POSICION DE CAMARA, LA MAS COMUN
    public void StartDialogue(string csvFileName, float zoomCamara)
    {
        StartCoroutine(StartDialogueCoroutine(csvFileName, zoomCamara));
    }

    public IEnumerator StartDialogueCoroutine(string csvFileName, float zoomCamara)
    {
        // Cuando no se tiene posicion de camara, se calcula segun el personaje

        // Cargar CSV
        csvFile = Resources.Load<TextAsset>("Dialogues/" + csvFileName);

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
       
        yield return StartCoroutine(IniciarDialogo(csvFile, zoomCamara, camPosX, camPosY));
        
    }


    // INICIO DE DIALOGO
    private IEnumerator IniciarDialogo(TextAsset csvFile, float zoomCamara, float camPosX, float camPosY)
    {
        // Referencias
        canvas = FindAnyObjectByType<Canvas>();
        CinemachineBrain cinemachineBrain = FindAnyObjectByType<CinemachineBrain>();
        CinemachineCamera cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        CinemachineConfiner2D cinemachineConfiner = cinemachineCamera.GetComponent<CinemachineConfiner2D>();
        Transform cameraTransform = cinemachineBrain.transform;
        
        // Instanciar prefabs necesarios para el dialogo
        GameObject instanceBlackBackground = Instantiate(prefabBlackBackground, canvas.transform);
        instanceDialogueScroll = Instantiate(prefabDialogueScroll, canvas.transform);

        // Deterner acciones
        inputActions.FindActionMap("Gameplay").Disable();
        inputActions.FindActionMap("UI").Enable();

        // Desvincular camara del personaje
        cinemachineCamera.Follow = null;

        // Desactivar el update momentateanamente
        cinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;

        // Desactivar confiner
        cinemachineConfiner.enabled = false;

        // No borrar esta linea rompe todo xd
        cinemachineCamera.transform.position = cameraTransform.position;

        // Reactivar update para que tome la nueva posicion
        cinemachineBrain.UpdateMethod = CinemachineBrain.UpdateMethods.SmartUpdate;
        
        // Zoom a la camara y background negro al mismo tiempo
        yield return StartCoroutine(DevTools.AnimarCamaraYBackground(cinemachineCamera, zoomCamara, camPosX, camPosY, duracionZoomCamara, 
                                                                        curvaZoom, instanceBlackBackground, curvaBlackBackground));
        
        // Iniciar guion
        arregloLines = csvFile.text.Split(new char[] { '\n' });
        indexLine = 0;

        //Avanzar a la primera linea del dialogo
        yield return AvanzarGuion();
    }

    
    public IEnumerator AvanzarGuion()
    {
        indexLine++;
  
        //Fin del dialogo
        if (indexLine >= arregloLines.Length) yield break;

        //variables
        string[] arregloPartes = arregloLines[indexLine].Split(';'); 
        string texto = arregloPartes[1];
        string personaje = arregloPartes[2];


        //calcular padding
        if(indexLine == 1)
        {
            int alturaPrefab = (int)pruebaPrefab.GetComponent<RectTransform>().rect.height;
            prueba_anadir prueba = instanceDialogueScroll.GetComponent<prueba_anadir>();
            prueba.SetPadding(alturaPrefab);
            prueba.Instanciar(pruebaPrefab);

        }
        else
        {
            int alturaPrefab = (int)pruebaPrefab.GetComponent<RectTransform>().rect.height;
            prueba_anadir prueba = instanceDialogueScroll.GetComponent<prueba_anadir>();
            prueba.Instanciar2(pruebaPrefab);
        }

        
        //prueba_anadir prueba = instanceDialogueScroll.GetComponent<prueba_anadir>();
        //prueba.Instanciar(pruebaPrefab);



        /*

        //elegir prefab segun personaje
        //if(personaje.Trim() == "character") instTextBox = Instantiate(prefab_TextBox, InstanceDialogueBox.transform);
        //if(personaje.Trim() == "recepcionista") instTextBox = Instantiate(prefab_TextBox2, InstanceDialogueBox.transform);

        //obtener componentes
        TMP_Text tmpText = instTextBox.GetComponentInChildren<TMP_Text>();
        Image pedazoPapel = instTextBox.transform.GetChild(0).GetComponent<Image>();
        Image characterImage = instTextBox.transform.GetChild(1).GetComponent<Image>();

        //Posicionar Caja
        if(indexLine == 1)
        {
            //Primer dialogo, posicionar en el centro
            instTextBox.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            
        }
           
        //Animar caja de texto
        yield return StartCoroutine(AnimarTexto(tmpText, pedazoPapel, characterImage, texto, duracionImageCharacter, curvaDesaparicionCharacter));
        //Añadir cajas a la lista*/
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            Debug.Log("Se presionó Q");
            StartCoroutine(AvanzarGuion());
        }
    }



    /*
    
    // HELPERS
    private IEnumerator AnimarTexto(TMP_Text tmpText, Image pedazoPapel, Image characterImage, string texto, float duracion, AnimationCurve curvaDesaparicionCharacter)
    {
        //Reproducir animación desde el inicio
        Animator animPapel = pedazoPapel.GetComponent<Animator>();
        animPapel.SetTrigger("PlayIntro");

        //Animar aparición del icono
        StartCoroutine(DevTools.AnimarImage(characterImage,1, duracion, curvaDesaparicionCharacter));

        //MostrarTexto
        int totalCaracteres = texto.Length;
        int caracteresMostrados = 0;
        float delay = 1f / velocidadEscritura;

        while (caracteresMostrados < totalCaracteres)
        {
            // Incrementa según velocidad (caracteres por segundo)
            tmpText.text = texto.Substring(0, caracteresMostrados);
            caracteresMostrados++;

            float timer = 0f;
            while (timer < delay)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
        tmpText.text = texto;

        yield return null;
    }*/
}
