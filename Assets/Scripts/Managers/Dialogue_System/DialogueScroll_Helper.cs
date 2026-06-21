using UnityEngine;
using UnityEngine.UI;

public class DialogueScroll_Helper : MonoBehaviour
{
    public GameObject content;   
    public ScrollRect scrollRect;

    public Image scrollImageBackground;
    public Image scrollImage;

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
