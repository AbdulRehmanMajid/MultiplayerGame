using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
public class Zombie_ai_manager : NetworkBehaviour
{
    // Start is called before the first frame update
    public float update_time = 0.02f;
    void Start()
    {
        
         Debug.LogError("Starting");
        StartCoroutine(zombie_ai());
        
        
    }
   

    IEnumerator zombie_ai()
    {
        if(IsServer)
        {
        Debug.LogError("Running");
        while(true)
        {
            GameObject [] Zombies  = GameObject.FindGameObjectsWithTag("Zombie");
            Player_Health [] Players  = test.GetAllPlayers();
            foreach(GameObject Zombie in Zombies)
            {
                float lowest_distance = 5000f;
                Player_Health Target_player;
                foreach(Player_Health Player in Players)
                {
                    
                    float current_distance = Vector3.Distance(Zombie.transform.position,Player.transform.position);
                    if(current_distance < lowest_distance)
                    {
                        lowest_distance = current_distance;
                        Target_player = Player;
                    }
                    Zombie.GetComponent<NavMeshAgent>().SetDestination(Player.transform.position);

                }
                
                

            }
            yield return new WaitForSeconds(update_time);
        }
    }
    }
}
