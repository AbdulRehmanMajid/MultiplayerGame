using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Explosive_projectile : MonoBehaviour
{
    #region Public Fields
    public GameObject Radius_Sphere;
    public ulong attackerid;
    public float gravity = 9.81f;
    public bool is_rocket = false;
    
    public int damage;
    public float thrust_force = 100f;
    
    public float sway_force;
    public bool sway = false;
    
    public float multiplyer = 3;
    public float random_multiplyer = 15;
    public float random_amount_x;
    public float random_amount_y;
    public float random_amount_z;
    public float wait_time = 0.15f;
    
    public ulong client_owner;
    #endregion

    #region Private Fields
    Rigidbody rig_body;
    float initial_random_x;
    float initial_random_y;
    float initial_random_z;
    
    Explosive_manger explosion_man;
    #endregion

    #region Unity Methods
    void Start()
    {
        // Cache the explosion manager.
        explosion_man = GameObject.FindGameObjectWithTag("Explosive_manager").GetComponent<Explosive_manger>();
        
        // Generate initial random values for sway direction.
        initial_random_x = Random.Range(-random_amount_x, random_amount_x);
        initial_random_y = Random.Range(-random_amount_y, random_amount_y);
        initial_random_z = Random.Range(-random_amount_z, random_amount_z);

        rig_body = GetComponent<Rigidbody>();
        // Use continuous collision detection to improve collision reliability.
        rig_body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        if (is_rocket)
            Invoke(nameof(EnableSway), wait_time);

        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        if (!is_rocket)
        {
            rig_body.AddForce(Vector3.down * gravity * rig_body.mass);
        }
        else
        {
            // Rocket propulsion.
            rig_body.AddForce(transform.forward * thrust_force, ForceMode.Force);

            // Apply random sway if enabled.
            if (sway)
            {
                ApplySway();
            }
        }
    }
    #endregion

    #region Sway Methods
    void EnableSway()
    {
        sway = true;
    }

    void ApplySway()
    {
        // Cache current rotation.
        Vector3 currentAngles = transform.localEulerAngles;

        // Calculate random sway values (direction determined by initial random values).
        float swayX = Random.Range(0f, random_amount_x) * Time.deltaTime * random_multiplyer;
        float swayY = Random.Range(0f, random_amount_y) * Time.deltaTime * random_multiplyer;
        float swayZ = Random.Range(0f, random_amount_z) * Time.deltaTime * random_multiplyer;

        swayX = (initial_random_x < 0) ? -swayX : swayX;
        swayY = (initial_random_y < 0) ? -swayY : swayY;
        swayZ = (initial_random_z < 0) ? -swayZ : swayZ;

        Vector3 swayVector = new Vector3(swayX, swayY, swayZ);

        // Smoothly interpolate to new rotation with random sway.
        transform.localEulerAngles = Vector3.Lerp(currentAngles, currentAngles + swayVector, Time.deltaTime * multiplyer);
    }
    #endregion

    #region Collision & Explosion
    // Use OnCollisionEnter for standard collisions.
    void OnCollisionEnter(Collision collision)
    {
        TriggerExplosion();
    }

    // Also trigger explosion on trigger events, in case colliders are set as triggers.
    void OnTriggerEnter(Collider other)
    {
        TriggerExplosion();
    }

    // Explosion logic centralized in one method.
    void TriggerExplosion()
    {
        if (explosion_man)
        {
            explosion_man.SpawnBoom_ServerRpc(transform.position, attackerid, damage, is_rocket);
        }
        Destroy(gameObject);
    }
    #endregion

    #region Network RPC Methods
    [ServerRpc(RequireOwnership = false)]
    public void SpawnBoom_ServerRpc(Vector3 loc)
    {
        GameObject explosion = Instantiate(Radius_Sphere, loc, Radius_Sphere.transform.rotation);
        ExplosionBall explosionBall = explosion.GetComponent<ExplosionBall>();
        explosionBall.attackerid = attackerid;
        explosionBall.damage = 3000;
        explosion.GetComponent<NetworkObject>().Spawn();
    }
    #endregion
}
