using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uvScroll : MonoBehaviour
{
    public float Scrollx;
    public float Scrolly;
    

    
    void Update()
    {
        float OffsetX = Time.time * Scrollx;
        float offsetY = Time.time * Scrolly;
        
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX,offsetY);
        
        if(GetComponent<Renderer>().material.mainTextureOffset.y >= 9999)
        {
            
            GetComponent<Renderer>().material.mainTextureOffset += new Vector2(OffsetX,-9999);
            

        }
        
        
    }
}
