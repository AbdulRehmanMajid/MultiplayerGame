using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class world_ui_look_stuff : MonoBehaviour
{
     public GameObject player_cam;
     public bool enable_z_axis = false;
    // Start is called before the first frame update
    

    // Update is called once per frame
     void FixedUpdate()
    {
        if(player_cam == null)
        {
            player_cam = GameObject.FindGameObjectWithTag("Self_cam");
        }
        if(player_cam && !enable_z_axis)
        {
             Vector3 v = player_cam.transform.position - transform.position;
            v.x = v.z = 0f;
        transform.LookAt(player_cam.transform.position-v);
        }
        if(player_cam && enable_z_axis)
        {
           
        transform.LookAt(player_cam.transform.position);
        }

    }
}
