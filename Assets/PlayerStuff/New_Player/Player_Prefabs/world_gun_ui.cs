using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class world_gun_ui : MonoBehaviour
{
    public GameObject player_cam;
    public bool is_active = true;
    public GameObject world_gun_ui_canvas;
    public world_gun_data_holder gun_data;
    public TextMeshProUGUI gun_stuff;
    public string Gun_Name;

    // Update is called once per frame

    void Start()
    {
        gun_stuff.text = Gun_Name + " | " +"Level: " + gun_data.Gun_level.Value.ToString() + " | " + gun_data.Rareity_level.Value.ToString();
    }
    void FixedUpdate()
    {
        if(player_cam == null)
        {
            player_cam = GameObject.FindGameObjectWithTag("Self_cam");
        }
        if(player_cam != null && is_active)
        {
        world_gun_ui_canvas.SetActive(true);
        world_gun_ui_canvas.transform.LookAt(player_cam.transform.position);
        
        }
        else
        {
       
            world_gun_ui_canvas.SetActive(false);

        }
        is_active = false;
        
        
    }
}
