using UnityEngine;

public class Level_Trasition_Helper : MonoBehaviour
{
    private bool isLevelTransition = false;
    public void LoadScene(string sceneName)
    {
        if(Game_Loader_Manager.Instance != null && !isLevelTransition)
        {
            isLevelTransition = true;
            Game_Loader_Manager.Instance.LoadScene(sceneName);
        }
    }

    public void NivelBloqueado()
    {
        if(SoundFX_Manager.Instance != null)
            SoundFX_Manager.Instance.PlaySound(SoundType.PUERTA_BLOQUEADA);
        else
        {
            Debug.LogError("No se encontró el SoundFX_Manager en la escena. Asegúrate de que esté presente para reproducir efectos de sonido correctamente.");
        }
    }
}
