using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Game_Loader_Manager : MonoBehaviour
{
    //SINGLETON
    public static Game_Loader_Manager Instance { get; private set; }

    private Canvas canvas;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
        canvas = FindAnyObjectByType<Canvas>();
    }


    public void NewGame()
    {
        StartCoroutine(LoadSceneAsync("Lobby"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Pantalla_Carga loader = canvas.GetComponentInChildren<Pantalla_Carga>().GetComponent<Pantalla_Carga>();
        yield return StartCoroutine(loader.FadeIn(1f));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(loader.FadeOut(1f));
        asyncLoad.allowSceneActivation = true;
        yield return null;
    }


    
}
