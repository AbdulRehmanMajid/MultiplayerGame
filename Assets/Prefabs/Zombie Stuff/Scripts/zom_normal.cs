using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zom_normal : MonoBehaviour
{
    public temp_zombie_ai zom_ai_script;
    public GameObject zombie;
    public void normalDamageServerRpc(float damage,string ownerid,bool was_explosive)
    {
        if(zom_ai_script.Health.Value > 0 && !was_explosive)
        {
        zom_ai_script.TakeDamageZomServerRpc(damage,ownerid,was_explosive);
        }
        if(zom_ai_script.Health.Value > 0 && was_explosive)
        {
             zom_ai_script.TakeDamageZom(damage,ownerid,was_explosive);

        }
        

    }
}
