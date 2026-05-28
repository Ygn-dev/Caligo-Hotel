using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class Game_Loader_Manager : MonoBehaviour
{
    //SINGLETON
    public static Game_Loader_Manager Instance { get; private set; }

    public AnimationCurve curvaFade;
    public float duracionFade;
    public Canvas canvas;
    public Image fadeImage;
    
    private bool isLoadingScreenActive;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    // INICIO DE NUEVO JUEGO
    public void NewGame()
    {
        StartCoroutine(LoadSceneAsync("Lobby"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        canvas.gameObject.SetActive(true);
        yield return StartCoroutine(FadeIn(duracionFade));
        isLoadingScreenActive = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)yield return null;

        yield return new WaitForSeconds(1f);
        yield return null;
    }

    public IEnumerator CompleteLoadScene()
    {
        if(!isLoadingScreenActive) yield break;
        yield return StartCoroutine(FadeOut(duracionFade));
        canvas.gameObject.SetActive(false);
        isLoadingScreenActive = false;
        yield return null;
    }

    public IEnumerator FadeIn(float duracion = 2.5f)
    {
        yield return DevTools.AnimarImage(fadeImage, 1f, duracion, curvaFade);
    }
    
    public IEnumerator FadeOut(float duracion = 2.5f)
    {
        yield return DevTools.AnimarImage(fadeImage, 0f, duracion, curvaFade);
    }
}
