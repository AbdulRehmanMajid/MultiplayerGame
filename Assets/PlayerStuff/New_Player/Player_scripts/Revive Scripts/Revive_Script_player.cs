using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using StarterAssets;
using UnityEngine.UI;
using Unity.Mathematics;


public class Revive_Script_player : NetworkBehaviour
{
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
    

    

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)return;
        if(reviveTimer>0&& reviveTimer<revive_time_limit +1f)
        {
        smooth_rev +=Time.deltaTime;
        if(rev_progressHolder.activeSelf)
        {
        rev_progress.value = smooth_rev/(revive_time_limit-1f);
        }
        }
        if(revive_Script == null)
        {

            rev_progressHolder.SetActive(false);
            
            reviving=false;
                p_animator.SetBool("Reviveing",false);
        }
       
       
        if(revive_Script != null)
        {
            rev_progressHolder.SetActive(true);
           revive_text.text = "Revive Player";
           
           
            
            if(!revive_Script.p_Health.Is_alive.Value)
            {
                if(other_collider!=null)
                {
                    float lol = Vector3.Distance(this.gameObject.transform.position,other_collider.transform.position);
                    Debug.LogError(lol);
                    if(lol>Distance)
                    {
                        revive_Script = null;
                         CancelInvoke("Increasce_Time");
                reviveTimer = 0f;
                reviving=false;
                p_animator.SetBool("Reviveing",false);
                    }
                }
            
            if(Input.GetKey(KeyCode.F))
            {
                
                if(!reviving)
                {
                     
                    reviving=true;
                     p_animator.SetBool("Reviveing",true);
                     p_animator.Play("Revive");
                    

                Invoke("Increasce_Time",1f);

                }
               
              
                
            }
            else
            {
                rev_progress.value = 0;
                CancelInvoke("Increasce_Time");
                reviveTimer = 0f;
                smooth_rev = 0f;
                reviving=false;
                p_animator.SetBool("Reviveing",false);
            }
            if(reviveTimer >= revive_time_limit)
                {
                    Debug.LogWarning("Reveieeeeeeeeeeeeeeeeeee");
                    revive_Script.p_Health.Alive_ServerRpc();
                    rev_progress.value = 0;
                     revive_text.text = "";
                     rev_progressHolder.SetActive(false);
                     revive_Script = null;
                     p_animator.SetBool("Reviveing",false);
                     reviveTimer = 0f;


                   
                }
            
            }
            else
            {
                revive_Script = null;
                reviveTimer = 0f;
                
                

            }
         //   if(revive_Script != null)
         //   {
          //   if(Vector3.Distance(revive_Script.transform.position,this.transform.position)>10)
         //   {
          //      revive_Script = null;
           //     revive_text.text = "";
           //     reviveTimer = 0f;
           //    reviving = false;
            //    p_animator.SetBool("Reviveing",false);
            //    return;
                

           // }
          //  }
            
            
            


        }
        
    }
    void Increasce_Time()
    {
        if(revive_Script!=null)
        {
        reviveTimer+=1;
        reviving = false;
        }
    }
}
