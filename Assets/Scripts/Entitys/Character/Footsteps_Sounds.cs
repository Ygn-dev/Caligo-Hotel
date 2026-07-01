using UnityEngine;

public class Footsteps_Sounds : MonoBehaviour
{
    private Vector2 previousMoveInput = Vector2.zero;
    private AudioSource audioSource;

    void Update()
    {
        Vector2 currentMoveInput = GetComponent<Player_Controller>().moveInputVector;   

        // Detecta cambio de estado: quieto -> movimiento
        if (previousMoveInput == Vector2.zero && currentMoveInput != Vector2.zero)
        {
            // Reproduce el sonido de pasos

            audioSource = SoundFX_Manager.Instance.GetRandomClip(SoundType.FOOTSTEP);
            audioSource.volume = 0.7f;
            audioSource.pitch = 0.9f;
            audioSource.loop = true;
            audioSource.enabled = true;
            audioSource.Play();
        }

        // Detecta cambio de estado: movimiento -> quieto
        else if (previousMoveInput != Vector2.zero && currentMoveInput == Vector2.zero)
        {
            Destroy(audioSource.gameObject);
        }
        // Actualiza el estado anterior
        previousMoveInput = currentMoveInput;
    }
}
