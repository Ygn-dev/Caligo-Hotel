using UnityEngine;

public class Level_Trasition_Helper : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if(Game_Loader_Manager.Instance != null)
            Game_Loader_Manager.Instance.LoadScene(sceneName);
        else
        {
            Debug.LogError("No se encontró el Game_Loader_Manager en la escena. Asegúrate de que esté presente para cargar escenas correctamente.");
        }
    }
}
