using UnityEngine;
using UnityEngine.Events;

public class Palanca_Behavior : MonoBehaviour, IInteractuable
{
    public UnityEvent onActivarPalanca;

    public void Interactuar()
    {
        onActivarPalanca?.Invoke();
    }
}
