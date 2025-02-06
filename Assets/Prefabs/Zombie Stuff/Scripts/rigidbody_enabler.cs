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
			
            stuff[i].GetComponent<Rigidbody>().isKinematic = true;
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
        
        for (int j = 0; j < colliders.Length; j++)
            {
                if(colliders[j].GetComponent<MeshCollider>() != null)
                {
                    colliders[j].GetComponent<MeshCollider>().enabled = false;
                }
                if(colliders[j].GetComponent<zom_critical>() != null)
                {
                    colliders[j].GetComponent<zom_critical>().enabled = false;
                }
                if(colliders[j].GetComponent<zom_normal>() != null)
                {
                    colliders[j].GetComponent<zom_normal>().enabled = false;
                }
                colliders[j].SetActive(false);

            }
        for (int i = 0; i < stuff.Length; i++)
            {
			stuff[i].GetComponent<Collider>().enabled = true;
            stuff[i].GetComponent<Rigidbody>().useGravity = true;
            stuff[i].GetComponent<Rigidbody>().isKinematic = false;
		}

		

    }
    
    
}
