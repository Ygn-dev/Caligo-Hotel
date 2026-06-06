using UnityEngine;
using UnityEngine.Events;

public class Interact_Handler : MonoBehaviour, IInteractuable
{
    public UnityEvent onInteract;

    public void Interactuar()
    {
        onInteract?.Invoke();
    }
}
