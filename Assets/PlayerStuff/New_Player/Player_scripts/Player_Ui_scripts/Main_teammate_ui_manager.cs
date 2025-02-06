using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Main_teammate_ui_manager : NetworkBehaviour
{
    public int Old_player_count;
    public Player_Health my_player_health;
    public GameObject team_ui_prefab;
    public Transform team_ui_panel;


    // Update is called once per frame
    void Start()
    {
        if(!IsOwner)return;
        GameObject lol = Instantiate(team_ui_prefab,team_ui_panel);
        lol.GetComponent<Teammates_ui_hud>().player_id = my_player_health.NetworkObjectId;

    }
    void Update()
    {
        if(!IsOwner)return;
        if(test.GetAllPlayers().Length != Old_player_count)
        {
            Old_player_count = test.GetAllPlayers().Length;
            Player_Health [] players = test.GetAllPlayers();
            foreach(Player_Health player in players)
            {
                if(player.NetworkObjectId != my_player_health.NetworkObjectId)
                {
                     GameObject lol = Instantiate(team_ui_prefab,team_ui_panel);
                     lol.GetComponent<Teammates_ui_hud>().player_id = player.NetworkObjectId;

                    
                }
            }

        }
        
    }
}
