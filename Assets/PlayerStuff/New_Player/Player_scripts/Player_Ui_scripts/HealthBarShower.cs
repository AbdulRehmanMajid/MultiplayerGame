using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class HealthBarShower : NetworkBehaviour
{
    // Start is called before the first frame update
    public Camera Maincam;
    public RaycastHit rayHit;
    public float range = 60f;
    
    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)return;
        
        Ray ray = Maincam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));
         if (Physics.Raycast(ray, out rayHit, range))
        {
            
            if(rayHit.collider.GetComponent<Zomb_health_collider>())
            {
                if(rayHit.collider.GetComponent<Zomb_health_collider>().zombai.P_cam == null)
                {
                     rayHit.collider.GetComponent<Zomb_health_collider>().zombai.P_cam = Maincam.transform;

                }
               
                rayHit.collider.GetComponent<Zomb_health_collider>().zombai.show_hp_to_player = true;
            }
            if(rayHit.collider.GetComponent<world_gun_ui>())
            {
                rayHit.collider.GetComponent<world_gun_ui>().is_active = true;

            }
           

        }
        
    }
}
