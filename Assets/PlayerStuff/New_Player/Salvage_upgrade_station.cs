using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salvage_upgrade_station : MonoBehaviour
{
    public int upgrade_cost = 100;

    void OnTriggerEnter(Collider other)
    {
        gunMangerV2 gunManager = other.GetComponent<gunMangerV2>();
        if (gunManager != null)
        {
            gunManager.Gun_Rareity_active = true;
            gunManager.gun_Rareity_upgrade_cost = upgrade_cost;
        }
    }

    void OnTriggerExit(Collider other)
    {
        gunMangerV2 gunManager = other.GetComponent<gunMangerV2>();
        if (gunManager != null)
        {
            gunManager.Gun_Rareity_active = false;
        }
    }
}
