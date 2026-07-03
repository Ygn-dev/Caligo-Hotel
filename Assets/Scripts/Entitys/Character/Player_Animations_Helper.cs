using UnityEngine;

public class Player_Animations_Helper : MonoBehaviour
{
    public void ForceMoveX(float value)
    {
        gameObject.GetComponent<Transform>().position += new Vector3(value, 0, 0);
    }

    public void ForceMoveY(float value)
    {
        gameObject.GetComponent<Transform>().position += new Vector3(0, value, 0);
    }

    public void ReproduceSound(string clip)
    {
        SoundFX_Manager.Instance.PlaySound((SoundType)System.Enum.Parse(typeof(SoundType), clip));
    }
}
