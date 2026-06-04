using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interact : MonoBehaviour
{
    //[Header("Editable")]
    [Space]
    [Header("No Editable")]
    public InputActionReference Interact;

    private IInteractuable interactuableActual;

    private void OnEnable()
    {
        Interact.action.performed += OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractuable interactuable))
        {
            interactuableActual = interactuable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractuable interactuable))
        {
            if (interactuable == interactuableActual)
            {
                interactuableActual = null;
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (interactuableActual != null)
        {
            interactuableActual.Interactuar();
        }
    }

}
