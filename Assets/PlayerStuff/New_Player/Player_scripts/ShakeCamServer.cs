using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ShakeCamServer : NetworkBehaviour
{
    public float magnitude;
    public float fadeInTime = 0f;
    public float fadeOutTime;
    public float roughness;
    public float duration;
   public void OnTriggerEnter(Collider other)
   {
    if(!IsServer)return;
    if(other.GetComponent<CamShakeClient>() != null)
    {
        Debug.LogError("found");
    other.GetComponent<CamShakeClient>().Shake_ClientRPC(magnitude,roughness,fadeInTime,fadeOutTime);
    }
   }
}
