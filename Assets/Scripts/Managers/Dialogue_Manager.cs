using UnityEngine;
using System.Collections;

public class Dialogue_Manager : MonoBehaviour
{
    //SINGLETON
    public static Dialogue_Manager Instance { get; private set; }


    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    public void StartDialogue(TextAsset csvFile, float zoomCamara, float camPosX, float camPosY)
    {
        StartCoroutine(DialogueCoroutine(csvFile, zoomCamara, camPosX, camPosY));
    }

    public IEnumerator StartDialogueCoroutine(TextAsset csvFile, float zoomCamara, float camPosX, float camPosY)
    {
        yield return StartCoroutine(DialogueCoroutine(csvFile, zoomCamara, camPosX, camPosY));
    }



    private IEnumerator DialogueCoroutine(TextAsset csvFile, float zoomCamara, float camPosX, float camPosY)
    {
        yield return null;
    }
}