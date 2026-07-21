using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class ZC_Nivel_6_Initiator : MonoBehaviour
{
    [Header("La parte editable está en su Scriptable Object")]
    
    [Space]
    [Header("No Editable")]
    public InputActionAsset inputActions;
    public ScriptableObject levelData;

    public GameObject llave;

    private GameObject character;
    private Level_Data_Base nivelData;
    private CinemachineCamera cinemachineCamera;

    void Awake()
    {
        inputActions.Disable();
        nivelData = (Level_Data_Base)levelData;
        if(cinemachineCamera == null) cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        // Este nivel usara cinimatica, asi que se asegura de que el Cinematic Manager exista en la escena
        if (Cinematic_Manager.Instance == null) DevTools.SetupCinematicManager();
        //este nivel usara el sistema de dialogos asi que se debe llamar
        if(Dialogue_Manager.Instance == null) DevTools.SetupDialogueManager();
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Save_Manager.Instance.data.currentLevel = "ZC_Nivel_6";
        Save_Manager.Instance.SaveData();

        //Spawn personaje y camara
        StartCoroutine(DevTools.SetupCharacter(character, nivelData.spawnPoints[0], newCharacter => { character = newCharacter; }));
        StartCoroutine(DevTools.SetupCamara(cinemachineCamera, levelData, character));

        GameObject sistemaPausaPrefab = Resources.Load<GameObject>("Prefabs/UI/CanvasPausa");
        GameObject canvasInstanciado = MenuPausaSystem.InicializarSistemas(sistemaPausaPrefab);
        
        //Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());

        //Musica
        Music_Manager.Instance.PlayMusic(MusicType.ZONA_CAMARAS);

        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();
        yield return null;
    }

    public void AccionFinalPlayer()
    {
        StartCoroutine(AccionFinalPlayerCoroutine());
    }

    private IEnumerator AccionFinalPlayerCoroutine()
    {
        //Deshabilitar Input
        inputActions.FindActionMap("Gameplay").Disable();

        //Detener Musica
        Music_Manager.Instance.StopMusic();
        SoundFX_Manager.Instance.PlaySound(SoundType.COGER_LLAVE);

        //Desactivar la llave
        llave.SetActive(false);

        yield return Cinematic_Manager.Instance.PlayCinematic("Flashback_Cinematic");
        yield return StartCoroutine(Dialogue_Manager.Instance.StartDialogueCoroutine("nivel6_post_flashback", 6));

        inputActions.FindActionMap("Gameplay").Enable();
    }

    public void ReiniciarElBucle()
    {
        Music_Manager.Instance.StopMusic();
        Save_Manager.Instance.ResetSaveData();
        Game_Loader_Manager.Instance.NewGame();
    }
}
