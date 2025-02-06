using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade : MonoBehaviour
{
    
    void Awake()
    {
        StartCoroutine(kill());
        
    }
    IEnumerator kill()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }

    
}
