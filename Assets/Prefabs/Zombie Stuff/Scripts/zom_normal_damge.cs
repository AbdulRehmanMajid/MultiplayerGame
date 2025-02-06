using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zom_normal_damge : MonoBehaviour
{
    public temp_zombie_ai zom_ai_script;
    
    public void CriticalDamageServerRpc(float damage,string ownerid)
    {
        zom_ai_script.TakeDamageZomServerRpc(damage,ownerid,false);

    }
}
