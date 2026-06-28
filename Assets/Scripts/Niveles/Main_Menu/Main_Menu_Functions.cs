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

    public void ReproducirSonido(string tipoSonido)
    {
        SoundType soundType = (SoundType)System.Enum.Parse(typeof(SoundType), tipoSonido);
        SoundFX_Manager.Instance.PlaySound(soundType);
    }
}
