using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject cam;
    public GameObject target;
    

    // Update is called once per frame
    void Update()
    {
        this.transform.localEulerAngles = new Vector3(0,0,cam.transform.localEulerAngles.x);
     
        
    }
}
