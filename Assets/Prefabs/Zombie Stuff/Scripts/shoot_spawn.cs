using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class shoot_spawn : NetworkBehaviour
{
    public GameObject zombie;
    public float max_speed;
    public float current_least_Speed = 5f;
    public float speed_increase_amount = 0.15f;
    public List<Transform> spawn_pos = new List<Transform>();
    public List<GameObject> zombies_active = new List<GameObject>();

    public int max_zombies = 24;
    public int zombies_to_spawn;
    public int increase_health_amount = 150;
    public int spawned_zombies;
    public bool is_running = false;
    float base_health = 0;
    public bool debug_testing = false;
    
    [SerializeField] public NetworkVariable<int> Round_num = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<int> Zombies_left = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [ServerRpc(RequireOwnership = false)]
    public void spawn_zom_ServerRpc()
    {
        if (!is_running && !debug_testing)
        {
            StartCoroutine(spawn_code());
        }
        if (debug_testing)
        {
            Instantiate(zombie, spawn_pos[0].position, Quaternion.identity);
        }
    }
   
    IEnumerator spawn_code()
    {
        Debug.LogError("Spawning zombies");
        is_running = true;
        while (true)
        {
            if (spawned_zombies < zombies_to_spawn)
            {
                if (zombies_active.Count < max_zombies)
                {
                    new_Spawn();
                }
            }
            if (spawned_zombies >= zombies_to_spawn && zombies_active.Count == 0)
            {
                Debug.LogError("Round Ended");
                zombies_to_spawn += 5;
                spawned_zombies = 0;
                Round_num.Value++;
                yield return new WaitForSeconds(1.25f);
            }
            zombies_active.RemoveAll(item => item == null);
            Zombies_left.Value = zombies_to_spawn - (spawned_zombies - zombies_active.Count);
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void new_Spawn()
    {
        // Get all players (assumed to be provided by your test class)
        Player_Health[] players = test.GetAllPlayers();
        foreach (Player_Health player in players)
        {
            int spawnIndex = GetNearestSpawnIndex(player);
            SpawnZombieAtIndex(spawnIndex);
        }
    }
    
    int GetNearestSpawnIndex(Player_Health player)
    {
        float lowestDistance = float.MaxValue;
        int bestIndex = 0;
        for (int index = 0; index < spawn_pos.Count; index++)
        {
            float distance = Vector3.Distance(player.transform.position, spawn_pos[index].position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                bestIndex = index;
            }
        }
        return bestIndex;
    }

    void SpawnZombieAtIndex(int index)
    {
        GameObject zombieEnemy = Instantiate(zombie, spawn_pos[index].position, Quaternion.identity);
        var aiScript = zombieEnemy.GetComponent<rigidbody_enabler>().ai_script;
        
        // Set base health on first spawn
        if (base_health == 0)
        {
            base_health = aiScript.max_health.Value;
        }
        aiScript.max_health.Value = base_health + (increase_health_amount * Round_num.Value);
        
        // Set zombie speed
        if (current_least_Speed < max_speed)
        {
            aiScript.zom_ai.speed = Random.Range(current_least_Speed, max_speed);
        }
        else
        {
            aiScript.zom_ai.speed = max_speed;
        }
        
        zombies_active.Add(zombieEnemy);
        zombieEnemy.GetComponent<NetworkObject>().Spawn();

        // Increase the base speed for later rounds
        current_least_Speed = 5f + Round_num.Value * speed_increase_amount;
        spawned_zombies++;
    }
}
