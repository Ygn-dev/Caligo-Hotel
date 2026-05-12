using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Lobby_Initiator : MonoBehaviour
{
    public InputActionAsset inputActions;

    void Awake()
    {
        GameObject character = Resources.Load<GameObject>("Character");
        character.GetComponent<Player_Controller>().move = InputActionReference.Create(inputActions.FindAction("Move"));
        Instantiate(character);
    }

    void Start()
    {
        StartCoroutine(seguirCinematic());
    }

    private IEnumerator seguirCinematic()
    {
        bool cinematicPlayed = false;
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());
        Cinematic_Manager.Instance.PlayCinematic("Lobby_Cinematic", () =>
        {
            cinematicPlayed = true;
        });
        while (!cinematicPlayed) yield return null;
        Debug.Log("Cinematic completed. Starting the game...");

        yield return null;
    }
}

