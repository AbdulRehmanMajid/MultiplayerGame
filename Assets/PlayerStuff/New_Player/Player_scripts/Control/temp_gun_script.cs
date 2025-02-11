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
    #region Fields & References
    // Data & Configurations
     public bool laser_active = true;
    public float laser_range = 40f;
    public Rifle_Data gun_data;
    public HitMarker_Sounds hitmarker_data;
    public Rarity_and_level level_and_rarity;
    public GameObject Damage_popup;
    public VisualEffect MuzzleFlash;
    public Gun_stuff_holder Gun_stuff;

    // Visuals
    public TrailRenderer Bullettrail;
    public LineRenderer Laser;
    public GameObject bulletHoleGraphic;
    
    // Audio & UI
    public AudioSource myaudiosource;
    public GameObject hitmark;
    public TextMeshProUGUI text;

    // Input & State Flags
    public bool is_aiming;
    public bool shooting, readyToShoot = true, reloading;
    public bool continous_fire = false;
    
    // Transforms & Raycast
    Transform attackPoint;
    public Transform attackPoint_hip;
    public Transform attackPoint_Ads;
    public Transform raycast_point;
    public RaycastHit rayHit;
    
    // Bullet Penetration
    public float penetrationAmount;
    Vector3 endPoint;
    Vector3? penetrationPoint;
    Vector3? impactPoint;
    public RaycastHit penlolHit;
    bool has_hit_zombie = false;
    
    // Magazines & Damage
    int bulletsLeft;
    int bulletsShot;
    int magazineSize;
    int old_rarity;
    int base_damage;

    // Mobile & RPM
    bool is_mobile = false;
    bool is_invoked = false;
    int bullets_shot_in_one_sec = 0;
    int bullets_shot = 0;
    public float elaspsed_time = 0;
    bool set_server_continue_fire = false;
    #endregion

    #region Initialization & Update
    private void Awake()
    {
        if (!IsOwner) return;

        is_mobile = (Application.platform == RuntimePlatform.Android);
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
        if (!IsOwner) return;
    }

    private void LateUpdate()
    {
        // Update Laser if active.
        if (laser_active && Laser != null)
        {
            Laser.SetPosition(0, attackPoint_hip.position);
            Laser.SetPosition(1, attackPoint_hip.position + attackPoint_hip.forward * laser_range);
        }
        
        if (!IsOwner) return;
        
        // Check for rarity changes.
        if (level_and_rarity.Rareity_level.Value != old_rarity)
        {
            Check_Rarity();
            old_rarity = level_and_rarity.Rareity_level.Value;
        }

        // Update UI and process input if player is alive.
        if (Gun_stuff.myhealthscript.Is_alive.Value)
        {
            MyInput();
            if (!gun_data.is_shotgun)
                text.SetText(bulletsLeft + " / " + magazineSize);
            else
                text.SetText((bulletsLeft / gun_data.bulletsPerTap) + " / " + (magazineSize / gun_data.bulletsPerTap));
        }

        // Process continuous shooting sounds.
        if (!gun_data.use_old_sound_system)
        {
            continue_shoot();
            if (shooting && !reloading)
                elaspsed_time += Time.deltaTime;
            else
            {
                elaspsed_time -= Time.deltaTime;
                if (elaspsed_time < 0)
                    elaspsed_time = 0;
            }
        }
    }
    #endregion

    #region Input & Shooting Methods
    private void MyInput()
    {
        if (bulletsLeft == 0 && !reloading)
            Reload();

        shooting = Gun_stuff.new_inputs.shoot;

        if (Gun_stuff.new_inputs.reload && bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = gun_data.bulletsPerTap;

            // Apply sniper aiming flag.
            Gun_stuff.aim_transition.is_sniper = gun_data.is_sniper;

            // Calculate random spread.
            float x = Random.Range(-gun_data.spread, gun_data.spread);
            float y = Random.Range(-gun_data.spread, gun_data.spread);

            // If not in continuous firing mode, play the single shot sound.
            if (!continous_fire)
            {
                myaudiosource.PlayOneShot(gun_data.shootsound_start);
                playshootsound_ServerRpc(false);
            }

            Shoot(x, y);
            is_aiming = true;
            attackPoint = attackPoint_Ads;
            raycast_point = Gun_stuff.fpsCam.transform;

            if (Input.GetMouseButtonDown(0) && gun_data.is_shotgun)
                myaudiosource.PlayOneShot(gun_data.shootsound_start);
        }
    }

    private void Shoot(float spreadX, float spreadY)
    {
        readyToShoot = false;
        shootnormal(spreadX, spreadY);
        Gun_stuff.cam_recoil.Fire();

        if (gun_data.use_old_sound_system && !gun_data.is_shotgun)
        {
            myaudiosource.PlayOneShot(gun_data.shootsound_start);
            playshootsound_ServerRpc(false);
        }
       
        // Apply recoil based on platform.
        if (!is_mobile)
            Gun_stuff.thrid_cam_script.mouseY += gun_data.upwards_recoil;
        else
            Gun_stuff.thrid_cam_script.mouseY += gun_data.upwards_recoil_mobile;
        
        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        if (MuzzleFlash != null)
            MuzzleFlash.Play();
      
        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", gun_data.timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0 && gun_data.is_shotgun)
            Invoke("shoot_shotgun", gun_data.timeBetweenShots);
    }

    void shoot_shotgun()
    {
        bulletsLeft--;
        bulletsShot--;
        float x = Random.Range(-gun_data.spread, gun_data.spread);
        float y = Random.Range(-gun_data.spread, gun_data.spread);
        shootnormal(x, y);

        if (bulletsShot > 0 && bulletsLeft > 0 && gun_data.is_shotgun)
            Invoke("shoot_shotgun", gun_data.timeBetweenShots);
    }

    public void shootnormal(float spreadX, float spreadY)
    {
        int damage = base_damage * level_and_rarity.Gun_level.Value;
        Gun_stuff.bullet_pen.UpdatePenetration();

        Vector3 direction = raycast_point.forward + new Vector3(spreadX, spreadY, 0);

        if (Physics.Raycast(raycast_point.position, direction, out rayHit, gun_data.range, Gun_stuff.whatIsEnemy))
        {
            // Visual tracer effect.
            if (gun_data.tracer_enabled)
            {
                Tracers_ServerRpc(rayHit.point);
                TrailRenderer trail = Instantiate(Bullettrail, attackPoint_hip.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, rayHit));
            }

            // Process penetration and direct hit effects.
            ProcessPenetration(rayHit, direction, damage);
            ProcessDirectHit(rayHit, damage);
        }
        else
        {
            // No hit: set default endpoint.
            endPoint = transform.position + transform.forward * 1000;
            penetrationPoint = null;
            impactPoint = null;
        }

        // Create bullet decal.
        Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
    }
    #endregion

    #region Bullet Penetration System
    /// <summary>
    /// Handles the penetration raycast from the initial hit.
    /// </summary>
    private void ProcessPenetration(RaycastHit initialHit, Vector3 direction, int damage)
    {
        impactPoint = initialHit.point;
        // Offset to ensure the ray starts outside the hit collider.
        Vector3 penetrationStart = initialHit.point + direction * 0.01f;
        Ray penRay = new Ray(penetrationStart, -direction);
        RaycastHit penHit;
        
        if (Physics.Raycast(penRay, out penHit, penetrationAmount))
        {
            Debug.Log("Penetration ray hit: " + penHit.collider.name);
            penetrationPoint = penHit.point;
            endPoint = raycast_point.position + raycast_point.forward * 1000;

            Vector3 penDirection = (endPoint - penHit.point).normalized;
            Ray penlolRay = new Ray(penHit.point, penDirection);
            if (Physics.Raycast(penlolRay, out penlolHit, 20f))
            {
                Debug.Log("Secondary penetration ray hit: " + penlolHit.collider.name);
                ApplyPenetrationDamage(penlolHit, damage);
            }
            else
            {
                Debug.Log("Secondary penetration ray did not hit any target.");
            }
        }
        else
        {
            Debug.Log("Penetration ray did not hit any target.");
            endPoint = impactPoint.Value + direction * penetrationAmount;
            penetrationPoint = endPoint;
        }
    }

    /// <summary>
    /// Applies damage through penetration based on the hit type.
    /// </summary>
    private void ApplyPenetrationDamage(RaycastHit penHit, int damage)
    {
        ShootingAi shootingAi = penHit.collider.GetComponent<ShootingAi>();
        if (shootingAi != null)
        {
            shootingAi.TakeDamageServerRpc(damage);
            hitenable();
            PlayRandomHitMarker();
        }

        zom_normal zomNormal = penHit.collider.GetComponent<zom_normal>();
        if (zomNormal != null)
        {
            zomNormal.normalDamageServerRpc(damage / 2, Gun_stuff.myhealthscript.NetworkObjectId.ToString(), false);
        }

        zom_critical zomCritical = penHit.collider.GetComponent<zom_critical>();
        if (zomCritical != null)
        {
            zomCritical.CriticalDamageServerRpc(damage / 1.3f, Gun_stuff.myhealthscript.NetworkObjectId.ToString(), false);
        }
    }

    /// <summary>
    /// Processes the direct hit from the initial raycast.
    /// </summary>
    private void ProcessDirectHit(RaycastHit hit, int damage)
    {
        // Spawn zombie if applicable.
        if (hit.collider.GetComponent<shoot_spawn>() != null)
        {
            hit.collider.GetComponent<shoot_spawn>().spawn_zom_ServerRpc();
        }

        // PvP damage.
        if (Gun_stuff.pvp)
        {
            Player_Health p_health = hit.collider.GetComponent<Player_Health>();
            if (p_health != null && p_health != Gun_stuff.myhealthscript)
            {
                ShowDamagePopup(hit.point, damage / 9f, hit.collider.gameObject);
                p_health.TakeDamagePlayer_ServerRpc(damage / 9);
            }
        }

        // Handle different enemy types.
        zom_critical zomCrit = hit.collider.GetComponent<zom_critical>();
        if (zomCrit != null)
        {
            ShowDamagePopup(hit.point, damage * 2f, hit.collider.gameObject, Color.yellow);
            zomCrit.CriticalDamageServerRpc(damage * 2f, Gun_stuff.myhealthscript.NetworkObjectId.ToString(), false);
            hitenable();
            PlayRandomHitMarker();
            has_hit_zombie = true;
        }

        zom_normal zomNormal = hit.collider.GetComponent<zom_normal>();
        if (zomNormal != null)
        {
            ShowDamagePopup(hit.point, damage, hit.collider.gameObject);
            zomNormal.normalDamageServerRpc(damage, Gun_stuff.myhealthscript.NetworkObjectId.ToString(), false);
            hitenable();
            PlayRandomHitMarker();
            has_hit_zombie = true;
        }

        ShootingAi shootingAi = hit.collider.GetComponent<ShootingAi>();
        if (shootingAi != null)
        {
            ShowDamagePopup(hit.point, damage, hit.collider.gameObject);
            shootingAi.TakeDamageServerRpc(damage);
            hitenable();
            PlayRandomHitMarker();
            has_hit_zombie = true;
        }

        Worm_target wormTarget = hit.collider.GetComponent<Worm_target>();
        if (wormTarget != null)
        {
            ShowDamagePopup(hit.point, damage, null);
            wormTarget.worm_Script.TakeDamageZomServerRpc(damage, Gun_stuff.myhealthscript.NetworkObjectId.ToString(), false);
            hitenable();
            PlayRandomHitMarker();
            has_hit_zombie = true;
        }
    }
    #endregion

    #region Visual & Sound Effects
    /// <summary>
    /// Instantiates a damage popup with optional text color.
    /// </summary>
    private void ShowDamagePopup(Vector3 position, float dmg, GameObject target, Color? textColor = null)
    {
        GameObject popup = Instantiate(Damage_popup, position, Damage_popup.transform.rotation);
        billboarder pop_bill = popup.GetComponent<billboarder>();
        pop_bill.camera = Gun_stuff.fpsCam.transform;
        if (target != null)
            pop_bill.target = target;
        if (textColor.HasValue)
            pop_bill.damage_Text.color = textColor.Value;
        pop_bill.damage_Text.text = dmg.ToString();
    }

    /// <summary>
    /// Plays a random hit marker sound.
    /// </summary>
    private void PlayRandomHitMarker()
    {
        int num = Random.Range(0, hitmarker_data.hitmarkersounds.Length);
        Gun_stuff.playeraudio.PlayOneShot(hitmarker_data.hitmarkersounds[num]);
    }

    IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 start_pos = attackPoint_hip.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(start_pos, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        Destroy(trail.gameObject, trail.time);
    }

    IEnumerator SpawnTrail_client(TrailRenderer trail, Vector3 hit)
    {
        float time = 0;
        Vector3 start_pos = trail.transform.position;
        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(start_pos, hit, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        Destroy(trail.gameObject, trail.time);
    }

    [ServerRpc(RequireOwnership = false)]
    void Tracers_ServerRpc(Vector3 hit)
    {
        Tracers_ClientRpc(hit);
    }

    [ClientRpc]
    void Tracers_ClientRpc(Vector3 hit)
    {
        if (!IsLocalPlayer)
        {
            if (MuzzleFlash != null)
                MuzzleFlash.Play();
            Instantiate(bulletHoleGraphic, hit, Quaternion.Euler(0, 180, 0));
            TrailRenderer trail = Instantiate(Bullettrail, attackPoint_hip.position, Quaternion.identity);
            StartCoroutine(SpawnTrail_client(trail, hit));
        }
    }
    
    void continue_shoot()
    {
        if (reloading)
        {
            StopShootingSound();
            return;
        }

        if (shooting)
        {
            elaspsed_time += Time.deltaTime;

            if (!continous_fire && elaspsed_time > gun_data.timeBetweenShooting / 2)
            {
                continous_fire = true;
                set_server_continue_fire = false;
            }

            if (continous_fire && !set_server_continue_fire)
            {
                set_server_continue_fire = true;
                StartCoroutine(StartContinuousFireSoundDelay());
            }

            if (elaspsed_time > gun_data.timeBetweenShooting)
                elaspsed_time = gun_data.timeBetweenShooting;
        }
        else
        {
            
            if (continous_fire)
            {
                continous_fire = false;
                set_server_continue_fire = false;
                myaudiosource.loop = false;
                myaudiosource.clip = null;
                myaudiosource.Stop(); 
                myaudiosource.PlayOneShot(gun_data.shootsound_end);
                playendsound_ServerRpc();
            }

            elaspsed_time -= Time.deltaTime;
            if (elaspsed_time < 0)
                elaspsed_time = 0;
        }
    }

    IEnumerator StartContinuousFireSoundDelay()
    {
        
        yield return new WaitForSeconds(0.1f);
        if (shooting && continous_fire)
        {
            playshootsound_ServerRpc(true);
            myaudiosource.clip = gun_data.shootsound_continue;
            myaudiosource.loop = true;
            myaudiosource.Play();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void playshootsound_ServerRpc(bool continues)
    {
        playShootSound_ClientRpc(continues);
    }

    [ClientRpc]
    void playShootSound_ClientRpc(bool continous)
    {
        if (!IsLocalPlayer)
        {
            if (continous)
            {
                myaudiosource.clip = gun_data.shootsound_continue;
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

    [ServerRpc(RequireOwnership = false)]
    void playendsound_ServerRpc()
    {
        playendSound_ClientRpc();
    }

    [ClientRpc]
    void playendSound_ClientRpc()
    {
        if (!IsLocalPlayer)
        {
            myaudiosource.loop = false;
            myaudiosource.clip = null;
            myaudiosource.PlayOneShot(gun_data.shootsound_end);
        }
    }
    #endregion

    #region Utility Methods
    private void ResetShot()
    {
        readyToShoot = true;
        bullets_shot++;
        gun_rpm_calc();
        //StartCoroutine(rpm());
    }

    private void Reload()
    {
        StopShootingSound();
        StopShootSound_ClientRpc();
        Gun_stuff.aim_transition.Reload();
        reloading = true;
        Invoke("ReloadFinished", gun_data.reloadTime);
    }
    [ServerRpc (RequireOwnership =false)]
    public void StopShootSound_ServerRpc()
    {
        StopShootSound_ClientRpc();

    }
    [ClientRpc]
    public void StopShootSound_ClientRpc()
    {
        if (!IsLocalPlayer)
        {
       myaudiosource.loop = false;
        myaudiosource.Stop();
        }
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
[ServerRpc(RequireOwnership = false)]
    public void kill_Gun_ServerRpc()
    {
        Destroy(this.gameObject);
        this.gameObject.GetComponent<NetworkObject>().Despawn();
    }

    public void gun_rpm_calc()
    {
        int current_rpm = bullets_shot_in_one_sec * 60;
        if (bullets_shot_in_one_sec > 0)
            Debug.LogError("Current Rpm is:" + current_rpm);
    }

    IEnumerator rpm()
    {
        if (!is_invoked)
        {
            is_invoked = true;
            bullets_shot_in_one_sec = 0;
            yield return new WaitForSeconds(1f);
            bullets_shot_in_one_sec = bullets_shot;
            bullets_shot = 0;
            is_invoked = false;
        }
    }

    public void hitenable()
    {
        // hitmark.SetActive(true);
        Gun_stuff.hitmark_anim.Play("hit_marker_anim");
    }
    
    public void hitdisable()
    {
        hitmark.SetActive(false);
    }

    public void Check_Rarity()
    {
        laser_active = false;
        Laser.enabled = false;
        base_damage = gun_data.def_damage;
        magazineSize = gun_data.def_magsize;

        if (level_and_rarity.Rareity_level.Value >= 2)
            laser_active = true;

        if (level_and_rarity.Rareity_level.Value == 3)
        {
            magazineSize = gun_data.rarity_lvl_3_magsize;
            laser_active = true;
        }

        if (level_and_rarity.Rareity_level.Value == 4)
        {
            base_damage = gun_data.rarity_lvl_4_damage;
            magazineSize = gun_data.rarity_lvl_3_magsize;
            laser_active = true;
        }

        if (level_and_rarity.Rareity_level.Value >= 5)
        {
            base_damage = gun_data.rarity_lvl_5_damage;
            magazineSize = gun_data.rarity_lvl_3_magsize;
            laser_active = true;
        }

        if (laser_active)
            Laser.enabled = true;
    }

    private void StopShootingSound()
    {
        continous_fire = false;
        set_server_continue_fire = false;
        elaspsed_time = 0;
        myaudiosource.loop = false;
        myaudiosource.Stop();
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!penetrationPoint.HasValue || !impactPoint.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(raycast_point.position, endPoint);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(raycast_point.position, impactPoint.Value);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(impactPoint.Value, penetrationPoint.Value);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(penetrationPoint.Value, penlolHit.point);
        }
    }
    #endregion
}
