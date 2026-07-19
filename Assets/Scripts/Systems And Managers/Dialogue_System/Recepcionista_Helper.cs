using UnityEngine;

public class Recepcionista_Helper : MonoBehaviour
{
    public string dialogueID1;
    public string dialogueID2;
    public float zoomCamara;

    public void TriggerDialogue()
    {
        if (Save_Manager.Instance.data.habloConRecepcionista)
        {
            Dialogue_Manager.Instance.StartDialogue(dialogueID2, zoomCamara);
        }
        else
        {
            Dialogue_Manager.Instance.StartDialogue(dialogueID1, zoomCamara);
            Save_Manager.Instance.data.habloConRecepcionista = true;
            Save_Manager.Instance.SaveData();
        }
    }
}
