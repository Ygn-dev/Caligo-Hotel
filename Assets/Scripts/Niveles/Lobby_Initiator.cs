using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Lobby_Initiator : MonoBehaviour
{
    public InputActionAsset inputActions;
    public GameObject characterPrefab;

    void Start()
    {
        StartCoroutine(InitializeLobby());
    }

    private IEnumerator InitializeLobby()
    {
        // Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());
        
        //Settear Personaje mientras se reproduce la cinemática
        bool spawned = false;
        StartCoroutine(SpawnCharacter(() => spawned = true));
        //yield return StartCoroutine(CinematicaInicial());
        yield return new WaitUntil(() => spawned);
        
        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();

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
        GameObject character = Instantiate(characterPrefab);
        onSpawned?.Invoke();
        yield return null;
    }
}

