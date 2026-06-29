using UnityEngine;

public class Dialogue_Trigger : MonoBehaviour
{
    public string dialogueID;
    public float zoomCamara;
    public void TriggerDialogue()
    {
        Dialogue_Manager.Instance.StartDialogue(dialogueID, zoomCamara);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TriggerDialogue();
            GetComponent<Collider2D>().enabled = false; // Desactivar el trigger para que no se vuelva a activar
        }
    }
}
