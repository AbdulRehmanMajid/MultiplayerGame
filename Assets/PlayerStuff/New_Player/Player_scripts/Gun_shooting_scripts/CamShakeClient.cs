using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using EZCameraShake;
public class CamShakeClient : NetworkBehaviour
{
    public CameraShaker cameraShake;
    [ClientRpc]
    public void Shake_ClientRPC(float magnitude,float roughness,float fadeInTime,float fadeOutTime)
    {
        if(!IsOwner)return;
        cameraShake.ShakeOnce(magnitude,roughness,fadeInTime,fadeOutTime);

    }
}
