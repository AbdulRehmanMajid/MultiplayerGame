using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System;

public class temp_zombie_ai : NetworkBehaviour
{
    #region Fields & References
    // UI
    public float ease_health_speed = 0.05f;
    public GameObject collider_hp_bar;
    public Slider hpbar;
    public Slider easehealth_Bar;
    public Canvas hp_stuff;

    // Audio & Animation
    public AudioClip[] death_sounds;
    public AudioClip zom_attack;
    public AudioClip zombie_sprint;
    public AudioSource zom_audio_source;
    public Animator zom_anim;
    
    // Navigation & AI
    public NavMeshAgent zom_ai;
    public float lowest_distance = 500f;
    public Transform current_target;
    public LayerMask whatisGround, WhatisPlayer;
    public float SightRange, AttackRange;
    bool alreadyAttacked = false;
    public float timebetweenattackes;
    public Transform player;
    public float damage = 30f;
    public GameObject zom_self;
    public bool can_attack = true;
    public float max_distance = 50f;
    public float old_health;
    public float current_health;
    public float lerp_time = 0.2f;
    public bool show_hp_to_player = false;
    public rigidbody_enabler rig_enabler;

    // Health & Networking
    public NetworkVariable<float> Health = new NetworkVariable<float>(150, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> max_health = new NetworkVariable<float>(150, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Camera
    public look_at_cam cam_looker;
    public Transform P_cam;
    
    // Materials & Visuals
    public Material propblock;
    public SkinnedMeshRenderer skin_mesh_render;
    
    // Pathfinding
    public float path_update_delay = 0.2f;
    private float path_update_deadline;
    private float def_speed;
    
    // AI Status
    private bool dead = false;
    public float max_speed = 14f;
    public bool old_ai_enable = false;
    
    // Salvage Drop
    public GameObject Low_grade_Salvage;
    public GameObject High_grade_Salvage;
    public float salvage_Drop_chance;
    public float High_grade_salvage_Drop_chance;
    public float low_grade_salavge_drop_chance;
    #endregion

    #region Unity Methods
    void Start()
    {
        Health.Value = max_health.Value;
        hpbar.value = Health.Value / max_health.Value;
        old_health = Health.Value;
        def_speed = zom_ai.speed;

        if (zom_anim)
        {
            zom_anim.SetFloat("speed", zom_ai.speed);
        }
        
        Set_anim_speed_ClientRpc(zom_ai.speed);
        hp_stuff.enabled = false;
    }

    void FixedUpdate()
    {
        UpdateHealthUI();
        HandleHpCanvas();

        if (!old_ai_enable)
            return;

        if (!IsServer)
        {
            current_health = Health.Value;
            float targetValue = Health.Value / max_health.Value;
            hpbar.value = Mathf.Lerp(hpbar.value, targetValue, lerp_time * Time.deltaTime);
        }
        else
        {
            FindClosestPlayer();

            if (player != null)
            {
                UpdatePath();

                if (Vector3.Distance(transform.position, player.position) <= AttackRange &&
                    can_attack &&
                    player.GetComponent<Player_Health>().Is_alive.Value &&
                    !alreadyAttacked)
                {
                    can_attack = false;
                    Invoke(nameof(Attack), 0.45f);
                    Invoke(nameof(ResetAttack), 1.5f);
                    Debug.LogError("Attacking");
                }
            }
        }
    }
    #endregion

    #region Attack & Damage Methods
    private void ResetAttack()
    {
        alreadyAttacked = false;
        Attack_reset_ClientRpc();
        can_attack = true;
    }
    
    private void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) <= AttackRange + 0.25f)
        {
            Attack_ClientRpc();
            player.GetComponent<Player_Health>().TakeDamage(damage);
            alreadyAttacked = true;
        }
    }

    [ClientRpc]
    void Attack_ClientRpc()
    {
        if (zom_audio_source)
        {
            zom_audio_source.PlayOneShot(zom_attack);
        }
        if (zom_anim)
        {
            zom_anim.SetBool("Attack", true);
        }
    }

    [ClientRpc]
    void Attack_reset_ClientRpc()
    {
        if (zom_anim)
        {
            zom_anim.SetBool("Attack", false);
        }
    }

    [ClientRpc]
    void Run_ClientRpc()
    {
        if (zom_anim)
        {
            zom_anim.SetBool("run", true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageZomServerRpc(float damage, string ownerid, bool was_explosive)
    {
        ApplyDamage(damage, ownerid, was_explosive);
    }

    public void TakeDamageZom(float damage, string ownerid, bool was_explosive)
    {
        ApplyDamage(damage, ownerid, was_explosive);
    }

    private void ApplyDamage(float dmg, string ownerid, bool was_explosive)
    {
        Health.Value -= dmg;
        if (Health.Value <= 0f && !dead)
        {
            Debug.LogError("Dead");
            Player_Health attacker = test.GetPlayer("Player" + ownerid);
            attacker?.scorefeed_ClientRpc(ownerid, was_explosive);
            if (attacker != null)
            {
                attacker.money_man.Money.Value += 90;
            }
            dead = true;
            if (rig_enabler)
            {
                rig_enabler.disablestuff_ServerRpc();
                enablerig_ClientRpc();
            }
            StartCoroutine(Despawn());
        }
    }
    #endregion

    #region Client RPC Methods
    [ClientRpc]
    void Set_anim_speed_ClientRpc(float speed)
    {
        if (zom_anim)
        {
            zom_anim.SetFloat("speed", speed);
        }
    }

    [ClientRpc]
    void enablerig_ClientRpc()
    {
        int num = UnityEngine.Random.Range(0, death_sounds.Length);
        if (zom_audio_source)
        {
            zom_audio_source.PlayOneShot(death_sounds[num]);
        }
        StartCoroutine(Dissolve());
        rig_enabler.enable_stuff();
        collider_hp_bar.SetActive(false);
    }

    [ClientRpc]
    public void updateSliderClientRpc()
    {
        hpbar.value = Health.Value / max_health.Value;
    }
    #endregion

    #region Server RPC Methods
    [ServerRpc(RequireOwnership = false)]
    public void updateSliderServerRpc()
    {
        hpbar.value = Health.Value / max_health.Value;
        updateSliderClientRpc();
    }
    #endregion

    #region Helper Methods
    void FindClosestPlayer()
    {
        Player_Health[] players = test.GetAllPlayers();
        lowest_distance = 500f;
        foreach (Player_Health p in players)
        {
            if (p != null && p.Is_alive.Value)
            {
                float dist = Vector3.Distance(transform.position, p.transform.position);
                if (dist < lowest_distance)
                {
                    lowest_distance = dist;
                    player = p.transform;
                }
            }
        }
    }

    // Add a new job struct to calculate a random offset
    public struct PathOffsetJob : IJob
    {
        public float offsetRadius;
        public uint seed;
        public NativeArray<float3> offset;

        public void Execute()
        {
            Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);
            float3 randomOffset = random.NextFloat3Direction() * random.NextFloat(0, offsetRadius);
            offset[0] = randomOffset;
        }
    }

    void UpdatePath()
    {
        if (Time.time >= path_update_deadline)
        {
            path_update_deadline = Time.time + path_update_delay;

            // Schedule a job to compute a random offset
            NativeArray<float3> offsetArray = new NativeArray<float3>(1, Allocator.TempJob);
            PathOffsetJob offsetJob = new PathOffsetJob
            {
                offsetRadius = 3f, // Adjust the radius accordingly
                seed = (uint)UnityEngine.Random.Range(1, 10000),
                offset = offsetArray
            };

            JobHandle jobHandle = offsetJob.Schedule();
            jobHandle.Complete();  // Wait for the job to finish
            float3 offset = offsetArray[0];
            offsetArray.Dispose();

            // Apply the offset on the XZ plane to the player's position
            Vector3 destination = player.position + new Vector3(offset.x, 0f, offset.z);
            zom_ai.SetDestination(destination);

            if (Vector3.Distance(transform.position, player.position) > max_distance ||
                !player.GetComponent<Player_Health>().Is_alive.Value)
            {
                lowest_distance = 5000f;
                Debug.LogError("finding new target");
            }
            Run_ClientRpc();
        }
    }

    void UpdateHealthUI()
    {
        current_health = Health.Value;
        float targetValue = Health.Value / max_health.Value;
        hpbar.value = Mathf.Lerp(hpbar.value, targetValue, lerp_time * Time.deltaTime);
        if (Mathf.Abs(easehealth_Bar.value - hpbar.value) > 0.001f)
        {
            easehealth_Bar.value = Mathf.Lerp(easehealth_Bar.value, hpbar.value, ease_health_speed);
        }
    }

    void HandleHpCanvas()
    {
        if (show_hp_to_player)
        {
            hp_stuff.enabled = true;
            if (cam_looker.p_cam == null)
            {
                cam_looker.p_cam = P_cam;
            }
        }
        else
        {
            hp_stuff.enabled = false;
        }
        // Reset flag each frame.
        show_hp_to_player = false;
    }
    #endregion

    #region Coroutines
    IEnumerator Dissolve()
    {
        yield return new WaitForSeconds(0.25f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;
            if (skin_mesh_render)
            {
                skin_mesh_render.material.SetFloat("_dissolve", t);
            }
            yield return null;
        }
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(0.15f);
        float chance = UnityEngine.Random.Range(0, 100);
        if (chance >= salvage_Drop_chance)
        {
            float chance2 = UnityEngine.Random.Range(0, 100);
            GameObject salvageObj = (chance2 >= High_grade_salvage_Drop_chance) ?
                Instantiate(High_grade_Salvage, transform.position, Quaternion.identity) :
                Instantiate(Low_grade_Salvage, transform.position, Quaternion.identity);

            var netObj = salvageObj.GetComponent<NetworkObject>();
            netObj.Spawn();
            Rigidbody rb = salvageObj.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
            rb.AddForce(salvageObj.transform.up * 4f, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(1.20f);
        zom_self.GetComponent<NetworkObject>().Despawn(true);
    }
    #endregion
}
