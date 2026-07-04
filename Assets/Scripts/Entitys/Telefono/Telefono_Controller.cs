using UnityEngine;

public class Telefono_Controller : MonoBehaviour
{
    private bool estaActivado = false;

    //on trigger enter
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            estaActivado = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            estaActivado = false;
            Music_Manager.Instance.SetVolume(0F);
        }
    }

    void Update()
    {
        if (estaActivado)
        {
            float distancia = Vector2.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
            float volumenDB = Mathf.Lerp(-40f, 0f, Mathf.InverseLerp(3f, 13f, distancia));
            Music_Manager.Instance.SetVolume(volumenDB);
        }
    }
}
