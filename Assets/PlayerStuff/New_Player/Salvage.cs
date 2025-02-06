using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Salvage : NetworkBehaviour
{
    public GameObject salvage;
    public GameObject mesh;
    public int Value;
    
    public void OnTriggerEnter(Collider other)
    {
       
        if(other.GetComponent<Player_Health>())
        {
            mesh.GetComponent<MeshRenderer>().enabled =false;
            if(!IsServer)return;
            salvage.GetComponent<NetworkObject>().Despawn(true);
            other.GetComponent<Player_Health>().money_man.Salvage.Value += Value;
          
        }
    }

    
    
}
