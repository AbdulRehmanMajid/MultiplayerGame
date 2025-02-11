using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Animations.Rigging;
using StarterAssets;

public class Aim_Transitioner : NetworkBehaviour
{
    #region Private Fields
    float default_fov;
    float default_offset;
    float default_height;
    float default_Distance;

    bool old_AimState = false;
    public bool is_reloading = false;
    bool current_aim_state = false;
    bool is_scoped = false;
    bool has_changed_fpp = false;
    #endregion

    #region Public Fields
    public Camera Main_cam;
    public bool is_sniper = false;
    public bool right_sholder = false;
    public bool _isAiming = false;
    public bool test_val = false;
    public vThirdPersonCamera camlooker;
    public float aim_speed;
    public GameObject scope_overlay;
    public Aim_transition_data aim_data;
    public GameObject crosshair;
    public Animator p_anim;
    public AvatarMask temp_mask;
    public bool SetWeight = false;
    public StarterAssetsInputs new_inputs;
    public bool has_changed_shoulder = false;
    public MultiAimConstraint aim_constraint;
    public MultiAimConstraint head_constraint;
    public MultiAimConstraint r_shoulder, l_shoulder;
    public NetworkVariable<bool> Is_Aiming = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> is_drinking_perk = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public float scope_in_time;
    public float zoom_fov = 100f;
    #endregion

    #region Unity Methods
    public override void OnNetworkSpawn()
    {
        Is_Aiming.OnValueChanged += On_aim_stateChange;
    }

