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
        if(other.GetComponent<Perks_manager>()!=null)
        {
           other.GetComponent<Perks_manager>().can_buy_perk = true;
            other.GetComponent<Perks_manager>().perk_id = perk_id;
              
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent <Perks_manager>()!=null)
        {
            other.GetComponent <Perks_manager>().can_buy_perk = false;
            other.GetComponent<Perks_manager>().perk_id = 0;
        }
    }
}
