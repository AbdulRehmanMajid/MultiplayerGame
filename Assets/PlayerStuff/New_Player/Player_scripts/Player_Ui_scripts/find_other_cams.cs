using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class find_other_cams : NetworkBehaviour
{
    public int Old_player_count;
    public Transform mycam;

    void Update()
    {
        if (!IsOwner)
            return;

        // Cache the player array to avoid multiple calls.
        Player_Health[] players = test.GetAllPlayers();
        int currentCount = players.Length;

        if (currentCount != Old_player_count)
        {
            Old_player_count = currentCount;
            foreach (Player_Health player in players)
            {
                // Assign the camera reference if the hp_shower is valid.
                if (player.hp_shower != null)
                    player.hp_shower.cam = mycam;
            }
        }
    }
}
