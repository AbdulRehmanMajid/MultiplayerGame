using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class look_at_cam : MonoBehaviour
{
    public Transform p_cam;
    public bool screen_space_overlay = false;
    public Transform offset;
    public Canvas mycanvas;
    

    // Update is called once per frame
    void LateUpdate()
    {
        if(!screen_space_overlay)
        {
        if(p_cam != null)
        {
            Vector3 v = p_cam.transform.position - transform.position;
            v.x = v.z = 0f;
        transform.LookAt(p_cam.transform.position-v);
        }
       
        }
       
        if(screen_space_overlay && p_cam != null)
        {
            Vector3 pos = p_cam.GetComponent<Camera>().WorldToScreenPoint(offset.position);
            this.transform.position = new Vector3(pos.x,pos.y,0f);
          
       // transform.forward = transform.position - camera.transform.position;
           

        }
        
        
        
    }
}
