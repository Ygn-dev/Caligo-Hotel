using UnityEngine;
using System.Collections;
public class SpawnSistemaPausa : MonoBehaviour
{
    public GameObject sistemaPausaPrefab;
    public float tiempoAparicion = 3.0f;
    //private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //isActive = true;
        StartCoroutine(AparecerSistemaPausa());
        MenuPausaSystem.InicializarSistemas(sistemaPausaPrefab);
    }
    private IEnumerator AparecerSistemaPausa()
    {
        GameObject canvasInstanciado = MenuPausaSystem.InicializarSistemas(sistemaPausaPrefab);
        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null)
        {
            collider.enabled = false;
        }

        if (canvasInstanciado == null)
        {
            Debug.LogError("No se pudo instanciar el Canvas de Pausa");
            Destroy(gameObject);
            yield break;
        }
        CanvasGroup cg = canvasInstanciado.GetComponentInChildren<CanvasGroup>();

        if (cg == null)
        {
            cg = canvasInstanciado.transform.GetChild(0).gameObject.AddComponent<CanvasGroup>();
        }

        cg.alpha = 0f;

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoAparicion)
        {
            tiempoTranscurrido += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, tiempoTranscurrido / tiempoAparicion);
            yield return null;
        }

        cg.alpha = 1f;

        Destroy(gameObject);
    }
}
