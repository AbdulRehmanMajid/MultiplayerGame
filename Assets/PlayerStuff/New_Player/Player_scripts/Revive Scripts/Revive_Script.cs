using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Revive_Script : NetworkBehaviour
{
    public Transform my_collider;
    public Player_Health p_Health;
    public Revive_Script_player my_Rev_script;
    public float distance = 3f;



    void Update()
    {
        if(p_Health.Is_alive.Value == false)
        {
        Collider[] colliders = Physics.OverlapSphere(my_collider.transform.position,2.5f);
        foreach (var Other in colliders)
        {

            if(Other != my_collider)
        {
            
            if(Other.GetComponent<Revive_Script_player>() != my_Rev_script)
            {
            if(Other.GetComponent<Revive_Script_player>()!= null)
            {
                
                
            if(!p_Health.Is_alive.Value)
            {
           Revive_Script_player p_rev_script = Other.GetComponent<Revive_Script_player>();
           p_rev_script.revive_Script = this.GetComponent<Revive_Script>();
           Other.GetComponent<Revive_Script_player>().other_collider = my_collider;
            
            }
            
            }
            
            }
        }
        }
        }

        
    }


    
    }

