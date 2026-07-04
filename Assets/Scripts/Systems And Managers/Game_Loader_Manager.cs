using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class Game_Loader_Manager : MonoBehaviour
{
    //SINGLETON
    public static Game_Loader_Manager Instance { get; private set; }

    [Header("Editable")]
    public AnimationCurve curvaFade;
    public float duracionFade;

    [Space]

    [Header("No Editable")]
    public Canvas canvas;
    public Image fadeImage;
    public InputActionAsset inputActions;
    
    private bool isLoadingScreenActive;
    private Coroutine fadeIn;
    private Coroutine fadeOut;

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

    // CARGAR ESCENA
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Time.timeScale = 0f;

        if(fadeOut != null) StopCoroutine(fadeOut);

        //desactivar todos los inputs
        for(int i = 0; i < inputActions.actionMaps.Count; i++)
        {
            inputActions.actionMaps[i].Disable();
        }


        canvas.gameObject.SetActive(true);
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        fadeIn = StartCoroutine(FadeIn(duracionFade));
        yield return fadeIn;
        isLoadingScreenActive = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)yield return null;

        Time.timeScale = 1f;

        yield return new WaitForSeconds(1f);
        yield return null;
    }

    public IEnumerator CompleteLoadScene()
    {
        if(!isLoadingScreenActive) yield break;
        fadeOut = StartCoroutine(FadeOut(duracionFade));
        yield return fadeOut;
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
