using System;
using UnityEngine;

public class Player_Respawn_Trigger : MonoBehaviour
{
    public Player_Respawn playerRespawn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("DeathZone"))
        {
            playerRespawn.RequestRespawn();
        }
    }
}
