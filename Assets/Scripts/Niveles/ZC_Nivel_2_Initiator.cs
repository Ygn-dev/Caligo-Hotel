using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class ZC_Nivel_2_Initiator : MonoBehaviour
{
    [Header("La parte editable está en su Scriptable Object")]
    
    [Space]
    [Header("No Editable")]
    public InputActionAsset inputActions;
    public ScriptableObject levelData;
    public GameObject puerta;
    public GameObject puertaBloqueada;
    public GameObject trigger;

    private GameObject character;
    private Level_Data_Base nivelData;
    private CinemachineCamera cinemachineCamera;

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
        Vector2 spawnPoint;
        
        switch(Save_Manager.Instance.data.currentLevel)
        {
            case "ZC_Nivel_1":
                spawnPoint = nivelData.spawnPoints[0];
                break;

            case "ZC_Nivel_3":
                spawnPoint = nivelData.spawnPoints[1];
                break;
            case "ZC_Nivel_4":
                spawnPoint = nivelData.spawnPoints[2];
                break;

            default:
                Debug.LogWarning("Current level not recognized.");
                spawnPoint = nivelData.spawnPoints[0];
                break;
        }

        Save_Manager.Instance.data.currentLevel = "ZC_Nivel_2";
        Save_Manager.Instance.SaveData();

        //Spawn personaje y camara
        StartCoroutine(DevTools.SetupCharacter(character, spawnPoint, newCharacter => { character = newCharacter; }));
        StartCoroutine(DevTools.SetupCamara(cinemachineCamera, levelData, character));

        GameObject sistemaPausaPrefab = Resources.Load<GameObject>("Prefabs/UI/CanvasPausa");
        GameObject canvasInstanciado = MenuPausaSystem.InicializarSistemas(sistemaPausaPrefab);
        
        //Musica
        Music_Manager.Instance.PlayMusic(MusicType.ZONA_CAMARAS);

        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();

        if(Save_Manager.Instance.data.tieneLlaveN2 == true) {
            puerta.SetActive(true);
            puertaBloqueada.SetActive(false);
            trigger.SetActive(false);
        }

        //Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());
        
        yield return null;
    }
}
