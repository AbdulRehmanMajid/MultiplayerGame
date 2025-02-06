using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Script : MonoBehaviour
{
    public GameObject Melee_weapon;
    public Animator Melee_anim;
    
    public float attack_time;
  
    public bool can_attack = true;
    public Player_Health myhealthscript;
    public bool enable = false;
    public attack_object_Script attack_object;
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && can_attack)
        {
            StartCoroutine(start_attack());

        }
        
    }
   
    IEnumerator start_attack()
    {
        Melee_anim.SetBool("attack",true);
        
        can_attack = false;
        yield return new WaitForSeconds(attack_time);
        can_attack = true;
        
         Melee_anim.SetBool("attack",false);

    }
    public void enable_attack()
    {
        attack_object.enable = true;

    }
    public void disable_attack()
    {
        attack_object.enable = false;
    }
}
