using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class BlackHole : NetworkBehaviour
{
    public float pullRadius = 2f;
    public float damage = 30f;
    public ulong attakerid;
    public float forward_speed = 2f;
    public GameObject electic_arc;
    public float time = 100f;

    void Start()
    {
        if (!IsServer) return;
        Destroy(gameObject, time);
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        
        
        transform.position += transform.forward * Time.deltaTime * forward_speed;
        
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, pullRadius);
        foreach (Collider col in colliders)
        {
            if (!col.CompareTag("Zombie"))
                continue;

          
            rigidbody_enabler rbEnabler = col.GetComponent<rigidbody_enabler>();
            if (rbEnabler == null || rbEnabler.ai_script == null)
                continue;
            
            
            if (rbEnabler.ai_script.Health != null && rbEnabler.ai_script.Health.Value >= 0)
            {
                rbEnabler.ai_script.TakeDamageZom(damage, attakerid.ToString(), false);
            }
        }
    }
}






