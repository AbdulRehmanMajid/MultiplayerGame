using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using StarterAssets;
using UnityEngine.UI;
using UnityEngine.VFX;




public class temp_gun_script : NetworkBehaviour
{
    
    public Rifle_Data gun_data;
    public HitMarker_Sounds hitmarker_data;
    public Rarity_and_level level_and_rarity;
    public GameObject Damage_popup;
    public VisualEffect MuzzleFlash;
   public Gun_stuff_holder Gun_stuff;
   
  
   
   
   public TrailRenderer Bullettrail;
   public LineRenderer Laser;
   
    
    
    
    public bool is_aiming;
   

    //bools 
    public bool shooting, readyToShoot =true, reloading;

    //Reference
    
    Transform attackPoint;
    public Transform attackPoint_hip;
    public Transform attackPoint_Ads;

    
    public RaycastHit rayHit;
    
    
    public AudioSource myaudiosource;
    public GameObject hitmark;
    
   
    public Transform raycast_point;
   
   

    //Graphics
    public GameObject bulletHoleGraphic;
   // public CamShake camShake;
   
  
    
    public bool laser_active = true;
    public float laser_range = 40f;
    
    int bulletsLeft;
    int bulletsShot;
   

    
    
   



    public float penetrationAmount;
     Vector3 endPoint;
     Vector3? penetrationPoint;
    Vector3? impactPoint;
    public RaycastHit penlolHit;
    bool has_hit_zombie =false;
    
 

    public TextMeshProUGUI text;
   

    

    int magazineSize;

    int old_rarity;
    int base_damage;

    
    
    bool is_mobile =false;
    bool is_invoked = false;
    int bullets_shot_in_one_sec=0;
    int bullets_shot =0;

    public bool continous_fire = false;
    
    public float elaspsed_time =0;
         
bool set_server_continue_fire = false;

    
    
    

    
    
    private void Awake()
    {
        if(!IsOwner)return;
        

        if(Application.platform == RuntimePlatform.Android)
        {
            is_mobile = true;
        }
        else
        {
            is_mobile = false;
        }
         
        laser_active = false;
        base_damage = gun_data.def_damage;
        magazineSize = gun_data.def_magsize;
         bulletsLeft = magazineSize;
        readyToShoot = true;
        attackPoint = attackPoint_hip;

        raycast_point = Gun_stuff.fpsCam.transform;
         
      
        
        
    }
    void Start()
    {
        if(!IsOwner)return;
        
    }
    private void LateUpdate()
    {
        if(laser_active && Laser != null)
        {
            Laser.SetPosition(0,attackPoint_hip.transform.position);
            Laser.SetPosition(1,attackPoint_hip.transform.position + attackPoint_hip.transform.forward * laser_range);
        }
        
        if(!IsOwner)return;
        if(level_and_rarity.Rareity_level.Value != old_rarity)
        {
            Check_Rarity();
            old_rarity = level_and_rarity.Rareity_level.Value;
        }
        
        if(Gun_stuff.myhealthscript.Is_alive.Value)
        {
        MyInput();
        
        if(!gun_data.is_shotgun)
        {
        text.SetText(bulletsLeft + " / " + magazineSize);
        }
        if(gun_data.is_shotgun)
        {
            text.SetText((bulletsLeft/gun_data.bulletsPerTap) + " / " + (magazineSize/gun_data.bulletsPerTap));
        }
        }
        if(!gun_data.use_old_sound_system)
        {
        continue_shoot();
        
        if(shooting && !reloading)
        {
            elaspsed_time += Time.deltaTime;
        
        }
        else
        {
            elaspsed_time -=Time.deltaTime;
            if(elaspsed_time < 0)
            {
                elaspsed_time = 0;
            }
        }
        }
       
    }
    
