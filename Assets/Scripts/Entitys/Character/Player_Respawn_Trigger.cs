using System;
using UnityEngine;

public class Player_Respawn_Trigger : MonoBehaviour
{
    public Player_Respawn playerRespawn;
    public LayerMask capaParedes;

    void Start()
    {
        capaParedes = LayerMask.GetMask("Pared");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("DeathZone"))
        {
            Vector2 origenLuz = other.transform.position;
            Vector2 posicionJugador = transform.position;

            Vector2 direccion = (posicionJugador - origenLuz).normalized;
            float distancia = Vector2.Distance(origenLuz, posicionJugador);

            RaycastHit2D hitPared = Physics2D.Raycast(origenLuz, direccion, distancia, capaParedes);

            if (hitPared.collider != null)
            {
                // ¡Hay una pared en medio! Estás a salvo en la sombra visual.
                Debug.Log("El jugador tocó el trigger, pero la pared '" + hitPared.collider.name + "' lo cubre. No muere.");
            }
            else
            {
                // No hay nada en medio, la luz te dio directo. ¡Respawn!
                Debug.Log("Luz directa detectada. Ejecutando Respawn.");
                playerRespawn.RequestRespawn();
            }
            //playerRespawn.RequestRespawn();
        }
    }
}
