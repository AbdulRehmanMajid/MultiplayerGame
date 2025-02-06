using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show_Hp_to_others : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cam;
    

    // Update is called once per frame
    void Update()
    {
        
        if(cam != null)
        {
            Vector3 v = cam.transform.position - transform.position;
            v.x = v.z = 0f;
            transform.LookAt(cam.transform.position - v);
        }
        
    }
}
