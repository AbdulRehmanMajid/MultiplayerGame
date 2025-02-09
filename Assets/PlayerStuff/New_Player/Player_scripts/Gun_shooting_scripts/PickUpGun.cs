using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PickUpGun : NetworkBehaviour
{
    
    public GameObject gun_to_pick;
    public PickUpGun my_pickupScript;
    
    
    public int Gun_id;
    public string gun_pickup_name;
    public bool enable = false;
   
    public world_gun_data_holder gun_data;    

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Gun_pickup>()!=null && enable)
        {
            gunMangerV2 gunmanager = other.GetComponent<Gun_pickup>().gunMangerV2;
            gunmanager.gunPickUp_text.text = gun_pickup_name;
             gunmanager.gun_pick_script = my_pickupScript;
          
          
            gunmanager.in_pickup_range = true;
            

        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Gun_pickup>()!=null && enable)
        {
             gunMangerV2 gunmanager = other.GetComponent<Gun_pickup>().gunMangerV2;
            gunmanager.gunPickUp_text.text = "";
           gunmanager.gun_pick_script = null;
            gunmanager.gun_pickup_name = "";
              gunmanager.gun_pick_script = null;
            gunmanager.in_pickup_range = false;
            

        }

    }
    [ServerRpc(RequireOwnership = false)]
    public void Kill_gun_ServerRpc()
    {
        //Destroy(gun_to_pick);
        gun_to_pick.GetComponent<NetworkObject>().Despawn(true);
        Debug.LogError("Killing");

       // kill_ClientRpc();

    }

    [ClientRpc]
    void kill_ClientRpc()
    {
        Destroy(gun_to_pick);
    }
  

  

}
