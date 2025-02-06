using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
public class Enable_input : NetworkBehaviour
{
    // Start is called before the first frame update
    public PlayerInput P_input;
    void Start()
    {
        if(!IsOwner)return;
        P_input.enabled = true;

        
    }

    
}
