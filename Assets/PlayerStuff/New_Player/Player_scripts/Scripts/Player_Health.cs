using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Collections;
using System.IO;
using TMPro;
using Invector.vCharacterController;


public class Player_Health : NetworkBehaviour
{
    bool low_health_exit_state;
    bool low_health_enter_state;
    public AudioClip Low_health_enter;
    public AudioClip low_health_loop;
    public AudioClip low_health_exit;
    public float health_warning_threshold = 30f;
    [SerializeField] public NetworkVariable<float> Healthe = new NetworkVariable<float>(100f,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<float> max_healthe = new NetworkVariable<float>(100f,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> Is_alive = new NetworkVariable<bool>(true,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    [SerializeField]public NetworkVariable<FixedString64Bytes> Net_name = new NetworkVariable<FixedString64Bytes>("",NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
   
    public GameObject HealthBar_object;
    public GameObject revive_icon;
    public Show_Hp_to_others hp_shower;
    
    public TextMeshProUGUI hp;
    public GameObject guns;
    public float regen_timer = 5f;
    public float regen_speed = 50f;

   
    
    public Camera mymaincam;
    public Slider HealthBar;
    public int players_number;
    private int old_player_number = 0;
    public string found_username;
    [SerializeField] private KillFeed killfeedscript;
    public TextMeshProUGUI UsernameText;
    
 
    public TextMeshProUGUI timerui;
    public Slider myhpbar;

    public Transform score_feed_panel;
    public GameObject score_feed_item;
    public GameObject active_score_feed;
    public Transform medal_feed_panel;
    public GameObject medal_item;
    public GameObject one_shot_medal;
    public GameObject multi_kill_medal;
    public GameObject penta_kill_medal;
    public GameObject slaughter_medal;
    public AudioClip killeffect;
    public AudioClip medal_earn;
    public AudioSource player_audio_source;

    public bool is_shooting = false;
    public int medal_count = 0;
    public int k = 0;
    public bool giving_medal = false;
    public bool allow_medal_give = false;
    public float medal_ui_timer;
    
    [Header("MultiKill Medal")]
    public bool multikill = false;
    public bool pentakill = false;
    public bool slaughter = false;

    public int killcounter;
    public bool multikill_checker = false;
    [Header("One Shot One Kill Medal")]
    public bool one_shot_checker = false;
    public int shots_fired;

    public Animator Player_anim_1;
    public Animator Player_anim_2;
    public Rigidbody p_rigid;
    
    public vThirdPersonInput p_input_script;
    public Cam_look_at deathCam;
    public Money_manager_script money_man;
    bool is_healing = false;
    public TextMeshProUGUI regen_timer_text;
    public float regen_timer_value;
    public CapsuleCollider p_collider;
    public LayerMask ignore_collsion;
    public LayerMask def_mask;
    public Transform Spawn_pos;
    public shoot_spawn zomb_spawner;
    public TextMeshProUGUI roundtext;
    public TextMeshProUGUI zombies_rem_text;
    public Spectator_cam spec_cam_script;
    public GameObject Mobile_controls;
    public AudioSource gun_audio_source;
    public float elapsedtime_since_kill = 0f;
    public GameObject total_xp;
    public Transform xp_panel;
    Xp_tracker xp_Track;
    public float max_kill_elapsed_time = 3f;



     public override void OnNetworkSpawn()
     {
        revive_icon.SetActive(false);
        test.RegisterPlayer(this.NetworkObjectId.ToString(),this.GetComponent<Player_Health>());
        
        if(!IsOwner)return;
        //Mobile_controls.SetActive(false);
        if(IsLocalPlayer  && Application.platform == RuntimePlatform.Android)
        {
            Mobile_controls.SetActive(true);
        }
       
        if(Application.platform == RuntimePlatform.Android)
        {
           
            SetNameServerRpc("Player");
        }
          if(Application.platform != RuntimePlatform.Android)
          {
        string path = Application.dataPath + "/username.txt";
        found_username = File.ReadAllText(path);
        Net_name.Value = File.ReadAllText(path);
        
        SetNameServerRpc(Net_name.Value.ToString());
          }
        
        if(Spawn_pos == null)
        {
        GameObject pos = GameObject.FindGameObjectWithTag("Spawn");
        Spawn_pos = pos.transform;
        transform.position = Spawn_pos.transform.position;
        }
       
        StartCoroutine("health_regen_loop");
        if(zomb_spawner ==null)
        {
            zomb_spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<shoot_spawn>();
            zomb_spawner.Round_num.OnValueChanged += Set_round;
            zomb_spawner.Zombies_left.OnValueChanged += Set_zombie;
        }
        Healthe.OnValueChanged+=Low_health;
    
        
     }
     void Start()
    {
        if(!IsOwner)
        {
            timerui.enabled = false;
        }
        else
        {
             timerui.enabled = true;

        }
    }
      [ServerRpc(RequireOwnership = false)]
  public void SetNameServerRpc(string UserName)
  {
    Debug.LogWarning($"RPC reached");
    Debug.LogWarning("Chars name change client is called");
    
    changeclientnameClientRpc(UserName);
    UsernameText.text = UserName;
    

  }
  void stop_audio()
     {
        endSound_ServerRpc();
        gun_audio_source.clip = null;
                 gun_audio_source.loop = false;
                 gun_audio_source.Stop();
     }
      [ServerRpc (RequireOwnership = false)]
    void endSound_ServerRpc()
    {
        endSound_ClientRpc();

    }
    [ClientRpc]
    void endSound_ClientRpc()
    {
        if(!IsLocalPlayer)
        {
            
                 gun_audio_source.clip = null;
                 gun_audio_source.loop = false;
                 gun_audio_source.Stop();
           

            
        }
    }
            

  void Low_health(float old_val,float new_val)
  {
    if(new_val <= health_warning_threshold && !low_health_enter_state && new_val<old_val)
    {
        StartCoroutine("low_health_courutine");
        
    }
    if(new_val>= health_warning_threshold && !low_health_exit_state && low_health_enter_state)
    {
        StartCoroutine("low_health_Exit_courutine");

    }

  }
  IEnumerator low_health_courutine()
  {
    low_health_enter_state = true;
    player_audio_source.PlayOneShot(Low_health_enter);
    yield return new WaitForSeconds(Low_health_enter.length-0.15f);
    player_audio_source.clip = low_health_loop;
    player_audio_source.loop = true;
    player_audio_source.Play();

  }
  IEnumerator low_health_Exit_courutine()
  {
    low_health_exit_state = true;
    low_health_enter_state = false;
    player_audio_source.Stop();
    player_audio_source.loop = false;
    player_audio_source.PlayOneShot(low_health_exit);
    player_audio_source.clip = null;
    yield return new WaitForSeconds(low_health_exit.length);
     low_health_exit_state = false;
    
   

  }
  
  [ClientRpc]
    private void changeclientnameClientRpc(string lol)
    {
        UsernameText.text = lol;


    }

    [ClientRpc]
    public void Regen_timer_set_ClientRpc()

    {
        
        regen_timer =regen_timer_value;
        is_healing = false;

    }

    void Set_round(int old_round, int new_round)
    {
        if(old_round != new_round)
        {
            roundtext.text = "Round: " + new_round.ToString();
        }

    }
    void Set_zombie(int old_val, int new_val)
    {
        if(old_val != new_val)
        {
            zombies_rem_text.text = "Zombies Left: " + new_val.ToString();
        }

    }
    

    
    public void TakeDamage(float amount)
    {
        Regen_timer_set_ClientRpc();
        
        if(Is_alive.Value == true)
        {
        
        HealthBar.value = Healthe.Value / max_healthe.Value;
        
        Healthe.Value -= amount;
        Debug.LogError(Healthe.Value);
        Debug.LogWarning(Healthe.Value / max_healthe.Value);
        HealthBar.value = Healthe.Value / max_healthe.Value;
        hp.text = Healthe.Value.ToString();
        
        if(Healthe.Value <= 0)
        {
            
           
         
            
            HealthBar_object.SetActive(false);
            revive_icon.SetActive(true);
            Is_alive.Value = false;
            do_death_zombie_ClientRpc();
            
           
            
            

           
        }
        }
        

    }
    [ServerRpc(RequireOwnership = false)]
    void Temp_Death_ServerRpc()
    {
        HealthBar_object.SetActive(false);
            revive_icon.SetActive(true);
            Is_alive.Value = false;
            do_death_zombie_ClientRpc();
    }
    
   
    
    
    
    [ClientRpc]
    void do_death_zombie_ClientRpc()
    {
        
       // myMouseLookScript.is_active = false;
        p_collider.excludeLayers = ignore_collsion;
        spec_cam_script.set_main_cam(false);
        spec_cam_script.spectate_text.text = "";
        Die();
     
        
    }
   
    void Die()
    {
        Player_anim_1.enabled = false;
         Player_anim_2.enabled = false; 
      //   p_input_script.enabled = false;
      spec_cam_script.can_Spectate = true;
      spec_cam_script.set_start_cam();
        p_rigid.velocity = Vector3.zero;
        HealthBar_object.SetActive(false);
        revive_icon.SetActive(true);
        deathCam.is_dead = true;
        stop_audio();
       
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void Alive_ServerRpc()
    {
        revive_icon.SetActive(false);
        HealthBar_object.SetActive(true);
        Is_alive.Value = true;
        Healthe.Value = max_healthe.Value;
         Alive_ClientRpc();
        

    }
    [ClientRpc]
    void Alive_ClientRpc()
    {
        spec_cam_script.can_Spectate =false;
        spec_cam_script.disable_all_cams();
        spec_cam_script.set_main_cam(true);
        spec_cam_script.spectate_text.text = "";
          deathCam.is_dead = false;
         deathCam.Isalive();
        
        HealthBar_object.SetActive(true);
        revive_icon.SetActive(false);
        Player_anim_1.enabled = true;
        Player_anim_2.enabled = true;
      //  p_input_script.enabled=true;
        p_collider.excludeLayers = def_mask;

    }
    
   
 
    void Update()
    {
        HealthBar.value = Healthe.Value / max_healthe.Value;
        hp.text = Healthe.Value.ToString();
        if(!IsOwner)return;
        if(elapsedtime_since_kill > 0)
        {
            elapsedtime_since_kill -= Time.deltaTime;
        }
        if(elapsedtime_since_kill <0)
        {
            if(xp_Track != null)
            {
                xp_Track.fade();
                
                
            }
            elapsedtime_since_kill = 0f;
        }
        if(Input.GetKeyDown(KeyCode.I))
        {
            Alive_ServerRpc();
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            p_collider.excludeLayers = ignore_collsion;
            Temp_Death_ServerRpc();
        }
        
        
        allow_regen();
        regen_timer_text.text = regen_timer.ToString();
        Player_Health[] players = test.GetAllPlayers();
        
        if(players_number != players.Length)
        {
            
            players_number = players.Length;
            SetNameServerRpc(Net_name.Value.ToString());
           // makelookatcam();

        }
        
        if(Healthe.Value > 0)
        {
        myhpbar.value = Healthe.Value / max_healthe.Value;
        }
        else
        {
            myhpbar.value = 0;
        }
        if(Input.GetKey(KeyCode.H))
        {
            money_man.GiveMoney_debug_ServerRpc(500);
            money_man.GiveSalvage_debug_ServerRpc(500);
        }

    }
    
    
   
   

    
   

   [ServerRpc(RequireOwnership = false)]
   public void tellallplayers_ServerRpc(string killer,string killed)
   {
    Player_Health[] players = test.GetAllPlayers();

    foreach(Player_Health Player in players)
    {
        Player.tellallplayers_ClientRpc(killer,killed);

    }

   }
   [ClientRpc]
   public void tellallplayers_ClientRpc(string killer,string killed)
   {
    if(!IsOwner)return;
    killfeedscript.OnKill(killed,killer);

   }

   [ClientRpc]
   public void scorefeed_ClientRpc(string id,bool was_explosive)
   {
    if(id == this.NetworkObjectId.ToString())
    {
    if(IsLocalPlayer)
    {
    elapsedtime_since_kill = max_kill_elapsed_time;
    if(xp_Track != null)
    {
        xp_Track.Value=xp_Track.Value+ 90;
        xp_Track.Text.fontSize = xp_Track.def_size;
        xp_Track.do_anim();
    }
    else
    {
        GameObject xp = Instantiate(total_xp,xp_panel);
        xp_Track = xp.GetComponent<Xp_tracker>();
         xp_Track.Value=xp_Track.Value+ 90;
         xp_Track.Text.fontSize = xp_Track.def_size;
        xp_Track.do_anim();
    }
    if(!active_score_feed)
    {
    active_score_feed = Instantiate(score_feed_item,score_feed_panel); 
    }
    else
    {
        if(active_score_feed.GetComponent<score_feed_ui_item>().elapsedtime>0)
        {
            active_score_feed.GetComponent<score_feed_ui_item>().Increase();
        }
        else
        {
            active_score_feed = Instantiate(score_feed_item,score_feed_panel); 
        }
    }
    if(!was_explosive)
    {
    player_audio_source.PlayOneShot(killeffect);
    }
    
        
    medal_count +=1;
    killcounter +=1;
    if(allow_medal_give)
    {
    if(!giving_medal)
    {
    //StartCoroutine(give_medal());
    }
    }
    if(!multikill_checker)
    {
        StartCoroutine(multiKill_func());
        
    }

    if(!one_shot_checker)
    {
        Debug.LogError("giving");
        StartCoroutine(one_Shot_one_kill_medal());
    }
    //
    
    
    }
    else
    {
        return;
    }
    }

   }

   IEnumerator multiKill_func()
   {
    multikill_checker=true;
    float elapsedtime =0f;
    int next_multi_killtarget = 3;
    int next_penta_killtarget = 6;
    int slaughter_killtarget = 9;
    while(elapsedtime < 5f)
    {
    if(killcounter >= next_multi_killtarget && !multikill)
    {
        next_multi_killtarget += 3;
        multikill = true;
        yield return new WaitForSeconds(0.1f);
        Debug.LogError("giving multikill_medal");
         GameObject medal_prefab = Instantiate(multi_kill_medal,medal_feed_panel);
        medal_prefab.transform.SetSiblingIndex(0);
        
        player_audio_source.PlayOneShot(medal_earn);
        
    
    }
    if(killcounter >= next_penta_killtarget && multikill && !pentakill)
    {
        next_penta_killtarget += 6;
        pentakill = true;
        yield return new WaitForSeconds(0.1f);
        Debug.LogError("giving penta_kill_medal");
         GameObject medal_prefab = Instantiate(penta_kill_medal,medal_feed_panel);
        medal_prefab.transform.SetSiblingIndex(0);
        
        player_audio_source.PlayOneShot(medal_earn);
        

    }
    if(killcounter >= slaughter_killtarget)
    {
        slaughter_killtarget += 9;
        slaughter = true;
        yield return new WaitForSeconds(0.1f);
        Debug.LogError("giving slaughter_medal");
         GameObject medal_prefab = Instantiate(slaughter_medal,medal_feed_panel);
        medal_prefab.transform.SetSiblingIndex(0);
        
        player_audio_source.PlayOneShot(medal_earn);
        

    }
     yield return null;
     elapsedtime += Time.deltaTime;
    }
    killcounter = 0;
    multikill = false;
    pentakill = false;
    slaughter = false;
    multikill_checker = false;

   }

   IEnumerator one_Shot_one_kill_medal()
   {
    one_shot_checker = true;
    Debug.LogError(shots_fired + "shotted");
    yield return new WaitForSeconds(0.35f);
    if(shots_fired == 1)
    {
        
      
        yield return new WaitForSeconds(0.25f);
        Debug.LogError("giving oneshot_medal");
        GameObject medal_prefab = Instantiate(one_shot_medal,medal_feed_panel);
        medal_prefab.transform.SetSiblingIndex(0);
        
        player_audio_source.PlayOneShot(medal_earn);
        
        shots_fired = 0;
    }
    else
    {
        shots_fired = 0;
    }
   
    

    
    
    yield return new WaitForSeconds(1.25f);
    one_shot_checker = false;
   }
   
   IEnumerator give_medal()
   {
    giving_medal = true;
    while(is_shooting)
    {
        yield return null;
    }
    yield return new WaitForSeconds(0.55f);
    Debug.LogError("giving medals");
   
    while(k != medal_count)
    {
    Debug.LogError("giving medals loop");
    Instantiate(medal_item,medal_feed_panel);
    
    player_audio_source.PlayOneShot(medal_earn);
    k++;
    yield return new WaitForSeconds(1f);


    }
    medal_count = 0;
    k=0;
    giving_medal=false;
    

   }

   
   
   
   [ServerRpc(RequireOwnership =false)]
   void Give_health_ServerRpc(float amount)
   {
    Healthe.Value += amount;

   }
   void allow_regen()
   {
    if(is_healing)
    {
     if(Healthe.Value < max_healthe.Value)
     {
     Give_health_ServerRpc(regen_speed * Time.deltaTime);
     }
     else
     {
        set_max_health_ServerRpc();
        is_healing = false;
        regen_timer = regen_timer_value;
     }
    }
   }
   [ServerRpc(RequireOwnership = false)]
   void set_max_health_ServerRpc()
   {
    Healthe.Value = max_healthe.Value;

   }
   IEnumerator health_regen_loop()
   {
    if(IsLocalPlayer)
        {
    while(true)
    {
        
        if(Healthe.Value < max_healthe.Value || Healthe.Value > max_healthe.Value)
        {
        regen_timer-=1f;
        
        if(regen_timer <=0)
        {
            if(Healthe.Value < max_healthe.Value)
            {
                is_healing = true;
           
            }
            else
            {
                is_healing = false;
                set_max_health_ServerRpc();
               
            }

        }
        }
        yield return new WaitForSeconds(1f);
    }    
        }
    
   }
   

    
}
