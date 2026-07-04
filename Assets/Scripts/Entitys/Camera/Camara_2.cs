using UnityEngine;

public class Camara_2 : MonoBehaviour
{
    public Camara_Behavior camaraBehavior;

    private bool isIdle;
    private bool primeraVez = false;

    void Start()
    {
        Debug.Log("Camara_2: Start called.");
        if (camaraBehavior.modoCamara == ModoCamara.Idle)
        {
            Debug.Log("Camara_2: Camera is in Idle mode.");
            isIdle = true;
        }
        else
        {
            Debug.Log("Camara_2: Camera is not in Idle mode.");
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
