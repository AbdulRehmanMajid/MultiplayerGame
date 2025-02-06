using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class attack_object_Script : MonoBehaviour
{
    public float damage;
    public float knockback;
    public bool enable = false;
    public Player_Health myhealthscript;

    
    
    private void OnTriggerEnter(Collider other)
    {
        
        if(enable)
        {
        Debug.LogError(other.name);
        if(other.GetComponent<zom_critical>() != null)
        {
            other.GetComponent<zom_critical>().CriticalDamageServerRpc(damage,myhealthscript.NetworkObjectId.ToString(),false);

        }
        if(other.GetComponent<zom_normal>() != null)
        {
                other.GetComponent<zom_normal>().normalDamageServerRpc(damage,myhealthscript.NetworkObjectId.ToString(),false);
                
        }
        if(other.GetComponent<Rigidbody>()!= null)
        {
             Vector3 test_force = new Vector3(knockback,knockback,knockback);
        other.GetComponent<Rigidbody>().AddForce(test_force,ForceMode.Impulse);
        }
        }
    }
   
}
