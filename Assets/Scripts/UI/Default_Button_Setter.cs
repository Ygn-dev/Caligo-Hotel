using UnityEngine;

public class Default_Button_Setter : MonoBehaviour
{
    public GameObject botonInicial;

    void OnEnable()
    {
        Input_Schema_Manager.Instance.defaultCovered = botonInicial;
    }

    void ODisable()
    {
        Input_Schema_Manager.Instance.defaultCovered = null;
    }
}
