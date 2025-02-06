using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class Worm_ai : NetworkBehaviour
{
    // Start is called before the first frame update
     public NetworkVariable<float> Health = new NetworkVariable<float>(150,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);   

    public NetworkVariable<float> max_health = new NetworkVariable<float>(150,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public bool dead = false;
     public GameObject Low_grade_Salvage;
      public GameObject High_grade_Salvage;

    public float salvage_Drop_chance;
    public float High_grade_salvage_Drop_chance;
    public float low_grade_salavge_drop_chance;
    public GameObject zom_self;
   public float path_update_delay = 0.2f;
    public Transform player;
     private float path_update_deadline;
     public NavMeshAgent zom_ai;
     public float lowest_distance = 500f;
     public float max_distance = 50f;
     public float wiggle_speed;

    // Update is called once per frame
    void FixedUpdate()
    {
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
   // this.transform.position = new Vector3(Mathf.Sin(Time.deltaTime*wiggle_speed),0,0);
    

     if(player != null && !dead)
     {
    
     Update_path();
     
     
     
    }
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
           
            StartCoroutine(despawn());
            
            //StartCoroutine(dissolve());
          //  attacker.SelfTycoonScript.Getmoney(90);
           // attacker.SelfTycoonScript.GetKills();
          //  attacker.SelfTycoonScript.Playdeathmarker_ClientRpc();
        }


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
            

        }
    }
    
    IEnumerator despawn()
    {
        zom_ai.enabled= false;
        yield return new WaitForSeconds(0.1f);
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
}
