using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class ZC_Nivel_4_Initiator : MonoBehaviour
{
    [Header("La parte editable está en su Scriptable Object")]
    
    [Space]
    [Header("No Editable")]
    public InputActionAsset inputActions;
    public ScriptableObject levelData;
    public AudioSource audioSource;

    private GameObject character;
    private Level_Data_Base nivelData;
    private CinemachineCamera cinemachineCamera;
    private bool isCuadreandoxd = false;

    void Awake()
    {
        inputActions.Disable();
        nivelData = (Level_Data_Base)levelData;
        if(cinemachineCamera == null) cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        //este nivel usara el sistema de dialogos asi que se debe llamar
        if(Dialogue_Manager.Instance == null) DevTools.SetupDialogueManager();
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Save_Manager.Instance.data.currentLevel = "ZC_Nivel_4";
        Save_Manager.Instance.SaveData();
        
        //Spawn personaje y camara
        StartCoroutine(DevTools.SetupCharacter(character, nivelData.spawnPoints[0], newCharacter => { character = newCharacter; }));
        StartCoroutine(DevTools.SetupCamara(cinemachineCamera, levelData, character));
        
        //Musica
        Music_Manager.Instance.PlayMusic(MusicType.ZONA_CAMARAS);

        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();

        if(Save_Manager.Instance.data.habloPorTelefonoZC4)
        {
            audioSource.Stop();
        }

        //Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());

        yield return null;
    }

    public void ContestarTelefono()
    {
        if(Save_Manager.Instance.data.habloPorTelefonoZC4) return;

        audioSource.Stop();
        Save_Manager.Instance.data.habloPorTelefonoZC4 = true;
        Save_Manager.Instance.SaveData();
        Dialogue_Manager.Instance.StartDialogue("nivel4_telefono",8);
    }

    public void InteractuarPuerta()
    {
        if(!Save_Manager.Instance.data.puedeAbrirPuertaZC4)
        {
            SoundFX_Manager.Instance.PlaySound(SoundType.PUERTA_BLOQUEADA);
            //Dialogue_Manager.Instance.StartDialogue("nivel4_puerta_abierta", 8);
        }
        else
        {
            SoundFX_Manager.Instance.PlaySound(SoundType.ABRIR_PUERTA);
            Game_Loader_Manager.Instance.LoadScene("ZC_Nivel_5");
        }
    }

    public void ComprobarCuadros()
    {
        StartCoroutine(ComprobarCuadrosCoroutine());
    }

    private IEnumerator ComprobarCuadrosCoroutine()
    {
        yield return new WaitForSeconds(0.7f);

        if (isCuadreandoxd) yield break;
        isCuadreandoxd = true;
        if(Save_Manager.Instance.data.cuadro1 && Save_Manager.Instance.data.cuadro2 && Save_Manager.Instance.data.cuadro3)
        {
            SoundFX_Manager.Instance.PlaySound(SoundType.PALANCA);
            Save_Manager.Instance.data.puedeAbrirPuertaZC4 = true;
        }

        Save_Manager.Instance.SaveData();
        isCuadreandoxd = false;
        yield return null;
    }
}
