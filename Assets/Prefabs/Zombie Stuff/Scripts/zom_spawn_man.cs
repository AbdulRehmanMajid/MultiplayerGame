using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class zom_spawn_man : NetworkBehaviour
{
    public GameObject zombie;
    public bool do_spawn = true;
    public bool is_active = false;
    public float spawn_delay = 7f;
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(is_active)
    {
        if(!IsServer)return;
         StartCoroutine(spawn_zomb());
    }
    }
    void Update()
    {
    if(is_active)
    {
    if(!IsServer)return;
        if(do_spawn)
        {
            StartCoroutine(spawn_zomb());
            
        }

    }
    }

    IEnumerator spawn_zomb()
    {
        do_spawn = false;
        
        while(i != 1)
        {
            
            Debug.LogError("Spawning zombie");
           GameObject zombie_enemy = Instantiate(zombie, this.transform.position, Quaternion.identity);
            zombie_enemy.GetComponent<NetworkObject>().Spawn();
            yield return new WaitForSeconds (spawn_delay);
        }
       // yield return new WaitForSeconds (5f);
        //   Debug.LogError("Spawning zombie");
         //   GameObject zombie_enemy = Instantiate(zombie, this.transform.position, Quaternion.identity);
         //   zombie_enemy.GetComponent<NetworkObject>().Spawn();
    }
}
