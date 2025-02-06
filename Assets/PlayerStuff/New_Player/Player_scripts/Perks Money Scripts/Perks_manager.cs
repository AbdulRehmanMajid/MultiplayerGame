using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class Perks_manager : NetworkBehaviour
{
    public StarterAssetsInputs new_inputs;
     
      public bool has_pressed_pick_button = false;
    public TextMeshProUGUI Perk_text;
    public float perk_drink_wait_time = 4f;
    public bool can_buy_perk = false;
    public int perk_id;
    public bool has_healthPerk=false;
    public bool has_speedPerk= false;
    public bool has_fastReload = false;
    public bool has_quick_rev = false; 
    public Player_Health p_health;
    public Animator p_anim;
    
public vThirdPersonMotor p_controller;
    [Header("Health Perk Values")]
     
    public float max_health = 250f;

    public int health_perk_Cost = 500;
    public GameObject Health_perk_icon_prefab;
    public GameObject Health_perk_bottle;
    public Aim_Transitioner aim_Transitioner;

     [Header("Speed Perk Values")]
     public int Speed_Perk_cost =500;
    
    public float Sprint_speed;
    public float sprint_shoot_speed;
    public GameObject Speed_Perk_icon_prefab;
    public GameObject Speed_perk_bottle;
     [Header("FastReload Perk Values")]
     public int Fast_reload_cost = 500;
     public float reload_speed;
     public GameObject Fast_reload_icon_prefab;
     public GameObject Fast_reload_bottle;
      [Header("Quick Revive Perk Values")]
      public float quick_rev_cost = 500;
      public float revive_Speed;
      public float Regen_speed;
      public float regen_time;
      public Revive_Script_player player_rev_script;
      public GameObject quick_rev_icon_prefab;
      public GameObject quick_rev_bottle;
      bool can_buy = false;
   

    // Update is called once per frame
    void Start()
    {
        Health_perk_bottle.SetActive(false);
        
    }
    void Update()
    {
        if(!IsOwner)return;
         if(new_inputs.buy_button && !has_pressed_pick_button)
        {
            has_pressed_pick_button = true;
            can_buy = true;

        }
        else if(!new_inputs.buy_button)
        {
            has_pressed_pick_button = false;
            can_buy = false;
        }
        if(perk_id != 0)
        {
            if(perk_id == 1 && !has_healthPerk)
            {
                Perk_text.text = "Press For Jam-e-Sheeri";

            }
        }
        else
        {
            if(Perk_text.text!= "")
            {
             Perk_text.text = "";
            }
        }
        if(can_buy&& perk_id == 1 && can_buy_perk && p_health.money_man.Money.Value >= 500 && !has_healthPerk)
        {
            GiveHealthPerk();
            can_buy = false;

        }
        
        if(Input.GetKeyDown(KeyCode.L)&& p_health.money_man.Money.Value >= 500 && !has_speedPerk)
        {
            GiveSpeedPerk();
            
        }
        if(Input.GetKeyDown(KeyCode.J)&& p_health.money_man.Money.Value >= 500 && !has_quick_rev)
        {
            GiveRevPerk();
            
        }
        
    }
    void perk_anim(bool state)
    {
        aim_Transitioner.setPerk_Drink_State_ServerRpc(state);
        p_anim.SetLayerWeight(1,1);
         
    }

    public void GiveHealthPerk()
    {
        if(!IsOwner)return;
        StartCoroutine(drink_perk(1));
        perk_anim(true);
        p_anim.Play("Drink");
        p_health.money_man.TakeMoney_ServerRpc(500);
        GiveHealthPerk_ServerRpc();
        has_healthPerk = true;
        perk_anim(false);
        

    }
    public void GiveSpeedPerk()
    {
        if(!IsOwner)return;
        StartCoroutine(drink_perk(2));
        perk_anim(true);
          p_anim.Play("Drink");
        p_health.money_man.TakeMoney_ServerRpc(500);
        p_controller.sprintSpeed_new = Sprint_speed;
        has_speedPerk =true;
        perk_anim(false);

    }
    public void GiveRevPerk()
    {
         if(!IsOwner)return;
         StartCoroutine(drink_perk(3));
         perk_anim(true);
           p_anim.Play("Drink");
         p_health.money_man.TakeMoney_ServerRpc(500);
         p_health.regen_speed = Regen_speed;
         p_health.regen_timer = regen_time;
         p_health.regen_timer_value = regen_time;
         player_rev_script.revive_time_limit = revive_Speed;
         has_quick_rev = true;
         perk_anim(false);

    }
    [ServerRpc(RequireOwnership = false)]
    void GiveHealthPerk_ServerRpc()
    {
        p_health.max_healthe.Value = max_health;
       // p_health.Healthe.Value = max_health;
         

    }
    [ServerRpc(RequireOwnership = false)]
    void Enable_bottle_ServerRpc(int id,bool state)
    {
        Enable_Bottle_ClientRpc(id,state);

    }
    [ClientRpc]
    void Enable_Bottle_ClientRpc(int id,bool state)
    {
        if(id == 1)
        {
            Health_perk_bottle.SetActive(state);
        }
        if(id == 2)
        {
            Speed_perk_bottle.SetActive(state);
        }
        if(id == 3)
        {
            quick_rev_bottle.SetActive(state);
        }

    }
    IEnumerator drink_perk(int id)
    {
        Enable_bottle_ServerRpc(id,true);
        yield return new WaitForSeconds(perk_drink_wait_time);
         Enable_bottle_ServerRpc(id,false);
    }
}
