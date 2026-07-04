using UnityEngine;
using System.Collections;

public class Cuadro : MonoBehaviour
{
    public int cuadroNum;
    public ZC_Nivel_4_Initiator nivelInitiator;


    private GameObject cuadro;

    void Start()
    {  
        cuadro = gameObject;

        switch(cuadroNum)
        {
            case 1:
                if(Save_Manager.Instance.data.cuadro1) {
                    cuadro.SetActive(false);
                }
                else
                {
                    cuadro.SetActive(true);
                }
                break;
            case 2:
                if(Save_Manager.Instance.data.cuadro2)
                {
                    cuadro.SetActive(false);
                }
                else
                {
                    cuadro.SetActive(true);
                }
                break;
            case 3:
                if(Save_Manager.Instance.data.cuadro3)
                {
                    cuadro.SetActive(false);
                }
                else
                {
                    cuadro.SetActive(true);
                }
                break;
        }

    }

    public void GuardarCuadro()
    {
        switch(cuadroNum)
        {
            case 1:
                if(Save_Manager.Instance.data.cuadro1) return;
                Save_Manager.Instance.data.cuadro1 = true;
                break;
            case 2:
                if(Save_Manager.Instance.data.cuadro2) return;
                Save_Manager.Instance.data.cuadro2 = true;
                break;
            case 3:
                if(Save_Manager.Instance.data.cuadro3) return;
                Save_Manager.Instance.data.cuadro3 = true;
                break;
        }

        SoundFX_Manager.Instance.PlaySound(SoundType.COGER_CUADRO);
        nivelInitiator.ComprobarCuadros();
        Save_Manager.Instance.SaveData();
        cuadro.SetActive(false);
    }
}