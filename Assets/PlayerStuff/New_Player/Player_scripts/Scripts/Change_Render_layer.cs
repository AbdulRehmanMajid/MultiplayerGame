using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Change_Render_layer : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject[] test;
    void Start()
    {
        if(IsOwner)return;
         for (int i = 0; i < test.Length; i++)
      {
        test[i].layer = LayerMask.NameToLayer("Default");
        

      }


    }

    
}
