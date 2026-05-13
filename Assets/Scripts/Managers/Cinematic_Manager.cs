using UnityEngine;
using UnityEngine.Video;
using System;

public class Cinematic_Manager : MonoBehaviour
{
    //SINGLETON
    public static Cinematic_Manager Instance { get; private set; }

    public GameObject rendererPrefab;
    public VideoPlayer videoPlayer;

    private Canvas canvas;
    private Action onFinishedCallback;
    private GameObject rendererInstance;

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        canvas = FindAnyObjectByType<Canvas>();
    }

    public void PlayCinematic(string cinematicName, Action onFinished = null)
    {
        // Limpieza defensiva (IMPORTANTE)
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.prepareCompleted -= OnPrepared;

        // Callback
        onFinishedCallback = onFinished;

        // Renderer
        rendererInstance = Instantiate(rendererPrefab, canvas.transform);

        // Video
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, cinematicName + ".mp4");
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;

        // Eventos
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;

        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (rendererInstance != null) Destroy(rendererInstance);
        vp.loopPointReached -= OnVideoFinished;
        onFinishedCallback?.Invoke();
        onFinishedCallback = null;
    }


}
