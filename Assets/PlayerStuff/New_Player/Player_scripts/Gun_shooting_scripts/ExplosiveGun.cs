using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using StarterAssets;
using UnityEngine.VFX;
public class ExplosiveGun : NetworkBehaviour
{
    public Rifle_Data gun_data;
    public Rarity_and_level level_and_rarity;
    public Gun_stuff_holder Gun_stuff;
    [Header("Explosive amount Cam shake")]
        public GameObject Bullet_projectile;
   
    public float shoot_force;
    public bool Network_spawn = false;
    public float up_force;
    public bool is_rocket_launcher = false;
     public bool is_blackHole_gun = false;
    public LayerMask raycast_mask;
    
  public VisualEffect MuzzleFlash;
  public GameObject rocket;
 
    





    
  
    public float upwards_recoil = 0.5f;
    //Gun stats
    public bool shoot_bullet = false;
    
   
    
    public int base_damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsLeft, bulletsShot;
    
    public bool is_aiming;
    public AudioClip shootsound;
    

    //bools 
    public bool shooting, readyToShoot, reloading;

    //Reference
    
    public Transform attackPoint;
    public Transform attackPoint_hip;
    public Transform attackPoint_Ads;
    public RaycastHit rayHit;
   
   
    public AudioSource myaudiosource;
    public GameObject hitmark;
    
   
    public Transform raycast_point;
    
   

    //Graphics
  
   // public CamShake camShake;
   
  
    public float aim_spread;
 
    
   


    
   



  
    public Vector3 endPoint;
    public Vector3? penetrationPoint;
    public Vector3? impactPoint;
   
    private float default_spread;
  
   

    public TextMeshProUGUI text;
    public int rarity_lvl_3_magsize;
    public int rarity_lvl_4_damage;
    public int rarity_lvl_5_damage;

    public int def_damage;
    public int def_magsize;

    int old_rarity;
    

    
    
    private void Awake()
    {
        if(!IsOwner)return;
        bulletsLeft = magazineSize;
        readyToShoot = true;
        attackPoint = attackPoint_hip;
        base_damage = 500;
        magazineSize = gun_data.def_magsize;
         bulletsLeft = magazineSize;
        
       // raycast_point = fpsCam.transform;
        default_spread = spread;
    }
    private void LateUpdate()
    {
       if(!IsOwner)return;
       if(Gun_stuff.myhealthscript.Is_alive.Value)
       {
        MyInput();
       }
        text.SetText(bulletsLeft + " / " + magazineSize);
        if(old_rarity != level_and_rarity.Rareity_level.Value)
        {
            Check_Rarity();
            old_rarity = level_and_rarity.Rareity_level.Value;
        }
       
    }
         [ServerRpc(RequireOwnership =false)]
    public void kill_Gun_ServerRpc()
    {
         Destroy(this.gameObject);
        this.gameObject.GetComponent<NetworkObject>().Despawn();
        

    }
    
