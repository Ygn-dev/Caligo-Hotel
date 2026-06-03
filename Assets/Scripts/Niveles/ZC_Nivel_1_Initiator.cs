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
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        //Spawn personaje y camara
        StartCoroutine(SpawnCharacter());
        StartCoroutine(DevTools.SetupCamara(cinemachineCamera, levelData, character));
        
        //Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());

        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();
    }

    private IEnumerator SpawnCharacter()
    {
        if(character != null) yield break;
        character = Instantiate(Resources.Load<GameObject>("Prefabs/Character"), nivelData.spawnPoint, Quaternion.identity);
        character.GetComponent<Player_Respawn>().nivelData = nivelData;
        yield return null;
    }


}
