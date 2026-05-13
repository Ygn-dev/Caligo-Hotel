using UnityEngine;

public class Main_Menu_Functions : MonoBehaviour
{
    public void NewGame()
    {
        Game_Loader_Manager.Instance.NewGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
