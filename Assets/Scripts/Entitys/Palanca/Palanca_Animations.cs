using UnityEngine;

public class Palanca_Animations : MonoBehaviour
{
    public Interact_Handler interactHandler;
    public Animator animator;

    void Awake()
    {
        interactHandler.onInteract.AddListener(ActivarPalanca);
    }

    public void ActivarPalanca()
    {
        animator.SetTrigger("Activate");
    }
}
