using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StarterAssets;
using TMPro;

public class gunMangerV2 : NetworkBehaviour
{
    #region Fields & References
    public AnimationClip changegun;
    public AudioSource player_audio_source;
    public TextMeshProUGUI gunPickUp_text;
    
    [Header("Back Guns Prefabs List")]
    public GameObject XM4_Back;
    public GameObject Gernade_launcher_back;
    public GameObject MP5_Back;
    public GameObject Rai_back;
    public GameObject Gallo_back;
    public GameObject L96_back;
    public GameObject RPG7_back;
    public GameObject BlackHole_back;
    
    [Header("World Gun Prefabs List")]
    public GameObject XM4_World;
    public GameObject Gernade_launcher_World;
    public GameObject MP5_World;
    public GameObject Rai_World;
    public GameObject Gallo_World;
    public GameObject L96_World;
    public GameObject RPG7_World;
    public GameObject BlackHole_World;
    public Transform fpscam;
    
    [Header("Guns Prefabs")]
    public GameObject XM4;
    public GameObject Gernade_launcher;
    public GameObject MP5;
    public GameObject Rai;
    public GameObject Gallo;
    public GameObject L96;
    public GameObject RPG7;
    public GameObject BlackHole;
    
    GameObject gun_to_change;
    GameObject world_gun;
    public bool in_pickup_range = false;
    public string gun_pickup_name;
    
    public Transform BackGun_holder;
    public PickUpGun gun_pick_script;
    public Aim_Transitioner aim_Transitioner;
    
    public GameObject Slot1_gun;
    public GameObject Slot2_gun;
    PickUpGun gun_to_destroy;
    
    [SerializeField] public NetworkVariable<int> Slot1_gun_id = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<int> Slot2_gun_id = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public int current_slot = 1;
    
    public Transform player_transform;
    public Transform gunHolder;
    public RecoilNew cam_recoil;
    public recoil_values rec_val;
    public Player_Health myhealthScript;
    public StarterAssetsInputs new_inputs;
    
    public bool has_changed_gun = false;
    public bool has_pressed_pick_button = false;
    public int current_gun = 1;
    public float throw_force;
    public float throw_up_force;
    bool can_buy = false;
    public GameObject current_active_gun;
    public bool Gun_Upgrder_active = false;
    public bool Gun_Rareity_active = false;
    public int gun_upgrade_cost;
    public int gun_Rareity_upgrade_cost;
    public TextMeshProUGUI gun_upgrade_text;
    bool upgraded = true;
    #endregion

    #region Unity Methods
    void Start()
    {
        Disable_guns();
       // L96.SetActive(false);
        if (!IsOwner) return;
        // Uncomment and adjust if required:
        // DisableALL_guns();
        // ChangeGun_ServerRpc("XM4");
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        // Slot1_gun_id.OnValueChanged += Change_other_slot1;
        // Slot2_gun_id.OnValueChanged += Change_other_slot2;
    }
    
    void Update()
    {
        if (!IsOwner) return;
        
        // Gun switching input
        if (new_inputs.change_gun && !has_changed_gun && !aim_Transitioner.is_drinking_perk.Value && !aim_Transitioner.is_reloading)
        {
            stop_audio();
            has_changed_gun = true;
            current_gun = (current_gun % 2) + 1;
            ChangeGun_ServerRpc(current_gun);
        }
        else if (!new_inputs.change_gun)
        {
            has_changed_gun = false;
        }
        
        // Buy button input setup
        if (new_inputs.buy_button && !has_pressed_pick_button)
        {
            has_pressed_pick_button = true;
            can_buy = true;
        }
        else if (!new_inputs.buy_button)
        {
            has_pressed_pick_button = false;
            can_buy = false;
        }
        
        // Gun rarity upgrade
        if (Gun_Rareity_active && current_active_gun != null)
        {
            int upgradeCost = gun_Rareity_upgrade_cost * current_active_gun.GetComponent<Rarity_and_level>().Rareity_level.Value;
            gun_upgrade_text.text = $"Press To Upgrade Gun Rarity Cost: {upgradeCost}";
            if (new_inputs.buy_button && !upgraded)
            {
                upgraded = true;
                Gun_Rareity_upgrade_func(upgradeCost);
            }
            else if (!new_inputs.buy_button)
            {
                upgraded = false;
            }
        }
        
        // Gun level upgrade
        if (Gun_Upgrder_active && current_active_gun != null)
        {
            int upgradeCost = gun_upgrade_cost * current_active_gun.GetComponent<Rarity_and_level>().Gun_level.Value;
            gun_upgrade_text.text = $"Press To Upgrade Gun Level Cost: {upgradeCost}";
            if (new_inputs.buy_button && !upgraded)
            {
                upgraded = true;
                Gun_upgrade_func(upgradeCost);
            }
            else if (!new_inputs.buy_button)
            {
                upgraded = false;
            }
        }
        
        if (!Gun_Upgrder_active && !Gun_Rareity_active && gun_upgrade_text.text != "")
        {
            gun_upgrade_text.text = "";
        }
        
        // Gun pickup/swap
        if (can_buy && in_pickup_range && gun_pick_script != null && !aim_Transitioner.is_drinking_perk.Value && !aim_Transitioner.is_reloading)
        {
            swapgun();
        }
    }
    #endregion

