using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Revive_Script : NetworkBehaviour
{
    public Transform my_collider;
    public Player_Health p_Health;
    public Revive_Script_player my_Rev_script;
    public float distance = 3f;

    void Update()
    {
        // Only process if this player's health is not alive.
        if (p_Health.Is_alive.Value)
            return;
        
        // Find all colliders within a radius around my_collider.
        Collider[] colliders = Physics.OverlapSphere(my_collider.position, 2.5f);

        foreach (var col in colliders)
        {
            // Skip if the collider belongs to my_collider.
            if (col.transform == my_collider)
                continue;

            // Cache the Revive_Script_player reference.
            Revive_Script_player revivePlayer = col.GetComponent<Revive_Script_player>();
            if (revivePlayer == null || revivePlayer == my_Rev_script)
                continue;
            
            // Assign this Revive_Script and my_collider as the source data.
            revivePlayer.revive_Script = this;
            revivePlayer.other_collider = my_collider;
        }
    }
}

