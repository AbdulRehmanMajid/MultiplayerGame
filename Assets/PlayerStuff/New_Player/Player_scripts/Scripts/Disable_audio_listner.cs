using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class Disable_audio_listner : NetworkBehaviour
{
    public AudioListener myaudiolistner;
    
    // Start is called before the first frame update
    void Start()
    {
        if(IsOwner)return;
        myaudiolistner.enabled = false;
        
        
    }

    
}
