using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class HpBarRenderDisable : NetworkBehaviour
{
    public Canvas hpBar;
    
    void Start()
    {
        if(IsOwner)return;
        hpBar.enabled = true;

        
    }

    
}
