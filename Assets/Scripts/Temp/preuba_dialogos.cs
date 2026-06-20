using UnityEngine;
using System.Collections;

public class preuba_dialogos : MonoBehaviour
{
    public float zoom = 5;

    void Awake()
    {
        StartCoroutine(WaitForDialogueManager());
    }

    private IEnumerator WaitForDialogueManager()
    {
        while (Dialogue_Manager.Instance == null)
        {
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Dialogue_Manager.Instance.StartDialogue("Testing", zoom);
    }
}
