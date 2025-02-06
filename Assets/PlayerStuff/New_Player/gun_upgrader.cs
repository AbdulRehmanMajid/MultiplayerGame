using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class gun_upgrader : NetworkBehaviour
{
    public int upgrade_cost = 5000;
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<gunMangerV2>()!=null)
        {
            other.GetComponent<gunMangerV2>().Gun_Upgrder_active = true;
            other.GetComponent<gunMangerV2>().gun_upgrade_cost = upgrade_cost;
        }

    }
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<gunMangerV2>()!=null)
        {
            other.GetComponent<gunMangerV2>().Gun_Upgrder_active = false;
        }
    }
}
