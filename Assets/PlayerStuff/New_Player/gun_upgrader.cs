using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class gun_upgrader : NetworkBehaviour
{
    public int upgrade_cost = 5000;

    void OnTriggerEnter(Collider other)
    {
        gunMangerV2 gunManager = other.GetComponent<gunMangerV2>();
        if (gunManager != null)
        {
            gunManager.Gun_Upgrder_active = true;
            gunManager.gun_upgrade_cost = upgrade_cost;
        }
    }

    void OnTriggerExit(Collider other)
    {
        gunMangerV2 gunManager = other.GetComponent<gunMangerV2>();
        if (gunManager != null)
        {
            gunManager.Gun_Upgrder_active = false;
        }
    }
}
