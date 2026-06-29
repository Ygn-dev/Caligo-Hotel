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
        if (cinemachineCamera == null) cinemachineCamera = FindAnyObjectByType<CinemachineCamera>();
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        //Spawn personaje y camara
        StartCoroutine(DevTools.SetupCharacter(character, nivelData, newCharacter => { character = newCharacter; }));
        StartCoroutine(DevTools.SetupCamara(cinemachineCamera, levelData, character));

        //Completar Fade de Carga
        yield return StartCoroutine(Game_Loader_Manager.Instance.CompleteLoadScene());

        //Habilitar Input
        inputActions.FindActionMap("Gameplay").Enable();
        yield return null;
    }
}
