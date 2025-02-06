using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSpaceUi_Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Main_cam;
    public Vector3 offset;
    public Transform Target;
    public Image TargetImage;
    public Renderer m_Renderer;

    // Update is called once per frame
    void LateUpdate()
    {
        if(Main_cam!=null)
        {
            if(m_Renderer.isVisible)
            {
                Debug.LogError("Visible");
            }
            
        Vector3 direction = (Target.position - Main_cam.transform.position).normalized;
        bool isBehind = Vector3.Dot(direction,Main_cam.transform.forward) <= 0.0f;
     TargetImage.enabled = !isBehind;
        transform.position = Main_cam.WorldToScreenPoint(Target.position + offset);
        }
        else
        {
            Main_cam = GameObject.FindGameObjectWithTag("Self_cam").GetComponent<Camera>();
        }
        
    }
}
