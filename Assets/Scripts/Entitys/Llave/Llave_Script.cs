using UnityEngine;

public class Llave_Script : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Save_Manager.Instance.data.tieneLlaveN2 == true)
        {
            spriteRenderer.enabled = false;
        }
    }
}
