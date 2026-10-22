using UnityEngine;

public class AudioDebugger : MonoBehaviour
{
    void Start()
    {
        AudioListener[] listeners = FindObjectsByType<AudioListener>();
        Debug.Log($"[AudioDebug] AudioListeners encontrados en escena: {listeners.Length}");

        foreach (var al in listeners)
        {
            Debug.Log($"[AudioDebug] Listener en el objeto: {al.gameObject.name} | Activo: {al.enabled}");
        }

        AudioSource[] sources = FindObjectsByType<AudioSource>();
        Debug.Log($"[AudioDebug] Total de AudioSources en escena: {sources.Length}");

        foreach (var source in sources)
        {
            string clipName = source.clip != null ? source.clip.name : "NULO";
            Debug.Log($"[AudioDebug] Source en '{source.gameObject.name}' | Clip: {clipName} | IsPlaying: {source.isPlaying} | Vol: {source.volume}");
        }
    }
}
