using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;



public class PlayerPrefabSpawner : NetworkBehaviour
{
  
    public List<GameObject> Player_Skins = new List<GameObject>();
   

   
    



    
    public override void OnNetworkSpawn()
     {
        if(!IsOwner)return;
        SpawnPlayer_ServerRpc(OwnerClientId);
       

     }
     [ServerRpc(RequireOwnership = false)]
     public void SpawnPlayer_ServerRpc(ulong clientid)
     {
        int number = Random.Range(0,100);
        int val = 0;
        if(number > 50)
        {
            val = 1;
        }
        else
        {
            val = 0;
        }
        GameObject player = Instantiate(Player_Skins[val],this.transform.position,this.transform.rotation);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientid);
       
        this.NetworkObject.Despawn();

     }




}
