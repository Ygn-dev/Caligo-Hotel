using UnityEngine;

public class Cinematic_Manager : MonoBehaviour
{
    //SINGLETON
    public static Cinematic_Manager Instance { get; private set; }

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }

    public void PlayCinematic(string cinematicName)
    {
        // Aquí puedes implementar la lógica para reproducir la cinemática
        Debug.Log("Reproduciendo cinemática: " + cinematicName);
    }
}
