using UnityEngine;
using UnityEngine.UI;

public class DialogueScroll_Helper : MonoBehaviour
{
    public Image scrollImage;
    public GameObject content;   
    public GameObject leyenda;
    public ScrollRect scrollRect;
    public Image scrollImageBackground;
    

    public void MostrarScroll()
    {
        scrollImage.enabled = true;
        scrollImageBackground.enabled = true;
    }

    public void OcultarScroll()
    {
        scrollImage.enabled = false;
        scrollImageBackground.enabled = false;
    }
}
