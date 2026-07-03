using UnityEngine;

public class Recepcionista_Helper : MonoBehaviour
{
    public string dialogueID1;
    public string dialogueID2;
    public float zoomCamara;

    public void TriggerDialogue()
    {
        if(Save_Manager.Instance.data.habloConRecepcionista)
        {
            Debug.Log("Ya hablé con la recepcionista, iniciando diálogo 2");
            Dialogue_Manager.Instance.StartDialogue(dialogueID2, zoomCamara);
        }else{
            Debug.Log("No he hablado con la recepcionista, iniciando diálogo 1");
            Save_Manager.Instance.data.habloConRecepcionista = true;
            Save_Manager.Instance.SaveData();
            Dialogue_Manager.Instance.StartDialogue(dialogueID1, zoomCamara);
        }        
    }
}
