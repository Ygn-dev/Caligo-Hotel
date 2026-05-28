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

    public float duracionZoomCamara;
    public float duracionDesaparicion;
    public float duracionImageCharacter;
    public float velocidadEscritura;
    public AnimationCurve curvaZoom;
    public AnimationCurve curvaBlackBackground;
    public AnimationCurve curvaDesaparicion;
    public AnimationCurve curvaDesaparicionCharacter;
    public GameObject prefab_TextBox;
    public GameObject prefab_TextBox2;
    public GameObject prefabDialogueBox;
    public GameObject prefabBlackBackground;
    public InputActionAsset inputActions;
    


    private Canvas canvas;
    private CinemachineCamera cinemachineCamera;
    private GameObject InstanceBlackBackground;
    private GameObject InstanceDialogueBox;
    private string[] arrLines;
    private int indexLine;
    private List<GameObject> listaCajas;
    

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    //METODOS DE INICIO DE DIALOGO
    public void StartDialogue(TextAsset csvFile, float zoomCamara, float camPosX, float camPosY)
    {
        StartCoroutine(IniciarGuion(csvFile, zoomCamara, camPosX, camPosY));
    }

    public void StartDialogue(string csvFileName, float zoomCamara)
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Dialogues/" + csvFileName);
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        float alto = cinemachineCamera.Lens.OrthographicSize*2;
        float ancho = alto * 16/9;

        StartCoroutine(IniciarGuion(csvFile, zoomCamara, character.transform.position.x + ancho/4, character.transform.position.y+2));
    }

    public IEnumerator StartDialogueCoroutine(TextAsset csvFile, float zoomCamara, float camPosX, float camPosY)
    {
        yield return StartCoroutine(IniciarGuion(csvFile, zoomCamara, camPosX, camPosY));
    }

    public IEnumerator StartDialogueCoroutine(string csvFileName, float zoomCamara)
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Dialogues/" + csvFileName);
        GameObject character = GameObject.FindGameObjectWithTag("Player");

        float alto = zoomCamara*2;
        float ancho = alto * 16/9;

        yield return StartCoroutine(IniciarGuion(csvFile, zoomCamara, character.transform.position.x + ancho/4, character.transform.position.y+2));
    }


    // INICIO DE DIALOGO
    private IEnumerator IniciarGuion(TextAsset csvFile, float zoomCamara, float camPosX, float camPosY)
    {
        cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        canvas = FindAnyObjectByType<Canvas>();

        float startZoom = cinemachineCamera.Lens.OrthographicSize;
        Vector3 startPos = cinemachineCamera.transform.position;
        GameObject personaje = cinemachineCamera.Follow.gameObject;

        //Instanciar prefabs necesarios para el dialogo
        InstanceBlackBackground = Instantiate(prefabBlackBackground, canvas.transform);
        InstanceDialogueBox = Instantiate(prefabDialogueBox, canvas.transform);
        
        //Deterner accionesy hacer zoom a la camara
        inputActions.FindActionMap("Gameplay").Disable();
        inputActions.FindActionMap("UI").Enable();
        cinemachineCamera.Follow = null;
        yield return StartCoroutine(DevTools.AnimarCamara(cinemachineCamera, zoomCamara, camPosX, camPosY, duracionZoomCamara, curvaZoom, InstanceBlackBackground, 0, curvaBlackBackground));
        
        //Iniciar guion
        arrLines = csvFile.text.Split(new char[] { '\n' });
        indexLine = 0; //la primera linea del csv es el encabezado
        listaCajas = new List<GameObject>();

        //Avanzar a la primera linea del dialogo
        yield return AvanzarGuion();

        yield return null;
    }


    public IEnumerator AvanzarGuion()
    {
        indexLine++;
        
        //Fin del dialogo
        if (indexLine >= arrLines.Length)
        {
            
            yield break;
        }

        //variables
        string[] arrPartes = arrLines[indexLine].Split(';'); 
        string texto = arrPartes[1];
        string personaje = arrPartes[2];

        GameObject instTextBox = null;

        //elegir prefab segun personaje
        if(personaje.Trim() == "character") instTextBox = Instantiate(prefab_TextBox, InstanceDialogueBox.transform);
        if(personaje.Trim() == "recepcionista") instTextBox = Instantiate(prefab_TextBox2, InstanceDialogueBox.transform);

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
        //Añadir cajas a la lista
        listaCajas.Add(instTextBox);
    }




    
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
    }
}
