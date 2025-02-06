using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class test : MonoBehaviour
{
    public static test instance;
    private static Dictionary<string,Player_Health> Players = new Dictionary<string, Player_Health>();
   
    private const string PLAYER_ID_PREFIX = "Player";

    private KillFeed feed;
    public void OnPlayerKilledCallback(string player,string source)
    {
       feed = FindObjectOfType<KillFeed>();
       feed.OnKill(player,source);
    }

    
    
    
    public static void RegisterPlayer(string _netID, Player_Health _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        Players.Add(_playerID,_player);
        _player.transform.name = _playerID;
        



    }

    public static void UnregisterPlayer(string _playerID)
    {
        Players.Remove(_playerID);
        


    }

    public static Player_Health GetPlayer (string _playerID)
    {
        return Players[_playerID];
    }
    
    // void OnGUI(){
    //     GUILayout.BeginArea(new Rect(200,200,200,500));
    //     GUILayout.BeginVertical();
    //     foreach ( string _playerID in Players.Keys)
    //     {
    //          GUILayout.Label(_playerID + "  -  " + Players[_playerID].transform.name);
    //     }

    //     GUILayout.EndVertical();
    //     GUILayout.EndArea();
    // }

    //void Update()
   // {
        
    //}


    public static Player_Health[] GetAllPlayers ()
    {
       return Players.Values.ToArray();
    }

   


   
}
