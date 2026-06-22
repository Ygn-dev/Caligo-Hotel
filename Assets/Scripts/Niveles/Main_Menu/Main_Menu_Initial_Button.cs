using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Main_Menu_Initial_Button : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject newGameButton;
    public Button buttonDefault;
    public Button buttonCargarJuego;
    public Button ajustesButton;

    private GameObject primerBoton;


    void Awake()
    {
        DeterminarPrimerBoton();
    }

    private void DeterminarPrimerBoton()
    {
        //Por ahora se determina el nuevo juego de manera automatica
        primerBoton = newGameButton;
        continueButton.GetComponent<Button>().interactable = false;

        //cambiar la navegacion de los demas botones
        Navigation nav = buttonDefault.navigation;
        nav.mode = Navigation.Mode.Explicit;
        nav.selectOnUp = newGameButton.GetComponent<Button>();
        nav.selectOnDown = newGameButton.GetComponent<Button>();
        nav.selectOnLeft = newGameButton.GetComponent<Button>();
        nav.selectOnRight = newGameButton.GetComponent<Button>();
        buttonDefault.navigation = nav;


        Navigation nav2 = newGameButton.GetComponent<Button>().navigation;
        nav2.mode = Navigation.Mode.Explicit;
        nav2.selectOnLeft = buttonCargarJuego;
        newGameButton.GetComponent<Button>().navigation = nav2;

        Navigation nav3 = buttonCargarJuego.navigation;
        nav3.mode = Navigation.Mode.Explicit;
        nav3.selectOnRight = newGameButton.GetComponent<Button>();
        buttonCargarJuego.navigation = nav3;

        Navigation nav4 = ajustesButton.navigation;
        nav4.mode = Navigation.Mode.Explicit;
        nav4.selectOnUp = ajustesButton;
        nav4.selectOnDown = ajustesButton;
        ajustesButton.navigation = nav4;
    }

    public void MostrarPrimerBoton()
    {
        if(Input_Schema_Manager.Instance.isCursorMode) return;
        EventSystem.current.SetSelectedGameObject(primerBoton);
    }
}
