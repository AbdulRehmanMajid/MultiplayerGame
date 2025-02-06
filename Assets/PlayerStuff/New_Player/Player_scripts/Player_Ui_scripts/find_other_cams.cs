using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class find_other_cams : NetworkBehaviour
{
    // Start is called before the first frame update
   
    public int Old_player_count;
    public Transform mycam;   

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)return;
        if(test.GetAllPlayers().Length != Old_player_count)
        {
            Old_player_count = test.GetAllPlayers().Length;
            Player_Health [] lol = test.GetAllPlayers();
            foreach(Player_Health player in lol)
            {
                player.hp_shower.cam = mycam;
                
            }
        }
        
        
    }
}
