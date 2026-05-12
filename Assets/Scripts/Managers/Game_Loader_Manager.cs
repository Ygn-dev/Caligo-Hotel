using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Game_Loader_Manager : MonoBehaviour
{
    //SINGLETON
    public static Game_Loader_Manager Instance { get; private set; }

    public Canvas canvas;
    public Pantalla_Carga loader;

    private bool isLoadingScreenActive;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }


    public void NewGame()
    {
        StartCoroutine(LoadSceneAsync("Lobby"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        canvas.gameObject.SetActive(true);
        yield return StartCoroutine(loader.FadeIn(1f));
        isLoadingScreenActive = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)yield return null;

        yield return new WaitForSeconds(1f);
        yield return null;
    }

    public IEnumerator CompleteLoadScene()
    {
        if(!isLoadingScreenActive) yield break;
        yield return StartCoroutine(loader.FadeOut(1f));
        canvas.gameObject.SetActive(false);
        isLoadingScreenActive = false;
        yield return null;
    }


    
}
