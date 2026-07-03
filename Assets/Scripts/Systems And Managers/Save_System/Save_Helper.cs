using UnityEngine;

public class Save_Helper : MonoBehaviour
{
    public void SetLlaveN2(bool tieneLlaveN2)
    {
        Save_Manager.Instance.data.tieneLlaveN2 = tieneLlaveN2;
        Save_Manager.Instance.SaveData();
    }
}
