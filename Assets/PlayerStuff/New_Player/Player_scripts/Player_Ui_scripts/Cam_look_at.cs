using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Cam_look_at : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public bool is_dead;
    private quaternion def_pos;
    public bool Aim_assist = true;
    public float Aim_assist_speed = 0.1f;
    public GameObject Aim_assist_target;
    public float assis_delay = 0.01f;
    public bool assist_type_2 = false;

    void Start()
    {
        def_pos = transform.localRotation;
        StartCoroutine(aim_assist());

    }
   

    // Update is called once per frame
    void Update()
    {
        if(is_dead)
        {
            transform.LookAt(player.transform.position);
        }
        if(assist_type_2)
        {
         
        Vector3 dir = Aim_assist_target.transform.position - transform.position;
       // dir.y = 0f;
        Quaternion lookrot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation,lookrot,Time.deltaTime * Aim_assist_speed);
   }
 
        
       
  
        
        
        
        
        
    }
    IEnumerator aim_assist()
    {
        while(true)
        {
             if(Aim_assist && Aim_assist_target != null)
        {
              transform.LookAt(Aim_assist_target.transform.position);
       
        }
        yield return new WaitForSeconds(assis_delay);
        }
    }
    public void Isalive()
    {
        transform.localRotation = def_pos;
    }
}