    private void MyInput()
    {
        if(!IsOwner)return;
        if (gun_data.allowButtonHold) shooting = Gun_stuff.new_inputs.shoot;
        else shooting =  Gun_stuff.new_inputs.shoot;

        if ( Gun_stuff.new_inputs.reload && bulletsLeft < magazineSize && !reloading) Reload();
       

        
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0){
            bulletsShot = gun_data.bulletsPerTap;
            
        
            
         float x = Random.Range(-gun_data.spread, gun_data.spread);
         float y = Random.Range(-gun_data.spread, gun_data.spread);
         Shoot(x,y);
         is_aiming = true;
      
         attackPoint = attackPoint_Ads;
       
        raycast_point = Gun_stuff.fpsCam.transform;
        
        
        
        
       
        
        
        }
    }
    [ServerRpc (RequireOwnership = false)]
    void playshootsound_ServerRpc()
    {
        playShootSound_ClientRpc();

    }
    [ClientRpc]
    void playShootSound_ClientRpc()
    {
        
       //     AudioSource.PlayClipAtPoint(shootsound,this.transform.position);
       if(!IsLocalPlayer)
       {
       myaudiosource.PlayOneShot(shootsound);
       }
       
            
        

        
    }
    private void Shoot(float spreadX,float SpreadY)
    {
       // new_particlesystem.Play();
        
        readyToShoot = false;
        
        myaudiosource.PlayOneShot(gun_data.shootsound_start);
       
      


        
        
        //Vector3 direction = raycast_point.transform.forward + new Vector3(spreadX, SpreadY, 0);
        
        
      // ShootServerRpc(spreadX,SpreadY);+
      //shootClientRpc(spreadX,SpreadY);
      //------------------------------------------------------------
    //  weap_recoil_script.Fire(); 
      Gun_stuff.cam_recoil.Fire();
      playshootsound_ServerRpc();
      if(MuzzleFlash != null)
        {
        MuzzleFlash.Play();
        }
      //GameObject go = Instantiate(bullet,attackPoint_Ads.transform.position,attackPoint_Ads.transform.rotation);
      //------------ -----------------------------------------------
      //if(gun_anim != null)
     // {
      //gun_anim.Play("fire");
     // }
     //------------------------------------------------------------
//       mymouselookscript.rotY += upwards_recoil;


       Gun_stuff.thrid_cam_script.mouseY += upwards_recoil;



       //------------------------------------------------------------
      shootnormal(spreadX,SpreadY);
      if(is_rocket_launcher)
      {
      rocket.SetActive(false);
      }
      
     
        
       //if (Physics.Raycast(raycast_point.transform.position, direction, out rayHit, range,whatIsEnemy))
       // {
        //  Debug.DrawLine(raycast_point.transform.position,direction * 50, Color.red);
          ///  Debug.Log(rayHit.collider.name);

           // if (rayHit.collider.CompareTag("enemy"))
           //     rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
              //  hitenable();
              //  playeraudio.PlayOneShot(hitmarkersound);
                //Invoke("hitdisable",0.5f);
      // }

        
//        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
      //  Instantiate(muzzleFlash,attackPoint.position,Quaternion.identity);
       

        bulletsLeft--;
        bulletsShot--;
        

        Invoke("ResetShot", gun_data.timeBetweenShooting);
        

        if(bulletsShot > 0 && bulletsLeft > 0)
        Invoke("shoot_burst", gun_data.timeBetweenShots);
    }
    
    
    public void shootnormal(float spreadX,float SpreadY)
    {
       
     
        if(!shoot_bullet)
        {
        Vector3 direction = raycast_point.transform.forward + new Vector3(spreadX, SpreadY, 0);
        
       Vector3 target_point;
       Ray ray = Gun_stuff.fpsCam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
         if (Physics.Raycast(ray, out rayHit,100,raycast_mask))
        {
            //impactPoint = rayHit.point;
            target_point = rayHit.point;
           
        
    
        
       // Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        //SpawnBoom_ServerRpc(rayHit.point);
        }
        else
        {
            target_point = ray.GetPoint(75);
        }
        Vector3 dir_without_spread = target_point - attackPoint_hip.transform.position;
        if(Network_spawn)
        {
        Spawn_ServerRpc(dir_without_spread);
        }
        else
        {
            Spawn_projectile(dir_without_spread);
        }
      
        

      
    }
   
    }
     [ServerRpc(RequireOwnership = false)]
    void Spawn_ServerRpc(Vector3 dir_without_spread)
    {
        if(!is_blackHole_gun)
        {
        GameObject projectile = Instantiate(Bullet_projectile,attackPoint_hip.transform.position, Bullet_projectile.transform.rotation);
        projectile.transform.forward = dir_without_spread.normalized;
       // projectile.transform.rotation = attackPoint_hip.transform.rotation;
        projectile.GetComponent<Explosive_projectile>().attackerid = Gun_stuff.myhealthscript.NetworkObjectId;
        projectile.GetComponent<Explosive_projectile>().damage = 3000;
         projectile.GetComponent<Explosive_projectile>().client_owner = Gun_stuff.myhealthscript.NetworkObjectId;
        projectile.GetComponent<NetworkObject>().Spawn();
      
      //  projectile.GetComponent<NetworkObject>().SpawnWithOwnership(Gun_stuff.myhealthscript.NetworkObjectId);
        
        if(!is_rocket_launcher)
        {
        projectile.GetComponent<Rigidbody>().AddForce(dir_without_spread.normalized * shoot_force,ForceMode.Impulse);
        projectile.GetComponent<Rigidbody>().AddForce(Gun_stuff.fpsCam.transform.up * up_force,ForceMode.Impulse);
        }
        }
        if(is_blackHole_gun)
        {
             GameObject projectile = Instantiate(Bullet_projectile,attackPoint_hip.transform.position, attackPoint_hip.transform.rotation);
              projectile.transform.forward = dir_without_spread.normalized;
              projectile.GetComponent<BlackHole>().attakerid = Gun_stuff.myhealthscript.NetworkObjectId;
               projectile.GetComponent<BlackHole>().damage = base_damage * level_and_rarity.Gun_level.Value;
                projectile.GetComponent<NetworkObject>().Spawn();
        }
        
        

    }
      
    void Spawn_projectile(Vector3 dir_without_spread)
    {
        
        GameObject projectile = Instantiate(Bullet_projectile,attackPoint_hip.transform.position, Bullet_projectile.transform.rotation);
        projectile.transform.forward = dir_without_spread.normalized;
       // projectile.transform.rotation = attackPoint_hip.transform.rotation;
        projectile.GetComponent<Explosive_projectile>().attackerid = Gun_stuff.myhealthscript.NetworkObjectId;
        projectile.GetComponent<Explosive_projectile>().damage = 3000;
        projectile.GetComponent<Explosive_projectile>().client_owner = Gun_stuff.myhealthscript.NetworkObjectId;
        //projectile.GetComponent<Rigidbody>().AddForce(dir_without_spread.normalized * shoot_force,ForceMode.Impulse);
      
      //  projectile.GetComponent<NetworkObject>().SpawnWithOwnership(Gun_stuff.myhealthscript.NetworkObjectId);
        
        if(!is_rocket_launcher)
        {
        projectile.GetComponent<Rigidbody>().AddForce(dir_without_spread.normalized * shoot_force,ForceMode.Impulse);
        projectile.GetComponent<Rigidbody>().AddForce(Gun_stuff.fpsCam.transform.up * up_force,ForceMode.Impulse);
        }
        

    }
    
    
  
    
    private void ResetShot()
    {
        readyToShoot = true;
        
    }
    private void Reload()
    {
        Gun_stuff.aim_transition.Reload();
        reloading = true;
       
        Invoke("ReloadFinished", gun_data.reloadTime);
    }
    private void ReloadFinished()
    {
       
        
        bulletsLeft = magazineSize;
        reloading = false;
        if(is_rocket_launcher)
      {
         rocket.SetActive(true);
      }
    }
     public void hitenable()
         {
        hitmark.SetActive(true);

         }
    public void hitdisable()
         {
       hitmark.SetActive(false);

         }
         
    public void Check_Rarity()
    {
        
       // base_damage = gun_data.def_damage;
        magazineSize = gun_data.def_magsize;
       
        
           

        if(level_and_rarity.Rareity_level.Value >=2)
        {
          
        }
        if(level_and_rarity.Rareity_level.Value == 3)
        {
            magazineSize = gun_data.rarity_lvl_3_magsize;
       
        }
        if(level_and_rarity.Rareity_level.Value == 4)
        {
           // base_damage = gun_data.rarity_lvl_4_damage;
             magazineSize = gun_data.rarity_lvl_3_magsize;
             

        }
       
        if(level_and_rarity.Rareity_level.Value >= 5)
        {
          //  base_damage = gun_data.rarity_lvl_5_damage;
            magazineSize = gun_data.rarity_lvl_3_magsize;
            
        }
       
        
    
        
           
        
       
    }
    
    
    
    
   
}
