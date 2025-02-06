using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_world_ui_script : MonoBehaviour
{
    public GameObject screen_test;
    public Camera world_ui_cam;
    public GameObject ui;
    

    // Update is called once per frame
    void LateUpdate()
    {
        if(screen_test == null)
        {
            screen_test = GameObject.FindWithTag("screenspace");
        }
        if(screen_test)
        {
            ui.transform.position = world_ui_cam.WorldToScreenPoint(screen_test.transform.position);

        }
        
    }
}
