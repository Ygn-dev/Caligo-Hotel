using UnityEngine;

public class Dialogue_Trigger_Helper : MonoBehaviour
{
    public string dialogueID;
    public float zoomCamara;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TriggerDialogue()
    {
        Dialogue_Manager.Instance.StartDialogue(dialogueID, zoomCamara);
    }
}
