using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zom_critical : MonoBehaviour
{
    public temp_zombie_ai zom_ai_script;
    public GameObject zombie;
    
    public void CriticalDamageServerRpc(float damage,string ownerid,bool explosive)
    {
        if(zom_ai_script.Health.Value > 0)
        {
        zom_ai_script.TakeDamageZomServerRpc(damage,ownerid,explosive);
        }

    }
}
