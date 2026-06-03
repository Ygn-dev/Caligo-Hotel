using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class Player_Respawn : MonoBehaviour
{
    
    [Header("Editable")]
    public float duracion = 0.15f;
    public float amplitud = 0.08f;
    public float frecuencia = 45f;
    public float blinkFreq = 25f;

    [Space]
    [Header("No Editable")]
    public Animator animator;
    public InputActionReference move;
    public event Action RespawnEvent;
    public Transform characterTransform;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D DeathTrigger;
    [HideInInspector] public Level_Data_Base nivelData;

    


    private void OnEnable()
    {
        RespawnEvent += OnRespawn;
    }

    public void RequestRespawn()
    {
        RespawnEvent?.Invoke();
    }

    // Logica Respawn
    private void OnRespawn()
    {
        StartCoroutine(SecuenciaPreRespawn());
    }

    private IEnumerator SecuenciaPreRespawn()
    {
        yield return StartCoroutine(DesactivarMovimiento());
        yield return StartCoroutine(Vibrar());
        yield return StartCoroutine(TriggerAnimation());
        //Sigue en PostRespawn()
    }

    private IEnumerator PostRespawn()
    {
        Vector3 spawnPoint;

        if(nivelData == null) spawnPoint = Vector3.zero;
        else spawnPoint = nivelData.spawnPoint;
        
        characterTransform.localPosition = spawnPoint;
        yield return null;
    }

    private IEnumerator ActivarMovimiento()
    {
        move.action.Enable();
        DeathTrigger.enabled = true;
        animator.SetFloat("moveY",-1);
        yield return null;
    }

    private IEnumerator TriggerAnimation()
    {
        animator.SetTrigger("Die");
        yield return null;
    }

    private IEnumerator DesactivarMovimiento()
    {
        move.action.Disable();
        DeathTrigger.enabled = false;
        yield return null;
    }

    private IEnumerator Vibrar()
    {
        Vector3 restLocalPos = characterTransform.localPosition;

        // Color actual del sprite (para volver al final)
        Color originalColor = spriteRenderer.color;
        Color blinkColor = Color.black;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;

            // Vibración izquierda-derecha (onda cuadrada)
            float dir = Mathf.Sign(Mathf.Sin(t * Mathf.PI * 2f * frecuencia)); // -1 o +1
            characterTransform.localPosition = restLocalPos + Vector3.right * (dir * amplitud);

            // Parpadeo entre color original y blinkColor
            bool useOriginal = Mathf.Sin(t * Mathf.PI * 2f * blinkFreq) > 0f;
            spriteRenderer.color = useOriginal ? originalColor : blinkColor;

            yield return null;
        }

        // Restaurar
        characterTransform.localPosition = restLocalPos;
        spriteRenderer.color = originalColor;
    }
}
