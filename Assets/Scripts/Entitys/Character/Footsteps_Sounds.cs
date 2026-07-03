using UnityEngine;

public class Footsteps_Sounds : MonoBehaviour
{
    private Vector2 previousMoveInput = Vector2.zero;
    private AudioSource audioSource;
    private Player_Controller playerController;

    private enum FootstepDirection
    {
        None,
        Left,
        Right,
        Up,
        Down
    }

    private FootstepDirection currentDirection = FootstepDirection.None;


    //start
    void Start()
    {
        playerController = GetComponent<Player_Controller>();
    }

    void Update()
    {
        Vector2 moveInput  = playerController.moveInputVector;
        FootstepDirection newDirection = GetDirection(moveInput);

        // Si no cambió la dirección, no hacemos nada.
        if (newDirection == currentDirection) return;

        // Destruye el sonido anterior si existe.
        if (audioSource != null)
        {
            Destroy(audioSource.gameObject);
            audioSource = null;
        }

        // Si se quedó quieto, solo actualizamos el estado.
        if (newDirection == FootstepDirection.None)
        {
            currentDirection = newDirection;
            return;
        }

        // Elegimos el tipo de sonido.
        SoundType soundType;

        switch (newDirection)
        {
            case FootstepDirection.Left:
                soundType = SoundType.FOOTSTEP_LEFT;
                break;

            case FootstepDirection.Right:
                soundType = SoundType.FOOTSTEP_RIGHT;
                break;

            default: // Vertical
                soundType = SoundType.FOOTSTEP_VERTICAL;
                break;
        }

        // Reproducimos el nuevo sonido.
        audioSource = SoundFX_Manager.Instance.GetRandomClip(soundType);
        audioSource.volume = 0.7f;
        audioSource.pitch = 0.9f;
        audioSource.loop = true;
        audioSource.enabled = true;
        audioSource.Play();

        currentDirection = newDirection;
    }

    private FootstepDirection GetDirection(Vector2 moveInput)
    {
        if (moveInput == Vector2.zero)
            return FootstepDirection.None;

        if (moveInput.x > 0)
            return FootstepDirection.Right;

        if (moveInput.x < 0)
            return FootstepDirection.Left;

        if (moveInput.y > 0)
            return FootstepDirection.Up;

        return FootstepDirection.Down;
    }
}
