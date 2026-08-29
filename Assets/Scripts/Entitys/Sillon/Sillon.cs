using UnityEngine;

public class Sillon : MonoBehaviour
{
    //cuando salga del overlap
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Activar el collider del sillon
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
