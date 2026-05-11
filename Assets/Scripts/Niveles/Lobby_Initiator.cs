using UnityEngine;
using UnityEngine.InputSystem;

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
        Cinematic_Manager.Instance.PlayCinematic("Lobby_Cinematic");
        inputActions.FindActionMap("Gameplay").Enable();
    }
}

