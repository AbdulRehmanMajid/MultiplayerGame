using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PickUpGun : NetworkBehaviour
{
    #region Public Fields
    public GameObject gun_to_pick;
    public PickUpGun my_pickupScript;
    public int Gun_id;
    public string gun_pickup_name;
    public bool enable = false;
    public world_gun_data_holder gun_data;
    #endregion

    #region Unity Methods
    void OnTriggerEnter(Collider other)
    {
        if (!enable)
            return;

        // Use TryGetComponent to safely check for the Gun_pickup component.
        if (other.TryGetComponent<Gun_pickup>(out Gun_pickup pickup))
        {
            gunMangerV2 gunmanager = pickup.gunMangerV2;
            if (gunmanager != null)
            {
                gunmanager.gunPickUp_text.text = gun_pickup_name;
                gunmanager.gun_pick_script = my_pickupScript;
                gunmanager.in_pickup_range = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!enable)
            return;

        if (other.TryGetComponent<Gun_pickup>(out Gun_pickup pickup))
        {
            gunMangerV2 gunmanager = pickup.gunMangerV2;
            if (gunmanager != null)
            {
                gunmanager.gunPickUp_text.text = "";
                gunmanager.gun_pick_script = null;
                gunmanager.gun_pickup_name = "";
                gunmanager.in_pickup_range = false;
            }
        }
    }
    #endregion

    #region RPC Methods
    [ServerRpc(RequireOwnership = false)]
    public void Kill_gun_ServerRpc()
    {
        if (gun_to_pick != null)
        {
            NetworkObject netObj = gun_to_pick.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Despawn(true);
            }
        }
        Debug.LogError("Killing");
        // Optionally, you can call kill_ClientRpc() if needed.
        // kill_ClientRpc();
    }

    [ClientRpc]
    void kill_ClientRpc()
    {
        if (gun_to_pick != null)
        {
            Destroy(gun_to_pick);
        }
    }
    #endregion
}
