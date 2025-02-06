using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillFeeditem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text;

    public void Setup(string player, string source)
    {
        Text.text = "<b>" + source + "</b>" + " Killed " + "<i>" + player + "</i>";
    }
}
