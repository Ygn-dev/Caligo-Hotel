using UnityEngine;

public class SoundFX_Helper : MonoBehaviour
{
    public void PlaySound(string soundType)
    {
        if (SoundFX_Manager.Instance != null)
        {
            SoundFX_Manager.Instance.PlaySound((SoundType)System.Enum.Parse(typeof(SoundType), soundType));
        }
        else
        {
            Debug.LogError("No se encontró el SoundFX_Manager en la escena. Asegúrate de que esté presente para reproducir efectos de sonido correctamente.");
        }
    }
}
