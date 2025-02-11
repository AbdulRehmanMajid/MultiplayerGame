using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthBarShower : NetworkBehaviour
{
    public Camera Maincam;
    public RaycastHit rayHit;
    public float range = 60f;
    
    void Update()
    {
        if (!IsOwner)
            return;
        
        Ray ray = Maincam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out rayHit, range))
        {
            // Cache the Zomb_health_collider component
            var zombCollider = rayHit.collider.GetComponent<Zomb_health_collider>();
            if (zombCollider != null)
            {
                if (zombCollider.zombai.P_cam == null)
                    zombCollider.zombai.P_cam = Maincam.transform;

                zombCollider.zombai.show_hp_to_player = true;
            }
            
            // Cache the world_gun_ui component
            var gunUI = rayHit.collider.GetComponent<world_gun_ui>();
            if (gunUI != null)
                gunUI.is_active = true;
        }
    }
}
