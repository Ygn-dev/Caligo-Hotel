using UnityEngine;
using System.Collections;
using UnityEngine.Localization.Settings;
public class IdiomaController : MonoBehaviour
{
    public static IdiomaController instance;
    private bool active_controller = false;
    void Awake()
    {
        instance = this;
    }
    public void ChangeIdioma(int idioma_id)
    {
        if (active_controller)
        {
            return;
        }
        string idioma = "";
        switch (idioma_id)
        {
            case 0:
                idioma = "es";
                break;
            case 1:
                idioma = "en";
                break;
            default:
                idioma = "es";
                break;
        }
        Save_Manager.Instance.data.idioma = idioma;
        StartCoroutine(SetIdiomaCorrutine(idioma_id));
    }
    private IEnumerator SetIdiomaCorrutine(int idioma_id)
    {
        active_controller = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[idioma_id];
        active_controller = false;
    }
}
