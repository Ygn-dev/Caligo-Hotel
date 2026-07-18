using UnityEngine;

public class Camara_2 : MonoBehaviour
{
    public Camara_Behavior camaraBehavior;

    private bool isIdle;
    private bool primeraVez = false;

    void Start()
    {
        if (camaraBehavior.modoCamara == ModoCamara.Idle)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isIdle && primeraVez == false)
            {
                primeraVez = true;
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
    }
}
