using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
public class Spectator_cam : NetworkBehaviour
{
   int old_player_count = 0;
   public Camera Spec_cam;
   public AudioListener my_audio_listner;
   public Camera main_cam;
   public int current_cam_index;
   public List<Spectator_cam> p_spec_cams = new List<Spectator_cam>();
   public bool can_Spectate = false;
   public TextMeshProUGUI spectate_text;
   public Player_Health myhealthscript;
  

    void Start()
    {
        
            Spec_cam.enabled = false;

        
    }
    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)return;
        if(can_Spectate)
        {
        if(test.GetAllPlayers().Length != old_player_count)
        {
            
            old_player_count = test.GetAllPlayers().Length;
            if(test.GetAllPlayers().Length>1)
            {
            get_cam_index();
            }
        }
        Switch_cam();
        }
        
    }

    void get_cam_index()
    {
        if(!IsOwner)return;
        Player_Health [] players = test.GetAllPlayers();
       
        p_spec_cams.RemoveAll(x => !x);
        foreach(Player_Health player in players)
        {
            p_spec_cams.Add(player.spec_cam_script);
            

        }
    }

    public void disable_all_cams()
    {
         if(!IsOwner)return;
        if(p_spec_cams.Count >0)
        {
             foreach(Spectator_cam p_spec_cam in p_spec_cams)
        {
             p_spec_cam.Spec_cam.enabled = false;
             p_spec_cam.my_audio_listner.enabled = false;
        }

        }
    }
    void Switch_cam()
    {
         if(!IsOwner)return;
        
        if(Input.GetKeyDown(KeyCode.E)&& p_spec_cams.Count > 0 )
        {
            disable_all_cams();
            if(current_cam_index >= p_spec_cams.Count)
        {
            current_cam_index = 0;
             p_spec_cams[current_cam_index].Spec_cam.enabled = true;
              p_spec_cams[current_cam_index].my_audio_listner.enabled = true;
             spectate_text.text =  "Spectating: " + p_spec_cams[current_cam_index].myhealthscript.Net_name.Value.ToString();
        }
        else
        {
            p_spec_cams[current_cam_index].Spec_cam.enabled = true;
            p_spec_cams[current_cam_index].my_audio_listner.enabled = true;
               spectate_text.text = "Spectating: " + p_spec_cams[current_cam_index].myhealthscript.Net_name.Value.ToString();
        }
           
            
            current_cam_index++;
        }
    }
    public void set_main_cam(bool state)
    {
        if(!IsOwner)return;
        main_cam.enabled = state;
        main_cam.GetComponent<AudioListener>().enabled = state;
    }
    public void set_start_cam()
    {
         if(!IsOwner)return;
        Player_Health [] players = test.GetAllPlayers();
       
        p_spec_cams.RemoveAll(x => !x);
        foreach(Player_Health player in players)
        {
            p_spec_cams.Add(player.spec_cam_script);
            

        }
        if(p_spec_cams.Count>0)
        {
        disable_all_cams();
        
         p_spec_cams[0].Spec_cam.enabled = true;
         p_spec_cams[0].my_audio_listner.enabled = true;
         spectate_text.text =  "Spectating: " + p_spec_cams[0].myhealthscript.Net_name.Value.ToString();
        }
    }
    
}
