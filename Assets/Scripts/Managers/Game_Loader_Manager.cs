using UnityEngine;

public class Game_Loader_Manager : MonoBehaviour
{
    //SINGLETON
    public static Game_Loader_Manager Instance { get; private set; }

    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null) Instance = this;
    }


    
}
