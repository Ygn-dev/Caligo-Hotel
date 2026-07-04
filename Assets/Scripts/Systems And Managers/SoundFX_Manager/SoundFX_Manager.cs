using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum SoundType
{
    CLICK_JUGAR,
    SELLECIONADO,
    PUERTA_BLOQUEADA,
    ABRIR_PUERTA,
    FOOTSTEP_LEFT,
    FOOTSTEP_RIGHT,
    FOOTSTEP_VERTICAL,
    MUERTE,
    VISTO,
    REVIVE,
    PALANCA,
    ATENDER_TELEFONO,
    APARECE_CAJA,
    ABRIR_CAJA,
    ABRIR_DIALOGO,
    ABRIR_CAJA_DIALOGO,
    PASAR_HOJA,
    TYPEWRITER,
    PAUSE,
    COGER_LLAVE,
    USAR_LLAVE,
    COGER_CUADRO,
    CAMARA_SE_DETUVO,
    CAMARA_SE_MUEVE
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    public AudioClip[] sounds;
}


[ExecuteInEditMode]

public class SoundFX_Manager : MonoBehaviour
{
    public static SoundFX_Manager Instance;
    [SerializeField] public SoundList[] soundList;

    private string prefabPath = "Prefabs/Audio/SoundFX_Prefab";

    //singleton pattern
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlaySound(SoundType soundType, float volume = 1f)
    {
        //select random clip from the array
        AudioClip[] clips = soundList[(int)soundType].sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        //spawn in  gameObject
        AudioSource audioSource = Instantiate(Resources.Load<AudioSource>(prefabPath), transform);
        //assign the audioClip
        audioSource.clip = randomClip;
        //assign volume
        audioSource.volume = volume;
        //Enable
        audioSource.enabled = true;
        //play sound
        audioSource.Play();
        //get length of the clip
        float clipLength = randomClip.length;
        //destroy the audioSource after the clip has finished playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource GetRandomClip(SoundType soundType)
    {
        //select random clip from the array
        AudioClip[] clips = soundList[(int)soundType].sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        //spawn in  gameObject
        AudioSource audioSource = Instantiate(Resources.Load<AudioSource>(prefabPath), transform);
        //assign the audioClip
        audioSource.clip = randomClip;
        //return the audioSource
        return audioSource;
    }


    /*
    public void PlayRandomPitch(SoundType soundType, float volume = 1f, float minPitch = 0.8f, float maxPitch = 1.2f)
    {
        //select random clip from the array
        AudioClip[] clips = soundList[(int)soundType].sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        //spawn in  gameObject
        AudioSource audioSource = Instantiate(Resources.Load<AudioSource>(prefabPath), Vector3.zero, Quaternion.identity);
        //assign the audioClip
        audioSource.clip = randomClip;
        //assign random pitch
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //get length of the clip
        float clipLength = randomClip.length / audioSource.pitch;
        //destroy the audioSource after the clip has finished playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource GetRandomClipWithPitch(SoundType soundType, float minPitch = 0.8f, float maxPitch = 1.2f)
    {
        //select random clip from the array
        AudioClip[] clips = soundList[(int)soundType].sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        //spawn in  gameObject
        AudioSource audioSource = Instantiate(Resources.Load<AudioSource>(prefabPath), Vector3.zero, Quaternion.identity);
        //assign the audioClip
        audioSource.clip = randomClip;
        //assign random pitch
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        //return the audioSource
        return audioSource;
    }*/

#if UNITY_EDITOR
    private void OnValidate()
    {
        string[] names = Enum.GetNames(typeof(SoundType));

        Array.Resize(ref soundList, names.Length);

        for (int i = 0; i < soundList.Length; i++)
            soundList[i].name = names[i];

        EditorUtility.SetDirty(this);
    }
#endif
}
