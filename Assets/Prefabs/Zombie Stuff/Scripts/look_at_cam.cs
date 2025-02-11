using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class look_at_cam : MonoBehaviour
{
    public Transform p_cam;
    public bool screen_space_overlay = false;
    public Transform offset;
    public Canvas mycanvas;

    void LateUpdate()
    {
        if (screen_space_overlay)
        {
            UpdateScreenSpaceOverlay();
        }
        else
        {
            UpdateWorldLook();
        }
    }

    void UpdateWorldLook()
    {
        if (p_cam == null)
            return;

        Vector3 v = p_cam.position - transform.position;
        // Restrict rotation to only the y-axis.
        v.x = v.z = 0f;
        transform.LookAt(p_cam.position - v);
    }

    void UpdateScreenSpaceOverlay()
    {
        if (p_cam == null)
            return;

        Camera cam = p_cam.GetComponent<Camera>();
        if (cam == null)
            return;

        Vector3 pos = cam.WorldToScreenPoint(offset.position);
        transform.position = new Vector3(pos.x, pos.y, 0f);
    }
}
