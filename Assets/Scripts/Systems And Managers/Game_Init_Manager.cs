using UnityEngine;

public class Game_Init_Manager : MonoBehaviour
{
    private static bool initialized = false;

    //Se ejecuta antes de que se cargue cualquier escena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]  
    public static void Init()
    {
        if (initialized) return;

        GameObject persistObject = Resources.Load<GameObject>("Prefabs/PERSIST_OBJECT");
        Application.targetFrameRate = 60;

        if (persistObject != null)
        {
            DontDestroyOnLoad(Instantiate(persistObject));
        }
        else
        {
            Debug.LogError("No se encontró PERSIST_OBJECT en Resources");
        }

        initialized = true;
    }
}
