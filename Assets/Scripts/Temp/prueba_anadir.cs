using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class prueba_anadir : MonoBehaviour
{
    public GameObject content;
    public void Instanciar(GameObject prefab)
    {
        GameObject instanciado = Instantiate(prefab, content.transform);
        // Aquí puedes agregar cualquier lógica adicional que necesites para el objeto instanciado
    }

    public void SetPadding(int altura)
    {
        VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        layoutGroup.padding.top = 750-altura;
    }

    public void Instanciar2(GameObject prefab)
    {
        StartCoroutine(Animar(prefab));
    }

    private IEnumerator Animar(GameObject prefab)
    {
        int alturaPrefab = (int)prefab.GetComponent<RectTransform>().rect.height;
        int paddingTop = content.GetComponent<VerticalLayoutGroup>().padding.top;
        GameObject instanciado = Instantiate(prefab, content.transform);
        
        float tiempoAnimacion = 2f; // Duración de la animación en segundos
        float tiempoTranscurrido = 0f;

        int posicionInicial = content.GetComponent<VerticalLayoutGroup>().padding.top;
        int posicionFinal = posicionInicial - (int)prefab.GetComponent<RectTransform>().rect.height; // Ajusta el valor según tus necesidades
        
        /*yield return null;
        /*
        while (tiempoTranscurrido < tiempoAnimacion)
        {
            float t = tiempoTranscurrido / tiempoAnimacion;
            content.GetComponent<VerticalLayoutGroup>().padding.top = (int)Mathf.Lerp(posicionInicial, posicionFinal, t);
            Debug.Log("Animando... Padding Top: " + content.GetComponent<VerticalLayoutGroup>().padding.top);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }*/

        //content.GetComponent<VerticalLayoutGroup>().padding.top = posicionFinal;
        yield return null;
    }
}
