using UnityEngine;

public class Main_Menu_Functions : MonoBehaviour
{
    private bool isSelected = false;
    public Extras_Func extras;

    private void OnEnable()
    {
        isSelected = false;
    }

    public void NewGame()
    {
        if(isSelected) return;
        isSelected = true;
        SoundFX_Manager.Instance.PlaySound(SoundType.CLICK_JUGAR);
        Music_Manager.Instance.StopMusic();
        Save_Manager.Instance.ResetSaveData();
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

    public void Extras()
    {
        extras.ActivarExtras();
    }
}
