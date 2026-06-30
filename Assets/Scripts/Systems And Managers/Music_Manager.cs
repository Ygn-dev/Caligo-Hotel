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
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        //spawn in  gameObject
        AudioSource audioSource = Instantiate(Resources.Load<AudioSource>(prefabPath), Vector3.zero, Quaternion.identity);
        //assign the audioClip
        audioSource.clip = randomClip;
        //assign volume
        audioSource.volume = volume;
        //enable
        audioSource.enabled = true;
        //play sound
        audioSource.Play();
        //get length of the clip
        float clipLength = randomClip.length;
        //destroy the audioSource after the clip has finished playing
        Destroy(audioSource.gameObject, clipLength);
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
