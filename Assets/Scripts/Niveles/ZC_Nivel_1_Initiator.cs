using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class ZC_Nivel_1_Initiator : MonoBehaviour
{
    [Header("La parte editable está en su Scriptable Object")]
    
    [Space]
    [Header("No Editable")]
    public InputActionAsset inputActions;
    public ScriptableObject levelData;

    private GameObject character;
    private CinemachineCamera cinemachineCamera;
    private Level_Data_Base nivelData;
    void Awake()
    {
        inputActions.Disable();
        nivelData = (Level_Data_Base)levelData;
        if(cinemachineCamera == null) cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
        // Este nivel usara sistema de dialogos, asi que se asegura de que el Dialogue Manager exista en la escena
        if (Dialogue_Manager.Instance == null) DevTools.SetupDialogueManager();
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
            case "ZC_Nivel_2":
                spawnPoint = nivelData.spawnPoints[1];
                break;  
            default:
                spawnPoint = nivelData.spawnPoints[0];
                break;
        }
        
        
        Save_Manager.Instance.data.currentLevel = "ZC_Nivel_1";
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

        //Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());
       
        
        yield return null;
    }
}
