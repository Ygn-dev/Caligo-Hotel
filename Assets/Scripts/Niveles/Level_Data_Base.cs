using UnityEngine;

[CreateAssetMenu(fileName = "Level_Data_Base", menuName = "Scriptable Objects/Level_Data_Base")]
public class Level_Data_Base : ScriptableObject
{
    [Header("Nombre")]
    public string nivelName;

    [Header("Cámara")]
    public float camaraZoom;
    public Vector2 screenPositionComposer;
    public bool esDeadZone;
    public Vector2 deadZoneWidthHeight;
    public float damping;
    public float slowingDistance;

    [Header("Character Spawns")]
    public Vector3 spawnPoint;
    public Vector2[] spawnPoints;
}