    void Start()
    {
        // Initialize constraint weights.
        aim_constraint.weight = 0;
        r_shoulder.weight = 0;
        l_shoulder.weight = 0;
        head_constraint.weight = 1;

        if (!IsOwner)
            return;

        default_fov = Main_cam.fieldOfView;
        default_Distance = camlooker.defaultDistance;
        default_height = camlooker.height;
        default_offset = camlooker.rightOffset;

        if (Application.platform == RuntimePlatform.Android)
        {
            temp_mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Body, false);
        }
    }

    void Update()
    {
        if (!IsOwner)
            return;

        UpdateAimConstraint();
        ProcessShoulderChange();
        ProcessSniperZoom();
        ProcessFPPToggle();
        ProcessFPPAdjustments();
        ProcessAimState();
    }
    #endregion

    #region Private Methods
    void UpdateAimConstraint()
    {
        // Update aim constraint based on network variables and not reloading.
        if ((Is_Aiming.Value || is_drinking_perk.Value) && !is_reloading)
            aim_constraint.weight = 1;
        else if (!Is_Aiming.Value && !is_drinking_perk.Value && !is_reloading)
            aim_constraint.weight = 0;
    }

    void ProcessShoulderChange()
    {
        if (new_inputs.change_sholder && !has_changed_shoulder)
        {
            has_changed_shoulder = true;
            right_sholder = !right_sholder;
        }
        else if (!new_inputs.change_sholder)
        {
            has_changed_shoulder = false;
        }
    }

    void ProcessSniperZoom()
    {
        if (is_sniper && new_inputs.aim)
        {
            Main_cam.fieldOfView = Mathf.Lerp(Main_cam.fieldOfView, zoom_fov, Time.deltaTime * scope_in_time);
            if (Main_cam.fieldOfView <= zoom_fov + 2.3f)
            {
                scope_overlay.SetActive(true);
                is_scoped = true;
            }
        }
        else // When not sniper aiming.
        {
            is_scoped = false;
            if (scope_overlay.activeSelf)
                scope_overlay.SetActive(false);

            if (Main_cam.fieldOfView < default_fov)
            {
                Main_cam.fieldOfView = Mathf.Lerp(Main_cam.fieldOfView, default_fov, Time.deltaTime * scope_in_time);
                if (Main_cam.fieldOfView >= default_fov - 0.01f)
                    Main_cam.fieldOfView = default_fov;
            }
        }
    }

    void ProcessFPPToggle()
    {
        if (new_inputs.fpp_mode && !has_changed_fpp)
        {
            aim_data.first_person_mode = !aim_data.first_person_mode;
            has_changed_fpp = true;
        }
        else if (!new_inputs.fpp_mode && has_changed_fpp)
        {
            has_changed_fpp = false;
            temp_mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Body, true);
        }
    }

    void ProcessFPPAdjustments()
    {
        if (aim_data.first_person_mode)
        {
            temp_mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Body, false);
            camlooker.rightOffset = Mathf.Lerp(camlooker.rightOffset, aim_data.fpp_offset, aim_speed * Time.deltaTime);
            camlooker.defaultDistance = Mathf.Lerp(camlooker.defaultDistance, aim_data.fpp_distance, aim_speed * Time.deltaTime);
            camlooker.height = Mathf.Lerp(camlooker.height, aim_data.fpp_height, aim_speed * Time.deltaTime);
        }
    }
    public void Reload()
    {
        StartCoroutine(Drink_Perk());
    }

    void ProcessAimState()
    {
        if (new_inputs.aim || _isAiming || new_inputs.shoot)
        {
            current_aim_state = true;
            if (!is_drinking_perk.Value)
            {
                if (current_aim_state != old_AimState)
                {
                    setAimState_ServerRpc(true);
                    old_AimState = current_aim_state;
                }
                p_anim.SetBool("Aim", true);
                if (!aim_data.first_person_mode)
                {
                    camlooker.defaultDistance = Mathf.Lerp(camlooker.defaultDistance, aim_data.distance, aim_speed * Time.deltaTime);
                    camlooker.height = Mathf.Lerp(camlooker.height, aim_data.height, aim_speed * Time.deltaTime);
                }
                if (right_sholder && !aim_data.first_person_mode)
                    camlooker.rightOffset = Mathf.Lerp(camlooker.rightOffset, aim_data.offset, aim_speed * Time.deltaTime);
                else if (!aim_data.first_person_mode)
                    camlooker.rightOffset = Mathf.Lerp(camlooker.rightOffset, -aim_data.offset, aim_speed * Time.deltaTime);
                
                if (!is_sniper)
                    crosshair.SetActive(true);
            }
        }
        else
        {
            current_aim_state = false;
            if (!is_drinking_perk.Value)
            {
                if (current_aim_state != old_AimState)
                {
                    setAimState_ServerRpc(false);
                    old_AimState = current_aim_state;
                }
                p_anim.SetBool("Aim", false);
                if (!aim_data.first_person_mode)
                {
                    camlooker.defaultDistance = Mathf.Lerp(camlooker.defaultDistance, default_Distance, aim_speed * Time.deltaTime);
                    camlooker.height = Mathf.Lerp(camlooker.height, default_height, aim_speed * Time.deltaTime);
                    if (right_sholder)
                        camlooker.rightOffset = Mathf.Lerp(camlooker.rightOffset, default_offset, aim_speed * Time.deltaTime);
                    else
                        camlooker.rightOffset = Mathf.Lerp(camlooker.rightOffset, -default_offset, aim_speed * Time.deltaTime);
                    
                    crosshair.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region Coroutines
    IEnumerator Drink_Perk()
    {
        aim_constraint.weight = 0;
        is_reloading = true;
        p_anim.speed = 1.35f;
        yield return new WaitForSeconds(0.15f);

        p_anim.SetBool("Aim", false);
        setPerk_Drink_State_ServerRpc(true);
        p_anim.SetBool("drink", true);
        yield return new WaitForSeconds(2f);
        p_anim.SetBool("drink", false);
        yield return new WaitForSeconds(0.2f);
        setPerk_Drink_State_ServerRpc(false);
        p_anim.speed = 1f;
        is_reloading = false;
    }

    IEnumerator do_scope()
    {
        yield return new WaitForSeconds(0.2f);
        scope_overlay.SetActive(true);
    }
    #endregion

    #region Network RPC Methods
    [ServerRpc(RequireOwnership = false)]
    void setWeight_ServerRpc(float value)
    {
        setWeight_ClientRpc(value);
    }

    [ClientRpc]
    void setWeight_ClientRpc(float value)
    {
        p_anim.SetLayerWeight(1, value);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void setAimState_ServerRpc(bool state)
    {
        Is_Aiming.Value = state;
    }

    [ServerRpc(RequireOwnership = false)]
    public void setPerk_Drink_State_ServerRpc(bool state)
    {
        is_drinking_perk.Value = state;
    }
    #endregion

    #region Event Handlers
    public void On_aim_stateChange(bool previous, bool new_state)
    {
        if (new_state)
        {
            r_shoulder.weight = 1;
            l_shoulder.weight = 1;
            aim_constraint.weight = 1;
        }
        else
        {
            r_shoulder.weight = 0;
            l_shoulder.weight = 0;
            aim_constraint.weight = 0;
        }
    }
    #endregion
}
