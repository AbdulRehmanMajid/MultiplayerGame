using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ShakeCamSphere : NetworkBehaviour
{
    public float magnitude;
    public float fadeInTime = 0f;
    public float fadeOutTime;
    public float roughness;
    public float duration;
    // Start is called before the first frame update
    void Start()
    {
        if(!IsServer)return;
        Destroy(this.gameObject,0.05f);
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(!IsServer)return;
        if(other.GetComponent<CamShakeClient>()!= null)
        {
        other.GetComponent<CamShakeClient>().Shake_ClientRPC(magnitude,roughness,fadeInTime,fadeOutTime);
        }
    }
}
