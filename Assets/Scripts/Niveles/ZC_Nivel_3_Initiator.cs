using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class ZC_Nivel_3_Initiator : MonoBehaviour
{
    [Header("La parte editable está en su Scriptable Object")]
    
    [Space]
    [Header("No Editable")]
    public InputActionAsset inputActions;
    public ScriptableObject levelData;
    public GameObject puerta;
    public GameObject puertaBloqueada;

    private GameObject character;
    private Level_Data_Base nivelData;
    private CinemachineCamera cinemachineCamera;

    void Awake()
    {
        inputActions.Disable();
        nivelData = (Level_Data_Base)levelData;
        if(cinemachineCamera == null) cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        if(Dialogue_Manager.Instance == null) DevTools.SetupDialogueManager();
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        Save_Manager.Instance.data.currentLevel = "ZC_Nivel_3";
        Save_Manager.Instance.SaveData();

        //Spawn personaje y camara
        StartCoroutine(DevTools.SetupCharacter(character, nivelData.spawnPoints[0], newCharacter => { character = newCharacter; }));
        StartCoroutine(DevTools.SetupCamara(cinemachineCamera, levelData, character));
        
        //Musica
        Music_Manager.Instance.PlayMusic(MusicType.ZONA_CAMARAS);

        GameObject sistemaPausaPrefab = Resources.Load<GameObject>("Prefabs/UI/CanvasPausa");
        GameObject canvasInstanciado = MenuPausaSystem.InicializarSistemas(sistemaPausaPrefab);

        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();
        yield return null;

        if(Save_Manager.Instance.data.tieneLlaveN2 == true) {
            puerta.SetActive(true);
            puertaBloqueada.SetActive(false);
        }

        //Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());
        
        yield return null;
    }

    public void CambiarEstadoPuerta()
    {
        puerta.SetActive(true);
        puertaBloqueada.SetActive(false);
    }
}
