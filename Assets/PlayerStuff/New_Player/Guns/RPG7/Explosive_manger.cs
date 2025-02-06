using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Explosive_manger : NetworkBehaviour
{
    
    public GameObject Radius_Sphere_gernade;
    public GameObject Radius_Sphere_rocket;
    [ServerRpc(RequireOwnership =false)]
    public void SpawnBoom_ServerRpc(Vector3 loc,ulong id,float damage_amount,bool is_rocket)
    {
        if(is_rocket)
        {
        GameObject explosion = Instantiate(Radius_Sphere_rocket,loc,Radius_Sphere_rocket.transform.rotation);
        explosion.GetComponent<ExplosionBall>().attackerid = id;
         explosion.GetComponent<ExplosionBall>().damage = damage_amount;
         explosion.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
        GameObject explosion = Instantiate(Radius_Sphere_gernade,loc,Radius_Sphere_gernade.transform.rotation);
        explosion.GetComponent<ExplosionBall>().attackerid = id;
         explosion.GetComponent<ExplosionBall>().damage = damage_amount;
         explosion.GetComponent<NetworkObject>().Spawn();

        }
        
        
        
        

    }
}