         [ServerRpc(RequireOwnership =false)]
    public void kill_Gun_ServerRpc()
    {
        Destroy(this.gameObject);
        this.gameObject.GetComponent<NetworkObject>().Despawn();
        

    }
    public void gun_rpm_calc()
    {
        int current_rpm = bullets_shot_in_one_sec * 60;
        if(bullets_shot_in_one_sec > 0)
        {
        Debug.LogError("Current Rpm is:" + current_rpm);
        }
    }
    IEnumerator rpm()
    {
        if(!is_invoked)
        {
            is_invoked = true;
            bullets_shot_in_one_sec = 0;
        yield return new WaitForSeconds(1f);
        bullets_shot_in_one_sec = bullets_shot;
        bullets_shot = 0;
        is_invoked = false;
        }
    }
    private void MyInput()
    {
        if(bulletsLeft == 0 && !reloading)
        {
            Reload();
        }
        if (gun_data.allowButtonHold) shooting = Gun_stuff.new_inputs.shoot;
        else shooting = Gun_stuff.new_inputs.shoot;

        if (Gun_stuff.new_inputs.reload && bulletsLeft < magazineSize && !reloading) Reload();
       

        
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0){
            bulletsShot = gun_data.bulletsPerTap;
           
          
        
            if(gun_data.is_sniper)
            {
                Gun_stuff.aim_transition.is_sniper = true;
            }
            else
            {
                Gun_stuff.aim_transition.is_sniper = false;
            }
         float x = Random.Range(-gun_data.spread, gun_data.spread);
         float y = Random.Range(-gun_data.spread, gun_data.spread);
         Shoot(x,y);
         is_aiming = true;
      
         attackPoint = attackPoint_Ads;
        
        raycast_point = Gun_stuff.fpsCam.transform;
        
        
        
        
        if(Input.GetMouseButtonDown(0) && gun_data.is_shotgun)
        {
            myaudiosource.PlayOneShot(gun_data.shootsound_start);

        }
        
        
        }
    }
    [ServerRpc (RequireOwnership = false)]
    void playshootsound_ServerRpc(bool continues)
    {
        playShootSound_ClientRpc(continues);

    }
    [ClientRpc]
    void playShootSound_ClientRpc(bool continous)
    {
        if(!IsLocalPlayer)
        {
            if(continous)
            {
            myaudiosource.clip =gun_data.shootsound_continue;
            myaudiosource.loop = true;
            myaudiosource.Play();
            }
            else
            {
                 myaudiosource.loop = false;
                 myaudiosource.PlayOneShot(gun_data.shootsound_start);
           

            }
        }
            
        

        
    }
    void continue_shoot()
    {
        if(elaspsed_time > 0)
        {

            
            if(elaspsed_time > gun_data.timeBetweenShooting / 2 && !continous_fire)
            {
                Debug.LogError("Doing start Shoot sound");
                myaudiosource.PlayOneShot(gun_data.shootsound_start);
                playshootsound_ServerRpc(false);
        
                
                continous_fire = true;
               

                 
            }
            if(elaspsed_time <gun_data.timeBetweenShooting/2.8f && continous_fire)
            {
                continous_fire = false;
            myaudiosource.loop = false;
            myaudiosource.clip = null;
            myaudiosource.PlayOneShot(gun_data.shootsound_end);
            playendsound_ServerRpc();
            Debug.LogError("Playing End Sound");
              set_server_continue_fire = false;
              

            }
        if(elaspsed_time > gun_data.timeBetweenShooting)
        {
            elaspsed_time =gun_data.timeBetweenShooting;
        }
        if(continous_fire)
      {
       if(!set_server_continue_fire)
       {
        Debug.LogError("Doing continue Fire");
        set_server_continue_fire = true;
        playshootsound_ServerRpc(true);
      
         myaudiosource.clip =gun_data.shootsound_continue;
            myaudiosource.loop = true;
            myaudiosource.Play();
       }
        }
       
      

      }
      
        
        

      
        
        
        
       
        
    }
    [ServerRpc (RequireOwnership = false)]
    void playendsound_ServerRpc()
    {
        playendSound_ClientRpc();

    }
    [ClientRpc]
    void playendSound_ClientRpc()
    {
        if(!IsLocalPlayer)
        {
            
            myaudiosource.loop = false;
            myaudiosource.clip = null;
                 myaudiosource.PlayOneShot(gun_data.shootsound_end);
           

            
        }
            
        

        
    }

    private void Shoot(float spreadX,float SpreadY)
    {
       // new_particlesystem.Play();
        
        readyToShoot = false;
 
      shootnormal(spreadX,SpreadY);
      Gun_stuff.cam_recoil.Fire();
      if(gun_data.use_old_sound_system && !gun_data.is_shotgun)
      {
       myaudiosource.PlayOneShot(gun_data.shootsound_start);
      playshootsound_ServerRpc(false);
      }
      
      
  
       if(!is_mobile)
       {

       
       Gun_stuff.thrid_cam_script.mouseY += gun_data.upwards_recoil;
       }
       else
       {
        Gun_stuff.thrid_cam_script.mouseY += gun_data.upwards_recoil_mobile;
       }
      
     
        
      

        
        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        if(MuzzleFlash != null)
        {
        MuzzleFlash.Play();
        }
      
       

        bulletsLeft--;
        bulletsShot--;
        

        Invoke("ResetShot", gun_data.timeBetweenShooting);
        

        if(bulletsShot > 0 && bulletsLeft > 0 && gun_data.is_shotgun)
        Invoke("shoot_shotgun", gun_data.timeBetweenShots);
    }

    void shoot_shotgun()
    {
        bulletsLeft--;
        bulletsShot--;
        float x = Random.Range(-gun_data.spread, gun_data.spread);
         float y = Random.Range(-gun_data.spread, gun_data.spread);


         shootnormal(x,y);

        if(bulletsShot > 0 && bulletsLeft > 0 && gun_data.is_shotgun)
        Invoke("shoot_shotgun", gun_data.timeBetweenShots);

    }
    
    
    public void shootnormal(float spreadX,float SpreadY)
    {
        int damage = base_damage * level_and_rarity.Gun_level.Value;
        Gun_stuff.bullet_pen.UpdatePenetration();

     
        
        Vector3 direction = raycast_point.transform.forward + new Vector3(spreadX, SpreadY, 0);
        

         if (Physics.Raycast(raycast_point.transform.position, direction, out rayHit, gun_data.range,Gun_stuff.whatIsEnemy))
        {
            if(gun_data.tracer_enabled)

            {
            Vector3 lol = rayHit.point;
            Tracers_ServerRpc(lol);
            TrailRenderer trail = Instantiate(Bullettrail,attackPoint_hip.transform.position,Quaternion.identity);
            StartCoroutine(SpawnTrail(trail,rayHit));
            }

            impactPoint = rayHit.point;
            Ray penRay = new Ray(rayHit.point + direction * penetrationAmount, -direction);
            RaycastHit penHit;
             if (rayHit.collider.Raycast(penRay, out penHit, penetrationAmount))
            {
              //  Debug.LogError(penHit.collider.name);
                penetrationPoint = penHit.point;
                endPoint = raycast_point.transform.position + raycast_point.transform.forward * 1000;
                Ray penlolRay = new Ray(penHit.point, endPoint);
                
                if(Physics.Raycast(penlolRay,out penlolHit,20f))
                {
                    

                   // endPoint = penlolHit.point;
                    if(penlolHit.collider.GetComponent<ShootingAi>()!= null)
                {
                penlolHit.collider.GetComponent<ShootingAi>().TakeDamageServerRpc(damage);
                hitenable();
                int num = Random.Range(0,hitmarker_data.hitmarkersounds.Length);
                Gun_stuff.playeraudio.PlayOneShot(hitmarker_data.hitmarkersounds[num]);
                

                }
                GameObject hit_obj = penlolHit.collider.gameObject;
                if(penlolHit.collider.GetComponent<zom_normal>() !=null)
                {
                penlolHit.collider.GetComponent<zom_normal>().normalDamageServerRpc(damage /2f,Gun_stuff.myhealthscript.NetworkObjectId.ToString(),false);
                has_hit_zombie = true;
               
                }
                if(penlolHit.collider.GetComponent<zom_critical>() !=null)
                {
                penlolHit.collider.GetComponent<zom_critical>().CriticalDamageServerRpc(damage /1.3f,Gun_stuff.myhealthscript.NetworkObjectId.ToString(),false);
                has_hit_zombie = true;
               
                }


                

                }
               
            }
            else
            {
                endPoint = impactPoint.Value + direction * penetrationAmount;
                penetrationPoint = endPoint;
            }
            //Debug.DrawLine(raycast_point.transform.position,rayHit.point, Color.red);
            //linerend.SetPosition(0,raycast_point.transform.position);
            //linerend.SetPosition(1,rayHit.point);
            //Debug.Log(rayHit.collider.name);
          
             if(rayHit.collider.GetComponent<shoot_spawn>() !=null)
                {
                    rayHit.collider.GetComponent<shoot_spawn>().spawn_zom_ServerRpc();

                }
                if(Gun_stuff.pvp)
                {
                if(rayHit.collider.GetComponent<Player_Health>() !=null)
                {
                    Player_Health p_health =  rayHit.collider.GetComponent<Player_Health>();
                    if(p_health != this.Gun_stuff.myhealthscript)
                    {
                        GameObject popup = Instantiate(Damage_popup,rayHit.point,Damage_popup.transform.rotation);
                        billboarder pop_bill = popup.GetComponent<billboarder>();
                pop_bill.camera = Gun_stuff.fpsCam.transform;
                pop_bill.target = rayHit.collider.gameObject;
                pop_bill.damage_Text.text = (damage / 9).ToString();
                    p_health.TakeDamagePlayer_ServerRpc(damage / 9);
                    }

                }
                }
            if(rayHit.collider.GetComponent<zom_critical>() !=null)
                {
                    GameObject popup = Instantiate(Damage_popup,rayHit.point,Damage_popup.transform.rotation);
                billboarder pop_bill = popup.GetComponent<billboarder>();
                pop_bill.camera = Gun_stuff.fpsCam.transform;
                pop_bill.target = rayHit.collider.gameObject;
                pop_bill.damage_Text.color = Color.yellow;
                pop_bill.damage_Text.text = (damage *2f).ToString();
                rayHit.collider.GetComponent<zom_critical>().CriticalDamageServerRpc(damage * 2f,Gun_stuff.myhealthscript.NetworkObjectId.ToString(),false);
                hitenable();
                int num = Random.Range(0,hitmarker_data.hitmarkersounds.Length);
                Gun_stuff.playeraudio.PlayOneShot(hitmarker_data.hitmarkersounds[num]);
                
                has_hit_zombie = true;
                }
                if(rayHit.collider.GetComponent<zom_normal>() !=null)
                {
                    GameObject popup = Instantiate(Damage_popup,rayHit.point,Damage_popup.transform.rotation);
                     billboarder pop_bill = popup.GetComponent<billboarder>();
                pop_bill.camera = Gun_stuff.fpsCam.transform;
                 pop_bill.target = rayHit.collider.gameObject;
               pop_bill.damage_Text.text = damage.ToString();
                rayHit.collider.GetComponent<zom_normal>().normalDamageServerRpc(damage,Gun_stuff.myhealthscript.NetworkObjectId.ToString(),false);
                hitenable();
                int num = Random.Range(0,hitmarker_data.hitmarkersounds.Length);
                Gun_stuff.playeraudio.PlayOneShot(hitmarker_data.hitmarkersounds[num]);
                
                has_hit_zombie = true;
                }
                if(rayHit.collider.GetComponent<ShootingAi>()!= null)
                {
                 GameObject popup = Instantiate(Damage_popup,rayHit.point,Damage_popup.transform.rotation);
                     billboarder pop_bill = popup.GetComponent<billboarder>();
                pop_bill.camera = Gun_stuff.fpsCam.transform;
                 pop_bill.target = rayHit.collider.gameObject;
               pop_bill.damage_Text.text = damage.ToString();

                rayHit.collider.GetComponent<ShootingAi>().TakeDamageServerRpc(damage);
                hitenable();
                int num = Random.Range(0,hitmarker_data.hitmarkersounds.Length);
                Gun_stuff.playeraudio.PlayOneShot(hitmarker_data.hitmarkersounds[num]);
                
                has_hit_zombie = true;
                }
                 if(rayHit.collider.GetComponent<Worm_target>() !=null)
                {
                    GameObject popup = Instantiate(Damage_popup,rayHit.point,Damage_popup.transform.rotation);
                popup.GetComponent<billboarder>().camera = Gun_stuff.fpsCam.transform;
                popup.GetComponent<billboarder>().damage_Text.text = damage.ToString();
                rayHit.collider.GetComponent<Worm_target>().worm_Script.TakeDamageZomServerRpc(damage,Gun_stuff.myhealthscript.NetworkObjectId.ToString(),false);
                hitenable();
                int num = Random.Range(0,hitmarker_data.hitmarkersounds.Length);
                Gun_stuff.playeraudio.PlayOneShot(hitmarker_data.hitmarkersounds[num]);
                
                has_hit_zombie = true;
                }
             
           

            
        
        }
        else
        {
            
            endPoint = this.transform.position + this.transform.forward * 1000;
            penetrationPoint = null;
            impactPoint = null;
        }
        
        
            
             
            


        
        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        

      
    }
  
    
    private void ResetShot()
    {
        readyToShoot = true;
        
        bullets_shot++;
        gun_rpm_calc();
        //StartCoroutine(rpm());
        
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
    }
     public void hitenable()
         {

         // hitmark.SetActive(true);
          Gun_stuff.hitmark_anim.Play("hit_marker_anim");
        //  Invoke("hitdisable",0.2f);
        // hitmarker_image.color = new Color (default_color.r,default_color.g,default_color.b,default_color.a);
        // StartCoroutine(fade_hitMarker());
        


         }
         
         
    public void hitdisable()
         {
           
            
      hitmark.SetActive(false);

         }
    
    
         private void OnDrawGizmos()
    {
       //bullet_pen.UpdatePenetration();

        if(!penetrationPoint.HasValue || !impactPoint.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(raycast_point.transform.position, endPoint);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(raycast_point.transform.position, impactPoint.Value);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(impactPoint.Value, penetrationPoint.Value);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(penetrationPoint.Value ,penlolHit.point);
        }
    }
    IEnumerator SpawnTrail(TrailRenderer trail,RaycastHit hit)
    {
        float time =0;
        Vector3 start_pos = attackPoint_hip.transform.position;
        while(time <1)
        {
            trail.transform.position = Vector3.Lerp(start_pos,hit.point,time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        Destroy(trail.gameObject,trail.time);
    }
    IEnumerator SpawnTrail_client(TrailRenderer trail,Vector3 hit)
    {
        float time =0;
        Vector3 start_pos = trail.transform.position;

        while(time <1)
        {
            trail.transform.position = Vector3.Lerp(start_pos,hit,time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        Destroy(trail.gameObject,trail.time);
    }
    [ServerRpc(RequireOwnership = false)]
    void Tracers_ServerRpc(Vector3 hit)
    {
        Tracers_ClientRpc(hit);
        

    }
    
    [ClientRpc]
    
    
    void Tracers_ClientRpc(Vector3 hit)
    {
        if(!IsLocalPlayer)
        {
            if(MuzzleFlash != null)
        {
        MuzzleFlash.Play();
        }
        Instantiate(bulletHoleGraphic, hit, Quaternion.Euler(0, 180, 0));
        TrailRenderer trail = Instantiate(Bullettrail,attackPoint_hip.transform.position,Quaternion.identity);
        StartCoroutine(SpawnTrail_client(trail,hit));
        }

    }
    public void Check_Rarity()
    {
        laser_active = false;
        Laser.enabled = false;
        base_damage = gun_data.def_damage;
        magazineSize = gun_data.def_magsize;
       
        
           

        if(level_and_rarity.Rareity_level.Value >=2)
        {
            laser_active = true;
        }
        if(level_and_rarity.Rareity_level.Value == 3)
        {
            magazineSize = gun_data.rarity_lvl_3_magsize;
             laser_active = true;
        }
        if(level_and_rarity.Rareity_level.Value == 4)
        {
            base_damage = gun_data.rarity_lvl_4_damage;
             magazineSize = gun_data.rarity_lvl_3_magsize;
              laser_active = true;

        }
       
        if(level_and_rarity.Rareity_level.Value >= 5)
        {
            base_damage = gun_data.rarity_lvl_5_damage;
            magazineSize = gun_data.rarity_lvl_3_magsize;
             laser_active = true;
        }
        if(laser_active)
        {
            Laser.enabled = true;
        }
        
    
        
           
        
       
    }
    
    
    
}
