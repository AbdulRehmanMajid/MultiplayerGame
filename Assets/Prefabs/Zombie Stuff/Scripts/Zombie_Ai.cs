using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.UI;
public class Zombie_Ai : NetworkBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatisGround, WhatisPlayer;
    public float SightRange, AttackRange;
    public bool playerInSightRange, playerInAttackRange;
    bool alreadyAttacked = false;
    public float timebetweenattackes;
    public Animator zom_anim;
    public AudioClip zom_attack;
    public AudioSource zom_audio_source;
    public float damage = 30f;
    [SerializeField] public NetworkVariable<float> Health = new NetworkVariable<float>(500,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public Slider hpbar;
    public float max_health = 500f;
    public GameObject zom_self;



    public override void OnNetworkSpawn()
    {
        hpbar.value = Health.Value / max_health;

         Player_Health [] players = test.GetAllPlayers(); 
         for(int i = 0; i < players.Length; i++)
         {
            player = players[i].transform;
         }
    }
    
    

    

    
    

    
    void Update()
    {
        if(!IsServer)return;
        if(player != null)
        {
        playerInSightRange = Physics.CheckSphere(transform.position,SightRange,WhatisPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position,AttackRange,WhatisPlayer);
        if(playerInSightRange && !playerInAttackRange && !alreadyAttacked)
        {
            ChasePlayer();
        }
        if(playerInSightRange && playerInAttackRange)
        {
            attackPlayerServerRpc();
        }
        }
        else
        {
            Player_Health [] players = test.GetAllPlayers();
        for(int i = 0; i < players.Length; i++)
        {
            player = players[i].transform;

        }
        }
        
    }
    void ChasePlayer()
    {
        agent.SetDestination(player.position);

    }
    [ServerRpc(RequireOwnership = false)]
    void attackPlayerServerRpc()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        if(!alreadyAttacked)
        {
            Debug.LogError("Enemy Attacking");
            zom_audio_source.PlayOneShot(zom_attack);
            zom_anim.SetTrigger("Attack");
            alreadyAttacked = true;
            StartCoroutine(do_attack());
            
            Invoke(nameof(ResetAttack),timebetweenattackes);

        }

    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    IEnumerator do_attack()
    {
        yield return new WaitForSeconds(0.35f);
       // player.GetComponent<Player_Health>().TakeDamageServerRpc(damage);
        if(Physics.CheckSphere(transform.position,AttackRange,WhatisPlayer))
            {
                player.GetComponent<Player_Health>().TakeDamage(damage);

            }

    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageZomServerRpc(float damage)
    {
        Health.Value -= damage;
        updateSliderServerRpc();
        if(Health.Value <= 0f)
        {
            Debug.LogError("Dead");
            zom_self.GetComponent<NetworkObject>().Despawn(true);
        }


    }
    [ClientRpc]
    public void updateSliderClientRpc()
    {
        hpbar.value = Health.Value / max_health;

    }
    [ServerRpc (RequireOwnership = false)]
    public void updateSliderServerRpc()
    {
        hpbar.value = Health.Value / max_health;
        updateSliderClientRpc();

    }
}
