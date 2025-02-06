using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Explosive_projectile : MonoBehaviour
{
    public GameObject Radius_Sphere;
    public ulong attackerid;
    public float gravity = 9.81f;
    public bool is_rocket = false;
    
    public int damage;
    public float thrust_force = 100f;

    public float sway_force;
    public bool sway =false;
   Rigidbody rig_body;

   public float multiplyer = 3;
   public float random_multiplyer = 15;
   public float random_amount_x;
    public float random_amount_y;
     public float random_amount_z; 
     public float random_x;
    public float random_y;
    public float random_z;
     float initial_random_x;
    float initial_random_y;
    float initial_random_z;
     public float wait_time = 0.15f;
     public ulong client_owner;
     Explosive_manger explosion_man;
     

//calculate random number using x and y position

    

    // Update is called once per frame
    void Start()
    {
        

        
       // add_audioSource_ServerRpc();
       explosion_man = GameObject.FindGameObjectWithTag("Explosive_manager").GetComponent<Explosive_manger>();
       initial_random_x = Random.Range(-random_amount_x,random_amount_x);
        initial_random_y = Random.Range(-random_amount_y,random_amount_y);
        initial_random_z = Random.Range(-random_amount_z,random_amount_z);

       
       rig_body = this.gameObject.GetComponent<Rigidbody>();
       if(is_rocket)
       {
        Invoke("Sway",wait_time);

       }
        Destroy(this.gameObject,5f);
    
        
        
        
    }
    void FixedUpdate()
    {
        
        
        
        if(!is_rocket)
        {
        rig_body.AddForce(Vector3.down * gravity * this.gameObject.GetComponent<Rigidbody>().mass);
        }
        if(is_rocket)
        {
             rig_body.AddForce(this.transform.forward * thrust_force,ForceMode.Force);
             
                
             
        }
        if(is_rocket && sway)
        {
            Vector3 current_angles = transform.localEulerAngles;
            if(initial_random_x<0)
            {
                random_x =- Random.Range(random_x,random_amount_x) * Time.deltaTime * random_multiplyer;

            }
            if(initial_random_x>0)
            {
                random_x =+ Random.Range(random_x,random_amount_x) * Time.deltaTime * random_multiplyer;
            }
            if(initial_random_y<0)
            {
                random_y =- Random.Range(random_y,random_amount_y) * Time.deltaTime * random_multiplyer;

            }
            if(initial_random_y>0)
            {
                random_y =+ Random.Range(random_y,random_amount_y) * Time.deltaTime * random_multiplyer;
            }
            if(initial_random_z<0)
            {
                random_z =- Random.Range(random_z,random_amount_z) * Time.deltaTime * random_multiplyer;

            }
            if(initial_random_z>0)
            {
                random_z =+ Random.Range(random_z,random_amount_z) * Time.deltaTime * random_multiplyer;
            }
            
           
            transform.localEulerAngles = Vector3.Lerp(current_angles,transform.localEulerAngles + new Vector3(random_x,random_y,random_z),Time.deltaTime*multiplyer);

        }
        
        
    }

    void Sway()
    {
        sway = true;
    }
    
  
    void OnCollisionEnter(Collision collision)
    {
     if(damage!=0)
     {      
        
    // Debug.LogError(collision.gameObject.tag + "Name: " + collision.gameObject.name);
    if(collision.gameObject.CompareTag("Non_Explosive"))
      {
        if(explosion_man)
        {
        explosion_man.SpawnBoom_ServerRpc(this.transform.position,attackerid,damage,is_rocket);
        }
        Destroy(this.gameObject);
      }
     }
        
      
        
     
        
        
        
    }
    
    [ServerRpc(RequireOwnership =false)]
    public void SpawnBoom_ServerRpc(Vector3 loc)
    {
        
        GameObject explosion = Instantiate(Radius_Sphere,loc,Radius_Sphere.transform.rotation);
        explosion.GetComponent<ExplosionBall>().attackerid = attackerid;
         explosion.GetComponent<ExplosionBall>().damage = 3000;
         explosion.GetComponent<NetworkObject>().Spawn();
        
        
        

    }
}
