using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Object_Visibility : NetworkBehaviour
{
    private NetworkObject _net_object;
    public float range = 60f;
    
    void Start()
    {
        _net_object = GetComponent<NetworkObject>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!IsServer)return;
        foreach(var client in NetworkManager.Singleton.ConnectedClients)
        {
            if(client.Key == NetworkManager.LocalClientId) continue;
            var clientTransform = client.Value.PlayerObject.transform;
            bool isVisible = Vector3.Distance(clientTransform.position,transform.position)< range;
            if(_net_object.IsNetworkVisibleTo(client.Key)!= isVisible)
            {
                if(isVisible)
                {
                    _net_object.NetworkShow(client.Key);
                }
                else
                {
                    _net_object.NetworkHide(client.Key);
                }
            }

        }
        
    }
}
