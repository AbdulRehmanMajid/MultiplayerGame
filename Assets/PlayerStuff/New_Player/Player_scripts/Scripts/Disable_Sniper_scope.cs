using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Disable_Sniper_scope : NetworkBehaviour
{
    public Camera ScopeCam;
    
    public bool main_cam = false;
    public float render_dist = 200;
    public float mobile_fov = 70;
    void Start()
    {
        if(!IsOwner)return;
        ScopeCam.enabled = true;
        if(main_cam)
        {
        ScopeCam.tag = "Self_cam";
        }
        if(Application.platform == RuntimePlatform.Android)
        {
            ScopeCam.farClipPlane = render_dist;
            ScopeCam.fieldOfView = mobile_fov;
        }
        

    

        

    }
    
    
}
