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
        if (!IsOwner)
            return;
        Spec_cam.enabled = false;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (can_Spectate)
        {
            int playerCount = test.GetAllPlayers().Length;
            if (playerCount != old_player_count)
            {
                old_player_count = playerCount;
                if (playerCount > 1)
                {
                    get_cam_index();
                }
            }
            Switch_cam();
        }
    }

    // Updates the list of spectator cameras based on current players.
    void get_cam_index()
    {
        if (!IsOwner)
            return;

        Player_Health[] players = test.GetAllPlayers();
        // Clean up any null references.
        p_spec_cams.RemoveAll(cam => cam == null);
        // Add each player's spectator camera.
        foreach (Player_Health player in players)
        {
            p_spec_cams.Add(player.spec_cam_script);
        }
    }

    public void disable_all_cams()
    {
        if (!IsOwner)
            return;

        if (p_spec_cams.Count > 0)
        {
            foreach (Spectator_cam specCam in p_spec_cams)
            {
                if (specCam != null)
                {
                    specCam.Spec_cam.enabled = false;
                    specCam.my_audio_listner.enabled = false;
                }
            }
        }
    }

    void Switch_cam()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.E) && p_spec_cams.Count > 0)
        {
            disable_all_cams();

            if (current_cam_index >= p_spec_cams.Count)
                current_cam_index = 0;

            Spectator_cam targetCam = p_spec_cams[current_cam_index];
            if (targetCam != null)
            {
                targetCam.Spec_cam.enabled = true;
                targetCam.my_audio_listner.enabled = true;
                spectate_text.text = "Spectating: " + targetCam.myhealthscript.Net_name.Value.ToString();
            }
            current_cam_index++;
        }
    }

    public void set_main_cam(bool state)
    {
        if (!IsOwner)
            return;

        main_cam.enabled = state;
        AudioListener mainListener = main_cam.GetComponent<AudioListener>();
        if (mainListener != null)
            mainListener.enabled = state;
    }

    public void set_start_cam()
    {
        if (!IsOwner)
            return;

        // Update the spectator camera list.
        get_cam_index();
        if (p_spec_cams.Count > 0)
        {
            disable_all_cams();
            Spectator_cam startCam = p_spec_cams[0];
            if (startCam != null)
            {
                startCam.Spec_cam.enabled = true;
                startCam.my_audio_listner.enabled = true;
                spectate_text.text = "Spectating: " + startCam.myhealthscript.Net_name.Value.ToString();
            }
        }
    }
}
