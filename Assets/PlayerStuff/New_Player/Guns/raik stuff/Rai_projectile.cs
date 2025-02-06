using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rai_projectile : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
        
       // add_audioSource_ServerRpc();
       
        Destroy(this.gameObject,3f);
        
        
        
    }

    // Update is called once per frame
   void OnTriggerEnter(Collider other)
   {
    
    Destroy(this.gameObject);
    
    
   }
}
