using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthBarShower : NetworkBehaviour
{
    public Camera Maincam;
    public float range = 60f;

    void Update()
    {
        if (!IsOwner)
            return;

        // Cast a ray from the center of the screen.
        Ray ray = Maincam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            ActivateZombieHealthBar(hit.collider);
            ActivateGunUI(hit.collider);
        }
    }

    void ActivateZombieHealthBar(Collider col)
    {
        // Use TryGetComponent to minimize performance overhead.
        if (col.TryGetComponent<Zomb_health_collider>(out var zombCollider))
        {
            if (zombCollider.zombai.P_cam == null)
                zombCollider.zombai.P_cam = Maincam.transform;
                
            zombCollider.zombai.show_hp_to_player = true;
        }
    }

    void ActivateGunUI(Collider col)
    {
        if (col.TryGetComponent<world_gun_ui>(out var gunUI))
            gunUI.is_active = true;
    }
}
