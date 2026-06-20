using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraPatrol : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float waitTime;
    [SerializeField] List<Transform> patrolPoints;
    [SerializeField] Transform cabezal;
    [SerializeField] float tiempoRotacion = 0.5f;
    [SerializeField] float offsetAngulo = 90f;
    private int currentPatrolIndex;
    private bool isWaiting;

    void Start()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;
        transform.position = patrolPoints[0].position;
        currentPatrolIndex = 0;
        if (patrolPoints.Count > 1 && cabezal != null)
        {
            Vector2 sentidoInicial = patrolPoints[1].position - transform.position;
            OrientarInstantaneo(sentidoInicial);
        }
    }
    void Update()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;
        if (!isWaiting && transform.position != patrolPoints[currentPatrolIndex].position)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                patrolPoints[currentPatrolIndex].position, speed * Time.deltaTime);
        }
        else if (!isWaiting)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        Vector2 puntoActual = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        Vector2 nuevoSentido = (Vector2)patrolPoints[currentPatrolIndex].position - puntoActual;
        if (cabezal != null)
        {
            yield return StartCoroutine(RotarCabezal(nuevoSentido));
        }
        isWaiting = false;
    }
    private void OrientarInstantaneo(Vector2 sentido)
    {
        if (sentido == Vector2.zero) return;
        float angle = Mathf.Atan2(sentido.y, sentido.x) * Mathf.Rad2Deg;
        cabezal.localRotation = Quaternion.Euler(0f, 0f, angle - offsetAngulo);
    }
    IEnumerator RotarCabezal(Vector2 sentido)
    {
        if (sentido == Vector2.zero || cabezal == null) yield break;

        Quaternion rotacionInicial = cabezal.rotation;
        float anguloObjetivo = (Mathf.Atan2(sentido.y, sentido.x) * Mathf.Rad2Deg) - offsetAngulo;
        Quaternion rotacionFinal = Quaternion.Euler(0f, 0f, anguloObjetivo);

        float tiempo = 0f;
        while (tiempo < tiempoRotacion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Clamp01(tiempo / tiempoRotacion);

            cabezal.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, progreso);
            yield return null;
        }

        cabezal.localRotation = rotacionFinal;
    }
}
