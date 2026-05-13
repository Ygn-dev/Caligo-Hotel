using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public float speed = 5f;
    public InputActionReference move;
    [HideInInspector] public Vector2 moveInputVector;
    
    void Update()
    {
        moveInputVector = Vector4Direcciones();
    }

    public Vector2 Vector4Direcciones()
    {
        Vector2 direccion = move.action.ReadValue<Vector2>();
        if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
            direccion = new Vector2(Mathf.Sign(direccion.x), 0);
        else if (Mathf.Abs(direccion.y) > 0)
            direccion = new Vector2(0, Mathf.Sign(direccion.y));
        return direccion;
    }

    void FixedUpdate()
    {
        //Movimiento
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(moveInputVector.x * speed, moveInputVector.y * speed);
    }
}
