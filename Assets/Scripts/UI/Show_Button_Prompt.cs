using UnityEngine;

public class Show_Button_Prompt : MonoBehaviour
{
    public Animator buttonAnimator;

    //cuando el trigger toque al jugador, se activará el prompt
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            buttonAnimator.SetTrigger("Appear");
        }
    }

    //cuando el trigger deje de tocar al jugador, se desactivará el prompt
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            buttonAnimator.SetTrigger("Disappear");
        }
    }
}