    #region RPC Methods
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
            player_audio_source.clip = null;
            player_audio_source.loop = false;
            player_audio_source.Stop();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void Set_rarity_ServerRpc(int slot)
    {
        if (gun_pick_script == null) return;
        if (slot == 1)
        {
            Slot1_gun.GetComponent<Rarity_and_level>().Gun_level.Value = gun_pick_script.gun_data.Gun_level.Value;
            Slot1_gun.GetComponent<Rarity_and_level>().Rareity_level.Value = gun_pick_script.gun_data.Rareity_level.Value;
        }
        else if (slot == 2)
        {
            Slot2_gun.GetComponent<Rarity_and_level>().Gun_level.Value = gun_pick_script.gun_data.Gun_level.Value;
            Slot2_gun.GetComponent<Rarity_and_level>().Rareity_level.Value = gun_pick_script.gun_data.Rareity_level.Value;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void DropGun_ServerRpc()
    {
        DropGun_ClientRpc();
    }
    
    [ClientRpc]
    void DropGun_ClientRpc()
    {
        if (current_slot == 1 && Slot1_gun != null)
        {
            Slot1_gun.SetActive(false);
            Slot1_gun = null;
        }
        if (current_slot == 2 && Slot2_gun != null)
        {
            Slot2_gun.SetActive(false);
            Slot2_gun = null;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void ChangeGun_ServerRpc(int number)
    {
        ChangeGun_ClientRpc(number);
    }
    
    [ClientRpc]
    void ChangeGun_ClientRpc(int number)
    {
        float animTime = changegun.length;
        StartCoroutine(wait_for_anim(animTime, number));
    }
    
    [ServerRpc(RequireOwnership = false)]
    void Set_SlotID_ServerRpc(int slot_number, int id)
    {
        if (slot_number == 1)
        {
            Slot1_gun_id.Value = id;
        }
        else if (slot_number == 2)
        {
            Slot2_gun_id.Value = id;
        }
        Debug.LogError("Picking up from SlotID ServerRpc");
    }
    
    [ServerRpc(RequireOwnership = false)]
    void Spawn_Gun_world_ServerRpc(int id, int level, int Rareity_level)
    {
        GetGuns_world(id);
        GameObject gunObj = Instantiate(world_gun, player_transform.position, Quaternion.identity);
        var gunData = gunObj.GetComponent<world_gun_data_holder>();
        gunData.Gun_level.Value = level;
        gunData.Rareity_level.Value = Rareity_level;
        
        var netObj = gunObj.GetComponent<NetworkObject>();
        netObj.Spawn();
        
        Rigidbody rb = gunObj.GetComponent<Rigidbody>();
        rb.AddForce(fpscam.forward * throw_force, ForceMode.Impulse);
        rb.AddForce(fpscam.up * throw_up_force, ForceMode.Impulse);
        float randomTorque = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(randomTorque, randomTorque, randomTorque), ForceMode.Impulse);
    }
    #endregion

    #region Gun Methods
    void GetGuns(int id)
    {
        switch (id)
        {
            case 1: gun_to_change = XM4; break;
            case 2: gun_to_change = Gernade_launcher; break;
            case 3: gun_to_change = MP5; break;
            case 4: gun_to_change = Rai; break;
            case 5: gun_to_change = Gallo; break;
            case 6: gun_to_change = L96; break;
            case 7: gun_to_change = RPG7; break;
            case 8: gun_to_change = BlackHole; break;
        }
    }
    
    void GetGuns_world(int id)
    {
        switch (id)
        {
            case 1: world_gun = XM4_World; break;
            case 2: world_gun = Gernade_launcher_World; break;
            case 3: world_gun = MP5_World; break;
            case 4: world_gun = Rai_World; break;
            case 5: world_gun = Gallo_World; break;
            case 6: world_gun = L96_World; break;
            case 7: world_gun = RPG7_World; break;
            case 8: world_gun = BlackHole_World; break;
        }
    }
    
    void Disable_guns()
    {
        XM4.SetActive(false);
        Gernade_launcher.SetActive(false);
        MP5.SetActive(false);
        Rai.SetActive(false);
        Gallo.SetActive(false);
        L96.SetActive(false);
        RPG7.SetActive(false);
        BlackHole.SetActive(false);
    }
    
    // Changes the active gun for the given slot
    void ChangeGun_slot(int number)
    {
        Disable_guns();
        GameObject gun = null;
        if (number == 1)
        {
            switch (Slot1_gun_id.Value)
            {
                case 1: XM4.SetActive(true); gun = XM4; break;
                case 2: Gernade_launcher.SetActive(true); gun = Gernade_launcher; break;
                case 3: MP5.SetActive(true); gun = MP5; break;
                case 4: Rai.SetActive(true); gun = Rai; break;
                case 5: Gallo.SetActive(true); gun = Gallo; break;
                case 6: L96.SetActive(true); gun = L96; break;
                case 7: RPG7.SetActive(true); gun = RPG7; break;
                case 8: BlackHole.SetActive(true); gun = BlackHole; break;
            }
            BackGun(Slot2_gun_id.Value);
            current_slot = 1;
        }
        else if (number == 2)
        {
            switch (Slot2_gun_id.Value)
            {
                case 1: XM4.SetActive(true); gun = XM4; break;
                case 2: Gernade_launcher.SetActive(true); gun = Gernade_launcher; break;
                case 3: MP5.SetActive(true); gun = MP5; break;
                case 4: Rai.SetActive(true); gun = Rai; break;
                case 5: Gallo.SetActive(true); gun = Gallo; break;
                case 6: L96.SetActive(true); gun = L96; break;
                case 7: RPG7.SetActive(true); gun = RPG7; break;
                case 8: BlackHole.SetActive(true); gun = BlackHole; break;
            }
            BackGun(Slot1_gun_id.Value);
            current_slot = 2;
        }
        
        if (gun != null)
        {
            cam_recoil.SetRecoil(
                gun.GetComponent<recoil_values>().gun_data.returnSpeed,
                gun.GetComponent<recoil_values>().gun_data.rotationSpeed,
                gun.GetComponent<recoil_values>().gun_data.Recoil);
                
            if (gun.GetComponent<temp_gun_script>() != null)
            {
                aim_Transitioner.is_sniper = gun.GetComponent<temp_gun_script>().gun_data.is_sniper;
            }
            get_current_active_gun();
        }
    }
    
    void BackGun(int id)
    {
        XM4_Back.SetActive(false);
        Gernade_launcher_back.SetActive(false);
        MP5_Back.SetActive(false);
        Rai_back.SetActive(false);
        Gallo_back.SetActive(false);
        L96_back.SetActive(false);
        RPG7_back.SetActive(false);
        BlackHole_back.SetActive(false);
        
        switch (id)
        {
            case 1: XM4_Back.SetActive(true); break;
            case 2: Gernade_launcher_back.SetActive(true); break;
            case 3: MP5_Back.SetActive(true); break;
            case 4: Rai_back.SetActive(true); break;
            case 5: Gallo_back.SetActive(true); break;
            case 6: L96_back.SetActive(true); break;
            case 7: RPG7_back.SetActive(true); break;
            case 8: BlackHole_back.SetActive(true); break;
        }
    }
    
    void swapgun()
    {
        gunPickUp_text.text = "";
        can_buy = false;
        if (gun_pick_script != null)
        {
            if (current_slot == 1 && Slot1_gun_id.Value != 0 && Slot1_gun != null)
            {
                stop_audio();
                if (Slot1_gun.GetComponent<Rarity_and_level>() != null)
                    Spawn_Gun_world_ServerRpc(Slot1_gun_id.Value, Slot1_gun.GetComponent<Rarity_and_level>().Gun_level.Value, Slot1_gun.GetComponent<Rarity_and_level>().Rareity_level.Value);
                else
                    Spawn_Gun_world_ServerRpc(Slot1_gun_id.Value, 0, 0);
                Set_SlotID_ServerRpc(1, gun_pick_script.Gun_id);
            }
            else if (current_slot == 2 && 
                     Slot2_gun_id.Value != 0 && Slot2_gun != null)
            {
                stop_audio();
                if (Slot2_gun.GetComponent<Rarity_and_level>() != null)
                    Spawn_Gun_world_ServerRpc(Slot2_gun_id.Value, Slot2_gun.GetComponent<Rarity_and_level>().Gun_level.Value, Slot2_gun.GetComponent<Rarity_and_level>().Rareity_level.Value);
                else
                    Spawn_Gun_world_ServerRpc(Slot2_gun_id.Value, 0, 0);
                DropGun_ServerRpc();
                Set_SlotID_ServerRpc(2, gun_pick_script.Gun_id);
            }
            Change_other_slot();
            gun_pick_script.Kill_gun_ServerRpc();
        }
    }
    
    // Called after the animation delay to actually change the gun slot
    IEnumerator wait_for_anim(float animationtime, int number)
    {
        aim_Transitioner.p_anim.Play("Put_away");
        yield return new WaitForSeconds(animationtime);
        ChangeGun_slot(number);
    }
    
    void Change_other_slot()
    {
        if(gun_pick_script != null)
        {
            gun_to_destroy = gun_pick_script;
            GetGuns(gun_pick_script.Gun_id);
            if (current_slot == 1)
            {
                Slot1_gun = gun_to_change;
                if (Slot1_gun.GetComponent<Rarity_and_level>() != null)
                {
                    Set_rarity_ServerRpc(current_slot);
                    Slot1_gun.GetComponent<Rarity_and_level>().Check_Rarity();
                }
                Set_SlotID_ServerRpc(1, gun_pick_script.Gun_id);
                ChangeGun_ServerRpc(1);
            }
            else if (current_slot == 2)
            {
                Slot2_gun = gun_to_change;
                if (Slot2_gun.GetComponent<Rarity_and_level>() != null)
                {
                    Set_rarity_ServerRpc(current_slot);
                    Slot2_gun.GetComponent<Rarity_and_level>().Check_Rarity();
                }
                Set_SlotID_ServerRpc(2, gun_pick_script.Gun_id);
                ChangeGun_ServerRpc(2);
            }
        }
    }
    
    public void get_current_active_gun()
    {
        current_active_gun = current_slot == 1 ? Slot1_gun : Slot2_gun;
    }
    #endregion

    #region Upgrade Methods
    public void Gun_upgrade_func(int cost)
    {
        if (Gun_Upgrder_active && current_active_gun != null)
        {
            if (myhealthScript.money_man.Money.Value >= cost)
            {
                myhealthScript.money_man.TakeMoney_ServerRpc(cost);
                current_active_gun.GetComponent<Rarity_and_level>().Upgrade_gun_ServerRpc();
            }
        }
    }
    
    public void Gun_Rareity_upgrade_func(int cost)
    {
        if (Gun_Rareity_active && current_active_gun != null)
        {
            if (myhealthScript.money_man.Salvage.Value >= cost)
            {
                myhealthScript.money_man.TakeSalvage_ServerRpc(cost);
                current_active_gun.GetComponent<Rarity_and_level>().Upgrade_gun_Rarity_ServerRpc();
                current_active_gun.GetComponent<Rarity_and_level>().Check_Rarity();
            }
        }
    }
    
    void stop_audio()
    {
        endSound_ServerRpc();
        player_audio_source.clip = null;
        player_audio_source.loop = false;
        player_audio_source.Stop();
    }
    #endregion
}

