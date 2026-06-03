using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;




public class Lobby_Initiator : MonoBehaviour
{
    [Header("La parte editable está en su Scriptable Object")]
    
    [Space]
    [Header("No Editable")]
    public Image fade;
    private GameObject character;
    public AnimationCurve fadeCurve;
    public ScriptableObject levelData;
    public GameObject characterPrefab;
    public InputActionAsset inputActions;
    public CinemachineCamera cinemachineCamera;

    void Awake()
    {
        inputActions.Disable();

        // Este nivel usara cinimatica, asi que se asegura de que el Cinematic Manager exista en la escena
        if (Cinematic_Manager.Instance == null) DevTools.SetupCinematicManager();
        // Este nivel usara sistema de dialogos, asi que se asegura de que el Dialogue Manager exista en la escena
        if (Dialogue_Manager.Instance == null) DevTools.SetupDialogueManager();
        return;
    }
    
    void Start()
    {
        StartCoroutine(InitializeLobby());
    }

    private IEnumerator InitializeLobby()
    {
        
        // Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());
        //yield return StartCoroutine(FadeBlanco());

        //Settear Personaje y Camara mientras se reproduce la cinemática
    
        StartCoroutine(SpawnCharacter());
        StartCoroutine(SetupCamara());
        //StartCoroutine(EsperarYquitarFade(3f));
        //yield return StartCoroutine(CinematicaInicial());
        

        

        //Levantarse del sillon
        yield return new WaitForSeconds(3f);

        //Animacion de levantarse
        character.GetComponent<Animator>().SetTrigger("WakeUp");
        while (!character.GetComponent<Animator>().GetBool("isAwake")) yield return null;
        
        //Dialogo de introduccion
        yield return new WaitForSeconds(1f);
        yield return Dialogue_Manager.Instance.StartDialogueCoroutine("Lobby_Introduccion", 6f);
        
        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();
        yield return null;
    }

    private IEnumerator SetupCamara()
    {
        yield return StartCoroutine(DevTools.SetupCamara(cinemachineCamera, levelData, character));
        yield return null;
    }

    private IEnumerator CinematicaInicial()
    {
        yield return Cinematic_Manager.Instance.PlayCinematic("Lobby_Cinematic");
        yield return null;
    }

    private IEnumerator SpawnCharacter()
    {
        character = Instantiate(characterPrefab,new Vector3(7.36f, 1.65f, 0f), Quaternion.identity);
        character.GetComponent<Animator>().SetTrigger("Sit");
        character.GetComponent<Player_Controller>().Turn(Vector2.left);
        character.GetComponent<Animator>().SetFloat("moveX", -1);
        character.GetComponent<Animator>().SetFloat("moveY", 0);
        yield return null;
    }

    private IEnumerator FadeBlanco()
    {
        fade.color = new Color(1f, 1f, 1f, 0f);
        fade.gameObject.SetActive(true);
        yield return DevTools.AnimarImage(fade, 1f,3f, fadeCurve);
        yield return null;
    }

    private IEnumerator EsperarYquitarFade(float duration)
    {
        yield return new WaitForSeconds(duration);
        fade.gameObject.SetActive(false);
    }
}

