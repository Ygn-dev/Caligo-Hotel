using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum MusicType
{
    PORTADA,
    ZONA_CAMARAS
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
        

        //spawn in  gameObject
        currentMusicSource = Instantiate(Resources.Load<AudioSource>(prefabPath), transform);
        //assign the audioClip
        currentMusicSource.clip = randomClip;
        //assign volume
        currentMusicSource.volume = volume;
        //enable
        currentMusicSource.enabled = true;
        //play sound
        currentMusicSource.Play();
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
