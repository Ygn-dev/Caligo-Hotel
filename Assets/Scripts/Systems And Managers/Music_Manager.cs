using System;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum MusicType
{
    PORTADA,
    ZONA_CAMARAS,
    PREPORTADA
}

[Serializable]
public struct MusicList
{
    [HideInInspector] public string name;
    public AudioClip[] sounds;
}

public class Music_Manager : MonoBehaviour
{
    //SINGLETON
    public static Music_Manager Instance { get; private set; }
    [SerializeField] public MusicList[] musicList;

    private string prefabPath = "Prefabs/Audio/Music_Prefab";

    private AudioSource currentMusicSource;
    private MusicType currentMusicType;
    private int currentMusicIndex;

    //singleton pattern
    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    public void PlayMusic(MusicType musicType, float volume = 1f)
    {
        //select clip from the array
        AudioClip[] clips = musicList[(int)musicType].sounds;
        int index = UnityEngine.Random.Range(0, clips.Length);
        AudioClip randomClip = clips[index];

        if(musicType == currentMusicType && currentMusicIndex == index && currentMusicSource != null && currentMusicSource.isPlaying)
        {
            return;
        }
        currentMusicType = musicType;
        currentMusicIndex = index;


        //spawn in  gameObject
        AudioSource newMusicSource = Instantiate(Resources.Load<AudioSource>(prefabPath), transform);
        //assign the audioClip
        newMusicSource.clip = randomClip;
        //assign volume
        newMusicSource.volume = volume;
        //enable
        newMusicSource.enabled = true;

        //transicion de musica
        if(currentMusicSource != null)
        {
            newMusicSource.volume = 0f;
            StartCoroutine(FadeOutAndStop(currentMusicSource, 1f));
            StartCoroutine(FadeInAndPlay(newMusicSource, volume, 1f));
        }

        //play sound
        newMusicSource.Play();
        currentMusicSource = newMusicSource;
    }

    public void StopMusic(float fadeDuration = 1f)
    {
        if (currentMusicSource != null)
        {
            StartCoroutine(FadeOutAndStop(currentMusicSource, fadeDuration));
            currentMusicSource = null;
        }
    }

    private IEnumerator FadeOutAndStop(AudioSource audioSource, float duration = 1f)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        audioSource.Stop();
        Destroy(audioSource.gameObject);
    }

    private IEnumerator FadeInAndPlay(AudioSource audioSource, float targetVolume, float duration = 1f)
    {
        audioSource.volume = 0;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    
#if UNITY_EDITOR
    private void OnValidate()
    {
        string[] names = Enum.GetNames(typeof(MusicType));

        Array.Resize(ref musicList, names.Length);

        for (int i = 0; i < musicList.Length; i++)
            musicList[i].name = names[i];

        EditorUtility.SetDirty(this);
    }
#endif
}
