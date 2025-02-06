using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSize : MonoBehaviour
{
    public float FixedSize = .005f;
    public Camera Camera;

    void Update ()
    {
        if(!Camera)
        {
        var distance = (Camera.transform.position - transform.position).magnitude;
        var size = distance * FixedSize * Camera.fieldOfView;
        transform.localScale = Vector3.one * size;
        transform.forward = transform.position - Camera.transform.position;
        }
    }
}
