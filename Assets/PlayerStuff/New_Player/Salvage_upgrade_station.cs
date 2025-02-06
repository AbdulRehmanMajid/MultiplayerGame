using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salvage_upgrade_station : MonoBehaviour
{
    public int upgrade_cost = 100;
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<gunMangerV2>()!=null)
        {
            other.GetComponent<gunMangerV2>().Gun_Rareity_active = true;
            other.GetComponent<gunMangerV2>().gun_Rareity_upgrade_cost = upgrade_cost;
        }

    }
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<gunMangerV2>()!=null)
        {
            other.GetComponent<gunMangerV2>().Gun_Rareity_active = false;
        }
    }
}
