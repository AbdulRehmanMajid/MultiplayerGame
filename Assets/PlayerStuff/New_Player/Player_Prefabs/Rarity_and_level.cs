using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Rarity_and_level : NetworkBehaviour
{
    public bool is_explosive;
    [SerializeField] public NetworkVariable<int> Gun_level = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<int> Rareity_level = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    [ServerRpc(RequireOwnership = false)]
    public void Upgrade_gun_ServerRpc()
    {
        Gun_level.Value++;
        

    }
    [ServerRpc(RequireOwnership = false)]
    public void Upgrade_gun_Rarity_ServerRpc()
    {
       Rareity_level.Value++;
        

    }
    public void Check_Rarity()
    {
        if(!is_explosive)
        {
            temp_gun_script gun =  this.GetComponent<temp_gun_script>();
            gun.Check_Rarity();
            gun.elaspsed_time =0;
            gun.continous_fire = false;

        }

    }
}
