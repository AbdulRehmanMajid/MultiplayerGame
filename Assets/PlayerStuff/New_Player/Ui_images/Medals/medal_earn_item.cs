using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class medal_earn_item : MonoBehaviour
{
    public RawImage medal;
    public float image_size;
    private float def_image_size;
    public Vector2 size;
    public bool test = false;
    public float t = 3f;

    // Update is called once per frame
    void Start()
    {
     Invoke("shrink",0.25f);
     
     
     
    }


    void shrink()
    {
        StartCoroutine(shrink_font(t,image_size));

    }

        
    

       IEnumerator shrink_font(float t,float image_size)
    {
        size = medal.rectTransform.sizeDelta;
        while(size.y > image_size)
        {
            size = medal.rectTransform.sizeDelta;
            size.y -= (Time.deltaTime / t);
            medal.rectTransform.sizeDelta = new Vector2(size.y,size.y);
            yield return null;
        }
        Destroy(this.gameObject,1.4f);


    }
}
