using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class BlackHole : NetworkBehaviour
{
      public float pullRadius = 2;
   
    public float damage = 30f;
    public ulong attakerid;
    public float forward_speed = 2f;
  
    public GameObject electic_arc;
    public float time = 100f;
  
    //public arc_data_holder kebab;
    

    // Update is called once per frame
    void Start()
    {
        if(!IsServer)return;
        
        Destroy(this.gameObject,time);
    }
    void FixedUpdate()
    {
        if(!IsServer)return;
         transform.position += transform.forward * Time.deltaTime * forward_speed;
             foreach (Collider zombie in Physics.OverlapSphere(transform.position, pullRadius))
         {
            if(zombie.CompareTag("Zombie"))
 
        
            if(zombie.GetComponent<rigidbody_enabler>().ai_script.Health.Value >=0)
            {
              

            zombie.GetComponent<rigidbody_enabler>().ai_script.TakeDamageZom(damage,attakerid.ToString(),false);
         
            }
            
            
            }
        }
         
            
            
            
          
    }

         
        
    
    

