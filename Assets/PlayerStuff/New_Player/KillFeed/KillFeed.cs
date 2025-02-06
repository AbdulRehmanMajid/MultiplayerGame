using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour
{
    [SerializeField] private GameObject killfeeditemprefab;
    [SerializeField] Transform killfeedlist;
    
    public void OnKill(string player,string source)
    {
        Debug.Log(source + "Killed" + player);
        killfeedlist.SetAsFirstSibling();
        GameObject go = Instantiate(killfeeditemprefab,killfeedlist);
        go.GetComponent<KillFeeditem>().Setup(player,source);
        Destroy(go,4f);

    }
    
}
