using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;




public class shoot_spawn : NetworkBehaviour
{
    public GameObject zombie;
    public float max_speed;
    public float current_least_Speed = 5f;
    
    public float speed_increase_amount=0.15f;
  public List<Transform> spawn_pos = new List<Transform>();
    public List<GameObject> zombies_active = new List<GameObject>();

    public int max_zombies = 24;
    public int zombies_to_spawn;
    public int increase_health_amount = 150;
    public int spawned_zombies;
    public bool is_running = false;
    float base_health = 0;
    int i;
    int fruit;
    int j;
    public bool debug_testing = false;
   
   
    Transform pos_to_spawn;
    [SerializeField] public NetworkVariable<int> Round_num = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
        [SerializeField] public NetworkVariable<int> Zombies_left = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);


    
    
    [ServerRpc(RequireOwnership = false)]
    public void spawn_zom_ServerRpc()
    {
        if(!is_running && !debug_testing)
        {
            StartCoroutine("spawn_code");
            
        }
        if(debug_testing)
        {
           Instantiate(zombie, spawn_pos[0].transform.position, Quaternion.identity);
        }
       
        
    }
   
    IEnumerator spawn_code()
    {
        Debug.LogError("Spawning zombies");
        is_running = true;
        while(true)
        {
            if(spawned_zombies < zombies_to_spawn)
            {
                 if(zombies_active.Count < max_zombies)
        {
        
         new_Spawn();
            
            
            
        }
            }
            if(spawned_zombies >= zombies_to_spawn && zombies_active.Count == 0)
            {
                Debug.LogError("RoundEnded");
                zombies_to_spawn+=5;
                spawned_zombies=0;
                Round_num.Value++;
                 yield return new WaitForSeconds(1.25f);
            }
            zombies_active.RemoveAll(x => !x);
            Zombies_left.Value = zombies_to_spawn - (spawned_zombies-zombies_active.Count);
            yield return new WaitForSeconds(0.1f);
        }
    }
    void new_Spawn()
    {
        Player_Health [] players = test.GetAllPlayers();
        foreach(Player_Health player in players)
        {
            get_distance(player);

        }
    }
     void get_distance(Player_Health player)
    {
        i=0;
        float low_distance = 999999f;
       
        foreach(Transform pos in spawn_pos)
        {
            float current_distance = Vector3.Distance(player.transform.position,pos.transform.position);
            if(current_distance < low_distance)
            {
                low_distance = current_distance;
                
                j =i;
                
            }
            
           
           
       i++;
        }
        

        fruit = j;
        code_backup(fruit);
      
        
    


    }
    
        
    

   

    void code_backup(int index)
    {
         GameObject zombie_enemy = Instantiate(zombie, spawn_pos[index].transform.position, Quaternion.identity);
            if(base_health == 0)
            {
                base_health =  zombie_enemy.GetComponent<rigidbody_enabler>().ai_script.max_health.Value;
            }
            zombie_enemy.GetComponent<rigidbody_enabler>().ai_script.max_health.Value = base_health + (increase_health_amount * Round_num.Value);
            if(current_least_Speed < max_speed)
            {
                
            zombie_enemy.GetComponent<rigidbody_enabler>().ai_script.zom_ai.speed = Random.Range(current_least_Speed,max_speed);
            }
            else
            {
                zombie_enemy.GetComponent<rigidbody_enabler>().ai_script.zom_ai.speed = Random.Range(max_speed,max_speed);
            }
            zombies_active.Add(zombie_enemy);
            zombie_enemy.GetComponent<NetworkObject>().Spawn();
            current_least_Speed = 5f + Round_num.Value*speed_increase_amount;
            spawned_zombies++;
    }
    
   
}
