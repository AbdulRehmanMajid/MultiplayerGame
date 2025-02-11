using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Perk_machine_script : NetworkBehaviour
{
    public int perk_id;
    public string Perk_name;

    void OnTriggerEnter(Collider other)
    {
        Perks_manager perksManager = other.GetComponent<Perks_manager>();
        if (perksManager != null)
        {
            perksManager.can_buy_perk = true;
            perksManager.perk_id = perk_id;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Perks_manager perksManager = other.GetComponent<Perks_manager>();
        if (perksManager != null)
        {
            perksManager.can_buy_perk = false;
            perksManager.perk_id = 0;
        }
    }
}
