using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Stuff_Disabler : NetworkBehaviour
{
    public GameObject[] test;
    void Start()
    {
        if(IsOwner)return;
         for (int i = 0; i < test.Length; i++)
      {
        test[i].SetActive(false);

      }

    }
   
}
