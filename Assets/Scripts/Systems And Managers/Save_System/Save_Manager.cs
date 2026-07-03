using System.IO;
using UnityEngine;

public class Save_Manager : MonoBehaviour
{
    // PATRON SINGLETON
    public static Save_Manager Instance;

    [HideInInspector] public Save_Data data;

    private string savePath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Debug.Log(Application.persistentDataPath);
        LoadData();
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<Save_Data>(json);
        }
        else
        {
            data = new Save_Data();
            SaveData();
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void ResetSaveData()
    {
        data = new Save_Data();
        SaveData();
    }
}
