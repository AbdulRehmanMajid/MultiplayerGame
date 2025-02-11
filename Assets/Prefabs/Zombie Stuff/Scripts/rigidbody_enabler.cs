using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class rigidbody_enabler : MonoBehaviour
{
    public bool enable = false;
    public GameObject[] stuff;
    public GameObject[] colliders;
    public Animator animator;
    public temp_zombie_ai ai_script;
    public NavMeshAgent ai_agent;
    public LayerMask lol_layer;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < stuff.Length; i++)
        {
            Rigidbody rb = stuff[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void disablestuff_ServerRpc()
    {
        animator.enabled = false;
        ai_script.enabled = false;
        ai_agent.enabled = false;
    }

    [ClientRpc]
    public void enable_stuff()
    {
        animator.enabled = false;
        ai_script.enabled = false;
        ai_agent.enabled = false;
        
        // Disable specific components on colliders and then disable the collider GameObject.
        for (int j = 0; j < colliders.Length; j++)
        {
            MeshCollider mc = colliders[j].GetComponent<MeshCollider>();
            if (mc != null)
                mc.enabled = false;

            zom_critical critical = colliders[j].GetComponent<zom_critical>();
            if (critical != null)
                critical.enabled = false;

            zom_normal normal = colliders[j].GetComponent<zom_normal>();
            if (normal != null)
                normal.enabled = false;

            colliders[j].SetActive(false);
        }

        // Enable colliders and physics on each object in "stuff".
        for (int i = 0; i < stuff.Length; i++)
        {
            Collider col = stuff[i].GetComponent<Collider>();
            if (col != null)
                col.enabled = true;

            Rigidbody rb = stuff[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
    }
}
