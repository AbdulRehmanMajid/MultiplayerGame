using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class LookAtCam : NetworkBehaviour
{
    public Camera cam;
    public Transform Canvas;
    public GameObject healthbar;
    public TextMeshProUGUI username;
    
    
    void Update()
    {
        if(IsOwner)return;
        if(cam != null)
        {
        Canvas.transform.LookAt(cam.transform);
//username.transform.LookAt(cam.transform);
        }

        
    }

    
}
