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
        SetAnimations();
        Turn();
    }

    void FixedUpdate()
    {
        //Movimiento
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(moveInputVector.x * speed, moveInputVector.y * speed);
    }

    private Vector2 Vector4Direcciones()
    {
        Vector2 direccion = move.action.ReadValue<Vector2>();
        if (Mathf.Abs(direccion.x) > Mathf.Abs(direccion.y))
            direccion = new Vector2(Mathf.Sign(direccion.x), 0);
        else if (Mathf.Abs(direccion.y) > 0)
            direccion = new Vector2(0, Mathf.Sign(direccion.y));
        return direccion;
    }

    private void SetAnimations()
    {
        if (moveInputVector != Vector2.zero) GetComponent<Animator>().SetBool("isRunning", true);
        else GetComponent<Animator>().SetBool("isRunning", false);

        if (moveInputVector == Vector2.zero) return;
        GetComponent<Animator>().SetFloat("moveX", moveInputVector.x);
        GetComponent<Animator>().SetFloat("moveY", moveInputVector.y);
    }

    private void Turn()
    {
        if (GetComponent<Rigidbody2D>().linearVelocity != Vector2.zero)
        {
            if (moveInputVector.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (moveInputVector.x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void Turn(Vector2 direction)
    {
        if (direction.x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}
