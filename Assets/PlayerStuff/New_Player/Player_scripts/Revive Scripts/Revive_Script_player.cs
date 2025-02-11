using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using StarterAssets;
using UnityEngine.UI;

public class Revive_Script_player : NetworkBehaviour
{
    #region Public Fields
    public Revive_Script revive_Script;
    public TextMeshProUGUI revive_text;
    public bool reviving;
    public float reviveTimer;
    public Animator p_animator;
    public StarterAssetsInputs new_inputs;
    public Transform other_collider;
    public float Distance = 3f;
    public float revive_time_limit = 10f;
    public float smooth_rev = 0f;
    public Slider rev_progress;
    public GameObject rev_progressHolder;
    #endregion

    #region Unity Methods
    void Update()
    {
        if (!IsOwner) return;
        
        // If no valid revive_Script exists, disable UI & reset revive state.
        if (revive_Script == null)
        {
            DisableRevive();
            return;
        }
        
        // Activate revive UI and set initial text.
        rev_progressHolder.SetActive(true);
        revive_text.text = "Revive Player";
        
        // If the target player is not alive.
        if (!revive_Script.p_Health.Is_alive.Value)
        {
            // Check distance between this player and the target's collider.
            if (other_collider != null)
            {
                float dist = Vector3.Distance(transform.position, other_collider.position);
                Debug.LogError(dist);
                if(dist > Distance)
                {
                    revive_Script = null;
                    ResetRevive();
                    return;
                }
            }
            
            // Process user input for reviving.
            if (Input.GetKey(KeyCode.F))
            {
                if (!reviving)
                {
                    reviving = true;
                    p_animator.SetBool("Reviveing", true);
                    p_animator.Play("Revive");
                    Invoke("Increasce_Time", 1f);
                }
            }
            else
            {
                ResetRevive();
            }
            
            // Revive complete – execute revive actions.
            if (reviveTimer >= revive_time_limit)
            {
                Debug.LogWarning("Reveieeeeeeeeeeeeeeeeeee");
                revive_Script.p_Health.Alive_ServerRpc();
                ResetRevive();
                revive_text.text = "";
                rev_progressHolder.SetActive(false);
                revive_Script = null;
                return;
            }
        }
        else // If target is alive, cancel revive.
        {
            revive_Script = null;
            ResetRevive();
        }
        
        // Update progress slider if revive is underway.
        if (reviveTimer > 0 && reviveTimer < revive_time_limit + 1f && rev_progressHolder.activeSelf)
        {
            smooth_rev += Time.deltaTime;
            rev_progress.value = smooth_rev / (revive_time_limit - 1f);
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Increases the revive timer by 1 second (Invoked repeatedly while F key is held).
    /// </summary>
    void Increasce_Time()
    {
        if (revive_Script != null)
        {
            reviveTimer += 1f;
            reviving = false;
        }
    }

    /// <summary>
    /// Resets revive-related variables and cancels any pending invokes.
    /// </summary>
    void ResetRevive()
    {
        if (rev_progress != null)
            rev_progress.value = 0f;
        
        CancelInvoke(nameof(Increasce_Time));

        reviveTimer = 0f;
        smooth_rev = 0f;
        reviving = false;

        if (p_animator != null)
            p_animator.SetBool("Reviveing", false);
    }

    /// <summary>
    /// Disables the revive UI and resets the revive state.
    /// </summary>
    void DisableRevive()
    {
        if (rev_progressHolder != null)
            rev_progressHolder.SetActive(false);

        reviving = false;

        if (p_animator != null)
            p_animator.SetBool("Reviveing", false);

        ResetRevive();
    }
    #endregion
}
