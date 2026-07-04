using UnityEngine;

public class Dialogue_Trigger_Helper : MonoBehaviour
{
    public string dialogueID;
    public float zoomCamara;
    
    public void TriggerDialogue()
    {
        Dialogue_Manager.Instance.StartDialogue(dialogueID, zoomCamara);
    }

    public void TriggerDialogueOnlyOnce()
    {
        if (!Save_Manager.Instance.data.habloPorTelefonoZC2)
        {
            Save_Manager.Instance.data.habloPorTelefonoZC2 = true;
            Save_Manager.Instance.SaveData();
            TriggerDialogue();
        }
    }
}
