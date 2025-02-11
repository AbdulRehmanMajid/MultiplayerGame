using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Collections;
using System.IO;
using TMPro;
using Invector.vCharacterController;
using Unity.VisualScripting;

public class Player_Health : NetworkBehaviour
{
    #region Fields & Network Variables
    bool low_health_exit_state;
    bool low_health_enter_state;

    [Header("Audio Clips")]
    public AudioClip Low_health_enter;
    public AudioClip low_health_loop;
    public AudioClip low_health_exit;
    public AudioClip killeffect;
    public AudioClip medal_earn;

    [Header("Health Settings")]
    public float health_warning_threshold = 30f;
    [SerializeField] public NetworkVariable<float> Healthe = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<float> max_healthe = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> Is_alive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<FixedString64Bytes> Net_name = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("UI & GameObjects")]
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
    
    [Header("Score & Medal UI")]
    public Transform score_feed_panel;
    public GameObject score_feed_item;
    public GameObject active_score_feed;
    public Transform medal_feed_panel;
    public GameObject medal_item;
    public GameObject one_shot_medal;
    public GameObject multi_kill_medal;
    public GameObject penta_kill_medal;
    public GameObject slaughter_medal;

    [Header("Audio Sources")]
    public AudioSource player_audio_source;
    public AudioSource gun_audio_source;

    [Header("Medal & Kill Settings")]
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

    [Header("Player Components")]
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
    public float elapsedtime_since_kill = 0f;
    public GameObject total_xp;
    public Transform xp_panel;
    Xp_tracker xp_Track;
    public float max_kill_elapsed_time = 3f;
    public LayerMask non_enemy_mask;
    #endregion

    #region Constants
    private const float LOW_HEALTH_EXIT_OFFSET = 0.15f;
    #endregion

    #region Unity Methods
    public override void OnNetworkSpawn()
    {
        // Always perform network-wide initialization first.
        revive_icon.SetActive(false);
        test.RegisterPlayer(this.NetworkObjectId.ToString(), this);

        // Only owner should execute the following.
        if (!IsOwner) return;
        ScreenSpaceUITracker []screenui = GameObject.FindObjectsOfType<ScreenSpaceUITracker>();
        foreach(ScreenSpaceUITracker ui in screenui)
        {
            ui.mainCamera = mymaincam;
        }

        // Mobile-specific setup.
        if (IsLocalPlayer && Application.platform == RuntimePlatform.Android)
        {
            Mobile_controls.SetActive(true);
            SetNameServerRpc("Player");
        }
        else if (Application.platform != RuntimePlatform.Android)
        {
            string path = Application.dataPath + "/username.txt";
            found_username = File.ReadAllText(path);
            Net_name.Value = File.ReadAllText(path);
            SetNameServerRpc(Net_name.Value.ToString());
        }

        // Spawn position setup.
        if (Spawn_pos == null)
        {
            GameObject pos = GameObject.FindGameObjectWithTag("Spawn");
            if (pos != null)
            {
                Spawn_pos = pos.transform;
                transform.position = Spawn_pos.position;
            }
        }

        StartCoroutine(health_regen_loop());

        // Setup zombie spawner and callbacks.
        if (zomb_spawner == null)
        {
            GameObject spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
            if (spawnerObj != null)
            {
                zomb_spawner = spawnerObj.GetComponent<shoot_spawn>();
                zomb_spawner.Round_num.OnValueChanged += Set_round;
                zomb_spawner.Zombies_left.OnValueChanged += Set_zombie;
            }
        }
        Healthe.OnValueChanged += Low_health;
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

    void Start()
    {
        if (!IsOwner)
            timerui.enabled = false;
        else
            timerui.enabled = true;
    }

    void Update()
    {
        HealthBar.value = Healthe.Value / max_healthe.Value;
        hp.text = Healthe.Value.ToString();

        if (!IsOwner) return;

        if (elapsedtime_since_kill > 0)
            elapsedtime_since_kill -= Time.deltaTime;
        else if (elapsedtime_since_kill < 0)
        {
            if (xp_Track != null)
                xp_Track.fade();
            elapsedtime_since_kill = 0f;
        }

        // For debugging: key presses to reset or kill the player.
        if (Input.GetKeyDown(KeyCode.I))
            Alive_ServerRpc();
        if (Input.GetKeyDown(KeyCode.M))
        {
            p_collider.excludeLayers = ignore_collsion;
            Temp_Death_ServerRpc();
        }

        allow_regen();
        regen_timer_text.text = regen_timer.ToString();

        // Update player count if changed.
        Player_Health[] players = test.GetAllPlayers();
        if (players_number != players.Length)
        {
            players_number = players.Length;
            SetNameServerRpc(Net_name.Value.ToString());
        }

        myhpbar.value = (Healthe.Value > 0) ? (Healthe.Value / max_healthe.Value) : 0;

        if (Input.GetKey(KeyCode.H))
        {
            money_man.GiveMoney_debug_ServerRpc(500);
            money_man.GiveSalvage_debug_ServerRpc(500);
        }
    }
    #endregion

    #region Name & Audio Management
    [ServerRpc(RequireOwnership = false)]
    public void SetNameServerRpc(string UserName)
    {
        Debug.LogWarning($"RPC reached: changing name to {UserName}");
        changeclientnameClientRpc(UserName);
        UsernameText.text = UserName;
    }

    [ClientRpc]
    private void changeclientnameClientRpc(string name)
    {
        UsernameText.text = name;
    }

    void stop_audio()
    {
        endSound_ServerRpc();
        gun_audio_source.clip = null;
        gun_audio_source.loop = false;
        gun_audio_source.Stop();
    }

    [ServerRpc(RequireOwnership = false)]
    void endSound_ServerRpc()
    {
        endSound_ClientRpc();
    }

    [ClientRpc]
    void endSound_ClientRpc()
    {
        if (!IsLocalPlayer)
        {
            gun_audio_source.clip = null;
            gun_audio_source.loop = false;
            gun_audio_source.Stop();
        }
    }
    #endregion

    #region Low Health Handling
    void Low_health(float old_val, float new_val)
    {
        if (new_val <= health_warning_threshold && !low_health_enter_state && new_val < old_val)
            StartCoroutine(low_health_courutine());

        if (new_val >= health_warning_threshold && !low_health_exit_state && low_health_enter_state)
            StartCoroutine(low_health_Exit_courutine());
    }

    IEnumerator low_health_courutine()
    {
        low_health_enter_state = true;
        player_audio_source.PlayOneShot(Low_health_enter);
        yield return new WaitForSeconds(Low_health_enter.length - LOW_HEALTH_EXIT_OFFSET);
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
    #endregion

    #region Damage & Death Management
    public void TakeDamage(float amount)
    {
        Regen_timer_set_ClientRpc();

        if (Is_alive.Value)
        {
            HealthBar.value = Healthe.Value / max_healthe.Value;
            Healthe.Value -= amount;
            Debug.LogError(Healthe.Value);
            Debug.LogWarning(Healthe.Value / max_healthe.Value);
            HealthBar.value = Healthe.Value / max_healthe.Value;
            hp.text = Healthe.Value.ToString();

            if (Healthe.Value <= 0)
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
        p_collider.excludeLayers = ignore_collsion;
        spec_cam_script.set_main_cam(false);
        spec_cam_script.spectate_text.text = "";
        Die();
    }
    [ClientRpc]
    public void Regen_timer_set_ClientRpc()

    {
        
        regen_timer =regen_timer_value;
        is_healing = false;

    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamagePlayer_ServerRpc(float amount)
    {
        Regen_timer_set_ClientRpc();

        if (Is_alive.Value)
        {
            HealthBar.value = Healthe.Value / max_healthe.Value;
            Healthe.Value -= amount;
            Debug.LogError(Healthe.Value);
            Debug.LogWarning(Healthe.Value / max_healthe.Value);
            HealthBar.value = Healthe.Value / max_healthe.Value;
            hp.text = Healthe.Value.ToString();

            if (Healthe.Value <= 0)
            {
                HealthBar_object.SetActive(false);
                revive_icon.SetActive(true);
                Is_alive.Value = false;
                do_death_player_ClientRpc();
            }
        }
    }

    [ClientRpc]
    void do_death_player_ClientRpc()
    {
        p_collider.excludeLayers = ignore_collsion;
        spec_cam_script.set_main_cam(false);
        spec_cam_script.spectate_text.text = "";
        Die();
    }

    void Die()
    {
        Player_anim_1.enabled = false;
        Player_anim_2.enabled = false;
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
        spec_cam_script.can_Spectate = false;
        spec_cam_script.disable_all_cams();
        spec_cam_script.set_main_cam(true);
        spec_cam_script.spectate_text.text = "";
        deathCam.is_dead = false;
        deathCam.Isalive();
        HealthBar_object.SetActive(true);
        revive_icon.SetActive(false);
        Player_anim_1.enabled = true;
        Player_anim_2.enabled = true;
        p_collider.excludeLayers = def_mask;
    }
    #endregion

    #region XP, Score & Medal System
    [ServerRpc(RequireOwnership = false)]
    public void tellallplayers_ServerRpc(string killer, string killed)
    {
        Player_Health[] players = test.GetAllPlayers();
        foreach (Player_Health player in players)
        {
            player.tellallplayers_ClientRpc(killer, killed);
        }
    }

    [ClientRpc]
    public void tellallplayers_ClientRpc(string killer, string killed)
    {
        if (!IsOwner) return;
        killfeedscript.OnKill(killed, killer);
    }

    [ClientRpc]
    public void scorefeed_ClientRpc(string id, bool was_explosive)
    {
        if (id == this.NetworkObjectId.ToString())
        {
            if (IsLocalPlayer)
            {
                elapsedtime_since_kill = max_kill_elapsed_time;
                if (xp_Track != null)
                {
                    xp_Track.Value += 90;
                    xp_Track.Text.fontSize = xp_Track.def_size;
                    xp_Track.do_anim();
                }
                else
                {
                    GameObject xp = Instantiate(total_xp, xp_panel);
                    xp_Track = xp.GetComponent<Xp_tracker>();
                    xp_Track.Value += 90;
                    xp_Track.Text.fontSize = xp_Track.def_size;
                    xp_Track.do_anim();
                }
                if (!active_score_feed)
                    active_score_feed = Instantiate(score_feed_item, score_feed_panel);
                else
                {
                    if (active_score_feed.GetComponent<score_feed_ui_item>().elapsedtime > 0)
                        active_score_feed.GetComponent<score_feed_ui_item>().Increase();
                    else
                        active_score_feed = Instantiate(score_feed_item, score_feed_panel);
                }
                if (!was_explosive)
                    player_audio_source.PlayOneShot(killeffect);

                medal_count += 1;
                killcounter += 1;
                if (allow_medal_give && !giving_medal)
                {
                    // Uncomment the following line to enable generic medal awarding
                    // StartCoroutine(give_medal());
                }
                if (!multikill_checker)
                    StartCoroutine(multiKill_func());
                if (!one_shot_checker)
                {
                    Debug.LogError("giving oneshot medal");
                    StartCoroutine(one_Shot_one_kill_medal());
                }
            }
        }
    }

    /// <summary>
    /// Helper method to award a medal: instantiates the prefab at the appropriate panel and plays the award sound.
    /// </summary>
    private void AwardMedalImmediate(GameObject medalPrefab, string debugMessage)
    {
        Debug.LogError(debugMessage);
        GameObject medalInstance = Instantiate(medalPrefab, medal_feed_panel);
        medalInstance.transform.SetSiblingIndex(0);
        player_audio_source.PlayOneShot(medal_earn);
    }

    IEnumerator multiKill_func()
    {
        multikill_checker = true;
        float elapsedtime = 0f;
        int nextMultiKillTarget = 3;
        int nextPentaKillTarget = 6;
        int nextSlaughterTarget = 9;

        while (elapsedtime < 5f)
        {
            if (killcounter >= nextMultiKillTarget && !multikill)
            {
                nextMultiKillTarget += 3;
                multikill = true;
                yield return new WaitForSeconds(0.1f);
                AwardMedalImmediate(multi_kill_medal, "Multi Kill Medal Awarded");
            }
            if (killcounter >= nextPentaKillTarget && multikill && !pentakill)
            {
                nextPentaKillTarget += 6;
                pentakill = true;
                yield return new WaitForSeconds(0.1f);
                AwardMedalImmediate(penta_kill_medal, "Penta Kill Medal Awarded");
            }
            if (killcounter >= nextSlaughterTarget && !slaughter)
            {
                nextSlaughterTarget += 9;
                slaughter = true;
                yield return new WaitForSeconds(0.1f);
                AwardMedalImmediate(slaughter_medal, "Slaughter Medal Awarded");
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
        Debug.LogError(shots_fired + " shotted");
        yield return new WaitForSeconds(0.35f);
        if (shots_fired == 1)
        {
            yield return new WaitForSeconds(0.25f);
            AwardMedalImmediate(one_shot_medal, "One Shot Medal Awarded");
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
        while (is_shooting)
            yield return null;
        yield return new WaitForSeconds(0.55f);
        Debug.LogError("giving medals");
        while (k < medal_count)
        {
            Debug.LogError("giving generic medal");
            AwardMedalImmediate(medal_item, "Generic Medal Awarded");
            k++;
            yield return new WaitForSeconds(1f);
        }
        medal_count = 0;
        k = 0;
        giving_medal = false;
    }
    #endregion

    #region Health Regeneration
    [ServerRpc(RequireOwnership = false)]
    void Give_health_ServerRpc(float amount)
    {
        Healthe.Value += amount;
    }

    void allow_regen()
    {
        if (is_healing)
        {
            if (Healthe.Value < max_healthe.Value)
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
        if (IsLocalPlayer)
        {
            while (true)
            {
                if (Healthe.Value != max_healthe.Value)
                {
                    regen_timer -= 1f;
                    if (regen_timer <= 0)
                    {
                        if (Healthe.Value < max_healthe.Value)
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
    #endregion
}
