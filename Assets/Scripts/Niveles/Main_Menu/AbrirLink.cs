using UnityEngine;

public class AbrirLink : MonoBehaviour
{
    public void AbrirEnlace(string url)
    {
        Application.OpenURL(url);
    }
}
