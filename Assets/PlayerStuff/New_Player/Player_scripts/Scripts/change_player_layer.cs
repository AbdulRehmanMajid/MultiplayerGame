using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class change_player_layer : NetworkBehaviour
{
    public GameObject playerbody;
    // Start is called before the first frame update
    void Start()
    {
        if(!IsOwner)return;
        playerbody.layer = LayerMask.NameToLayer("Default");
        
    }

    
}
