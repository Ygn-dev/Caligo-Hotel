using UnityEngine;

public class Palanca_Animations : MonoBehaviour
{
    public Animator animator;

    public void ActivarPalanca()
    {
        animator.SetTrigger("Activate");
    }
}
