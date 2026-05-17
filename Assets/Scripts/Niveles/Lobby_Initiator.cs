using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;



public class Lobby_Initiator : MonoBehaviour
{
    public InputActionAsset inputActions;
    public GameObject characterPrefab;
    public Image fade;
    public AnimationCurve fadeCurve;

    private GameObject character;

    void Start()
    {
        StartCoroutine(InitializeLobby());
    }

    private IEnumerator InitializeLobby()
    {
        
        // Completar Fade de Carga
        //yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());
        //yield return StartCoroutine(FadeBlanco());

        //Settear Personaje mientras se reproduce la cinemática
        bool spawned = false;
        StartCoroutine(SpawnCharacter(() => spawned = true));
        /*StartCoroutine(EsperarYquitarFade(3f));
        yield return StartCoroutine(CinematicaInicial());
        yield return new WaitUntil(() => spawned);
        */

        //Levantarse del sillon
        yield return new WaitForSeconds(2f);
        character.GetComponent<Animator>().SetTrigger("WakeUp");
        
        //Esperar a que se presione X
        
        
        
        
        
        
        
        
        //Habilitar Input
        //inputActions.FindActionMap("Gameplay").Enable();
        yield return null;
    }

    private IEnumerator CinematicaInicial()
    {
        bool cinematicPlayed = false;
        
        Cinematic_Manager.Instance.PlayCinematic("Lobby_Cinematic", () =>
        {
            cinematicPlayed = true;
        });
        while (!cinematicPlayed) yield return null;
        yield return null;
    }

    private IEnumerator SpawnCharacter(System.Action onSpawned)
    {
        character = Instantiate(characterPrefab,new Vector3(7.36f, 1.65f, 0f), Quaternion.identity);
        character.GetComponent<Animator>().SetTrigger("Sit");
        onSpawned?.Invoke();
        yield return null;
    }

    private IEnumerator FadeBlanco()
    {
        fade.color = new Color(1f, 1f, 1f, 0f);
        fade.gameObject.SetActive(true);
        yield return DevTools.Animar(fade, 1f,3f, fadeCurve);
        yield return null;
    }

    private IEnumerator EsperarYquitarFade(float duration)
    {
        yield return new WaitForSeconds(duration);
        fade.gameObject.SetActive(false);
    }
}

