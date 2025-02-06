using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using UnityEngine.UI;






public class temp_zombie_ai : NetworkBehaviour
{
    public GameObject collider_hp_bar;
    public AudioClip[] death_sounds;
    public NavMeshAgent zom_ai;
    public float lowest_distance = 500f;
    public Transform current_target;
    public LayerMask whatisGround, WhatisPlayer;
    public float SightRange, AttackRange;
    public bool playerInSightRange, playerInAttackRange;
    bool alreadyAttacked = false;
    public float timebetweenattackes;
    public Animator zom_anim;
    public AudioClip zom_attack;
    public AudioSource zom_audio_source;
    public Transform player;
    public float damage = 30f;
    public GameObject zom_self;
    public Slider hpbar;
    public Slider easehealth_Bar;
    public float ease_health_speed = 0.05f;
    public Canvas hp_stuff;

    
    
    public NetworkVariable<float> Health = new NetworkVariable<float>(150,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);   

    public NetworkVariable<float> max_health = new NetworkVariable<float>(150,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    public bool can_attack = true;
    public float max_distance = 50f;
    public float old_health;
    public float current_health;
    public float lerp_time = 0.2f;
    public bool show_hp_to_player = false;
    public rigidbody_enabler rig_enabler;


    public look_at_cam cam_looker;
    public Transform P_cam;
    
    public Material propblock;
    public SkinnedMeshRenderer skin_mesh_render;
    public float path_update_delay = 0.2f;
    private float path_update_deadline;

    
    private float def_speed;
    private bool dead = false;
    public float max_speed = 14f;
    public AudioClip zombie_sprint;
    public bool old_ai_enable = false;
    public GameObject Low_grade_Salvage;
      public GameObject High_grade_Salvage;

    public float salvage_Drop_chance;
    public float High_grade_salvage_Drop_chance;
    public float low_grade_salavge_drop_chance;
    

    // Update is called once per frame
    void Start()
    {
        Health.Value = max_health.Value;
        hpbar.value = Health.Value / max_health.Value;
        old_health = Health.Value;
        def_speed = zom_ai.speed;
        
         if(zom_anim)
        {
        zom_anim.SetFloat("speed",zom_ai.speed);
        }
        Set_anim_speed_ClientRpc(zom_ai.speed);
        hp_stuff.enabled = false;
       
    
        
       
        // propblock.SetFloat("_dissolve",0);
        // propblock = skin_mesh_render.materials[0];
    }
    [ClientRpc]
    void Set_anim_speed_ClientRpc(float speed)
    {
         if(zom_anim)
        {
        zom_anim.SetFloat("speed",speed);
        }
    }
    
    
    
    void FixedUpdate()
    {
        update_health_new();
        if(show_hp_to_player)
            {
                hp_stuff.enabled = true;
                if(cam_looker.p_cam == null)
                {
                    cam_looker.p_cam = P_cam;
                }
        
            }
            else
            {
                hp_stuff.enabled = false;
            }
            show_hp_to_player = false;
        if(old_ai_enable)
        {
        if(!IsServer)
        {
        
        
            current_health = Health.Value;
            float value_to_lerp_to = Health.Value / max_health.Value;
            hpbar.value = Mathf.Lerp(hpbar.value,value_to_lerp_to,lerp_time * Time.deltaTime);
            
            
        }
 
        if(IsServer)
        {
        
        Player_Health[] players = test.GetAllPlayers();
        

    foreach(Player_Health Player in players)
    {
        
            //Debug.LogError(Vector3.Distance(this.transform.position,Player.transform.position));
            if(Player != null)
            {
                
            
            float current_distance = Vector3.Distance(this.transform.position,Player.transform.position);
            
            if(current_distance < lowest_distance)
            {
                if(Player.GetComponent<Player_Health>().Is_alive.Value == true)
                {
                lowest_distance = current_distance;
                player = Player.transform;
                }

            }
            }
             
            
    }
    

     if(player != null)
     {
    
     Update_path();
     
     
     
    
    
     
     if(Vector3.Distance(this.transform.position,player.transform.position) <= AttackRange && can_attack && player.GetComponent<Player_Health>().Is_alive.Value == true && !alreadyAttacked)
     {

        //do_attack();
        can_attack = false;
       // zom_ai.SetDestination(transform.position);
        //transform.LookAt(player.transform);
        
        //Attack_ClientRpc();
        
        //AttackClientRpc();
        
        
        Invoke("attack",0.45f);

        Invoke("ResetAttack",1.5f);
        
       // player.GetComponent<Player_Health>().Health.Value -= damage;
        Debug.LogError("Attacking");

     }
     }
        }
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
        Attack_reset_ClientRpc();
        can_attack = true;
    }
    private void attack()
    {
        if(Vector3.Distance(this.transform.position,player.transform.position) <= AttackRange + 0.25f)
        {
        Attack_ClientRpc();
        player.GetComponent<Player_Health>().TakeDamage(damage);
        alreadyAttacked = true;
        }
        

    }
    [ClientRpc]
    void Attack_ClientRpc()
    {
        if(zom_audio_source)
        {
        zom_audio_source.PlayOneShot(zom_attack);
        }
        if(zom_anim)
        {
        zom_anim.SetBool("Attack",true);
        }
    }
    [ClientRpc]
    void Attack_reset_ClientRpc()
    {
       // zom_audio_source.PlayOneShot(zom_attack);
        if(zom_anim)
        {
        zom_anim.SetBool("Attack",false);
        }
    }
    [ClientRpc]
    void Run_ClientRpc()
    {
       // zom_audio_source.PlayOneShot(zom_attack);
        if(zom_anim)
        {
        zom_anim.SetBool("run",true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageZomServerRpc(float damage,string ownerid,bool was_explosive)
    {
        Health.Value -= damage;
      
        
       
        if(Health.Value <= 0f)
        {
            Debug.LogError("Dead");
           
            //zom_self.GetComponent<NetworkObject>().Despawn(true);
            Player_Health attacker = test.GetPlayer("Player"+ownerid);
        
            if(Health.Value <=0f && !dead)
            {
                attacker.scorefeed_ClientRpc(ownerid,was_explosive);
                attacker.money_man.Money.Value += 90;
            }
             dead = true;
            if(rig_enabler)
            {
            rig_enabler.disablestuff_ServerRpc();
            enablerig_ClientRpc();
            }
            StartCoroutine(despawn());
            
            //StartCoroutine(dissolve());
          //  attacker.SelfTycoonScript.Getmoney(90);
           // attacker.SelfTycoonScript.GetKills();
          //  attacker.SelfTycoonScript.Playdeathmarker_ClientRpc();
        }


    }
    public void TakeDamageZom(float damage,string ownerid,bool was_explosive)
    {
        Health.Value -= damage;
        
        
       
        if(Health.Value <= 0f)
        {
            Debug.LogError("Dead");
           
            //zom_self.GetComponent<NetworkObject>().Despawn(true);
            Player_Health attacker = test.GetPlayer("Player"+ownerid);
        
            if(Health.Value <=0f && !dead)
            {
                attacker.scorefeed_ClientRpc(ownerid,was_explosive);
                attacker.money_man.Money.Value += 90;
                
            }
             dead = true;
              if(rig_enabler)
            {
            rig_enabler.disablestuff_ServerRpc();
            enablerig_ClientRpc();
            }
            StartCoroutine(despawn());
            
            //StartCoroutine(dissolve());
          //  attacker.SelfTycoonScript.Getmoney(90);
           // attacker.SelfTycoonScript.GetKills();
          //  attacker.SelfTycoonScript.Playdeathmarker_ClientRpc();
        }


    }
    [ClientRpc]
    void enablerig_ClientRpc()
    {
        int num = Random.Range(0,death_sounds.Length);
        AudioClip death_sound =death_sounds[num];
         if(zom_audio_source)
        {
        zom_audio_source.PlayOneShot(death_sound);
        }
        StartCoroutine(dissolve());
        rig_enabler.enable_stuff();
        collider_hp_bar.SetActive(false);
        

    }
    [ClientRpc]
    public void updateSliderClientRpc()
    {
        hpbar.value = Health.Value / max_health.Value;

    }
    [ServerRpc (RequireOwnership = false)]
    public void updateSliderServerRpc()
    {
        hpbar.value = Health.Value / max_health.Value;
        updateSliderClientRpc();

    }
    IEnumerator dissolve()
    {
        yield return new WaitForSeconds(0.25f);
        float t = 0;
        while(t< 1f)
        {
            t += Time.deltaTime /2f;
            if(skin_mesh_render)
            {
            skin_mesh_render.material.SetFloat("_dissolve",t);

            }
          //  propblock.SetFloat("_dissolve",t);
            yield return null;
            

        }
        
        
        
    }
    IEnumerator despawn()
    {
        yield return new WaitForSeconds(0.15f);
        float chance = Random.Range(0,100);
        

        
        if(chance >= salvage_Drop_chance)
        {
            float chance_2 = Random.Range(0,100);
        if(chance_2 >= High_grade_salvage_Drop_chance)
        {
        GameObject go = Instantiate(High_grade_Salvage,this.transform.position,Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<Rigidbody>().AddForce(this.transform.forward * 5f,ForceMode.Impulse);
        go.GetComponent<Rigidbody>().AddForce(go.transform.up * 4f,ForceMode.Impulse);

        }
        else
        {
            GameObject go = Instantiate(Low_grade_Salvage,this.transform.position,Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<Rigidbody>().AddForce(this.transform.forward * 5f,ForceMode.Impulse);
        go.GetComponent<Rigidbody>().AddForce(go.transform.up * 4f,ForceMode.Impulse);

        }
        
        }
        yield return new WaitForSeconds(1.20f);
        zom_self.GetComponent<NetworkObject>().Despawn(true);

    }
  

    void Update_path()
    {
        if(Time.time >= path_update_deadline)
        {
           // Debug.LogError("updating path");
            path_update_deadline = Time.time + path_update_delay;
            
            
            
                zom_ai.SetDestination(player.transform.position);
                 if(Vector3.Distance(this.transform.position,player.transform.position)>max_distance)
    {
        lowest_distance = 5000f;
        Debug.LogError("finding new target");
    }
    if(player.GetComponent<Player_Health>().Is_alive.Value == false)
    {
        lowest_distance = 5000f;
        Debug.LogError("finding new target cause player dead lol");
    }

            

            
            //transform.LookAt(player.transform);
            Run_ClientRpc();

        }
    }
    
 

    void update_health_new()
    {
        current_health = Health.Value;
        float value_to_lerp_to = Health.Value / max_health.Value;
        hpbar.value = Mathf.Lerp(hpbar.value,value_to_lerp_to,lerp_time * Time.deltaTime);

        if(easehealth_Bar.value != hpbar.value)
        {
            easehealth_Bar.value = Mathf.Lerp(easehealth_Bar.value,hpbar.value,ease_health_speed);
        }
    }
   
   
    
}
