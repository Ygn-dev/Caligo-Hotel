using UnityEngine;
using UnityEngine.InputSystem;

public class Level_Initiator_Plantilla : MonoBehaviour
{
    public InputActionAsset inputActions;

    void Start()
    {
        inputActions.FindActionMap("Gameplay").Enable();
        GameObject character = Resources.Load<GameObject>("Character");
        character.GetComponent<Player_Controller>().move = InputActionReference.Create(inputActions.FindAction("Move"));
        Instantiate(character);
    }
}
