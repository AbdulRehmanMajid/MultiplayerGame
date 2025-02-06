using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class show_rev_to_other : MonoBehaviour
{
    public Show_Hp_to_others hp;

    // Update is called once per frame
   public Transform cam;
    

    // Update is called once per frame
    void Update()
    {
        if(cam == null)
        {
            cam= hp.cam;
        }
        
        if(cam != null)
        {
            Vector3 v = cam.transform.position - transform.position;
            v.x = v.z = 0f;
            transform.LookAt(cam.transform.position - v);
        }
        
    }
}
