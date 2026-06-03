using UnityEngine;
using System.Collections;

public class CamaraBehavior : MonoBehaviour
{
    private Player_Respawn playerRespawn;
    private void OnEnable()
    {
        StartCoroutine(WaitForPlayer());
    }

    private IEnumerator WaitForPlayer()
    {
        GameObject player = null;

        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        playerRespawn = player.GetComponent<Player_Respawn>();
        playerRespawn.RespawnEvent += OnPlayerRespawn;
    }

    private void OnDisable()
    {
        playerRespawn.RespawnEvent -= OnPlayerRespawn;
    }

    private void OnPlayerRespawn()
    {
        Debug.Log("[CamaraBehavior] El Player hizo respawn.");
    }


}
