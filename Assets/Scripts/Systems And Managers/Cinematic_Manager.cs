using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class Cinematic_Manager : MonoBehaviour
{
    //SINGLETON
    public static Cinematic_Manager Instance { get; private set; }

    public GameObject rendererPrefab;
    public VideoPlayer videoPlayer;
    public Canvas canvas;
    
    private GameObject rendererInstance;
    private bool videoFinished = false;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        //buscar por tag el canvas
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
    }

    public IEnumerator PlayCinematic(string cinematicName)
    {
        // Limpieza defensiva
        videoPlayer.Stop();
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.prepareCompleted -= OnPrepared;

        // Destruir renderer anterior si existe
        if (rendererInstance != null)
        {
            Destroy(rendererInstance);
            rendererInstance = null;
        }

        // Renderer
        rendererInstance = Instantiate(rendererPrefab, canvas.transform);
        rendererInstance.transform.SetAsFirstSibling();

        // Conectar renderer al VideoPlayer
        videoPlayer.targetMaterialRenderer = rendererInstance.GetComponent<Renderer>();

        // Video
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, cinematicName + ".mp4");
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;

        // Flag de control
        videoFinished = false;

        // Eventos
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();

        // Esperar hasta que el video termine completamente
        yield return new WaitUntil(() => videoFinished);
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnVideoFinished;

        if (rendererInstance != null)
        {
            Destroy(rendererInstance);
            rendererInstance = null;
        }

        videoFinished = true;
    }
}
