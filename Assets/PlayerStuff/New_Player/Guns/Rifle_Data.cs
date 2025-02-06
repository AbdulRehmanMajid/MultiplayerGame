using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gun_Data",menuName ="Gun_stuff")]
public class Rifle_Data : ScriptableObject
{
    [Header("Stats Stuff")]
    
    public int def_damage;
    public int def_magsize;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int bulletsPerTap;
    public bool allowButtonHold;
    [Space]

     [Header("GunType Stuff")]
     
      public bool is_shotgun = false;
    
    public bool is_sniper = false;
    [Space]
     [Header("Recoil Stuff")]
    public float upwards_recoil;
    public float upwards_recoil_mobile;
      public float rotationSpeed;
	public float returnSpeed;
    public Vector3 Recoil;
    public Vector3 Recoil_mobile;
    [Space]
   
    [Header("Misc Stuff")]
     public bool use_old_sound_system = false;
    public AudioClip shootsound_start;
    public AudioClip shootsound_continue;
    public AudioClip shootsound_end;

     public bool tracer_enabled = false;
   
     [Space]

     [Header("Rarity Stuff")]
     public int rarity_lvl_3_magsize;
    public int rarity_lvl_4_damage;
    public int rarity_lvl_5_damage;
     
    
}
