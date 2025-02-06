using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Teammates_ui_hud : MonoBehaviour
{
    public ulong player_id;
    public Slider Player_hp;
    public TextMeshProUGUI Player_money;
    public TextMeshProUGUI Player_name;
    public Player_Health target_player;
    

    // Update is called once per frame
    void Update()
    {
        if(target_player == null)
        {
            if(test.GetPlayer("Player"+player_id.ToString()))
            {
            target_player = test.GetPlayer("Player"+player_id.ToString());
            }
            else
            {
                Destroy(this.gameObject);
            }

        }
        if(target_player)
        {
            Player_hp.value =  target_player.Healthe.Value / target_player.max_healthe.Value;
            Player_name.text = target_player.Net_name.Value.ToString();
            Player_money.text = target_player.money_man.Money.Value.ToString()+"$";
        }

        
    }
}
