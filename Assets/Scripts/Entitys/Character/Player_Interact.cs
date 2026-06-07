using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Interact : MonoBehaviour
{
    [Space]
    [Header("No Editable")]
    public InputActionReference Interact;

    private IInteractuable interactuableActual;
    private Collider2D otherCol;

    private void OnEnable()
    {
        Interact.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        Interact.action.performed -= OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractuable interactuable))
        {
            interactuableActual = interactuable;
            otherCol = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == otherCol)
        {
            interactuableActual = null;
            otherCol = null;
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