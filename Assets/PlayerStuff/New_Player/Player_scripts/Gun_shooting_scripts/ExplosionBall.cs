using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ExplosionBall : NetworkBehaviour
{
    public float damage;
    public float explosion_Radius = 5f;
    public float explosion_force = 5f;
    bool has_hit = false;
    
    public ulong attackerid;
    public AudioClip [] Boom_sounds;
    public GameObject shake_sphere;

    
    public void Start()
    {
        if(!IsServer)return;
        

        
        Playboom_ClientRpc();
        explode();
        
    }
    [ClientRpc]
    public void Playboom_ClientRpc()
    {
        AudioClip clip = Boom_sounds[Random.Range(0, Boom_sounds.Length)];
        AudioSource.PlayClipAtPoint(clip,this.transform.position,200f);
    }
    
    
    
    

    public void explode()
   {
    
    if(!IsServer)return;
    if(!has_hit)
    {
    has_hit = true;
    Collider[] colliders = Physics.OverlapSphere(transform.position,explosion_Radius);
    foreach(Collider coll in colliders)
    {
        if(coll.GetComponent<zom_critical>() !=null)
    {
        coll.GetComponent<zom_critical>().CriticalDamageServerRpc(damage,attackerid.ToString(),true);
        

    }
    }
    has_hit = true;
    Destroy(this.gameObject,0.15f);
        
    }
    
   }
  
}
