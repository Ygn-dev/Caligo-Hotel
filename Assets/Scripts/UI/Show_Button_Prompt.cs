using UnityEngine;

public class Show_Button_Prompt : MonoBehaviour
{
    [Header("No Editable")]
    public bool showPrompt;

    //cuando el trigger toque al jugador, se activará el prompt
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Show Prompt");
        }
    }
}
