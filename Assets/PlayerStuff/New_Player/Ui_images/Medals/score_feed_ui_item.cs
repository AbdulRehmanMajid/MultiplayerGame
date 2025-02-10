using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class score_feed_ui_item : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI text;
    public float font_size;
    public int kill_streak;
   
    public float life_time = 3f;
    public float elapsedtime;
    
    void Start()
    {
         Invoke("shrink",0.05f);
        StartCoroutine(lifetime());
        
    }
    public void Increase()
    {
        elapsedtime = life_time;
        kill_streak += 1;
        text.text = "+90 Zombie Kill X" +kill_streak; 
    }
    IEnumerator lifetime()
    {
        Debug.LogError("runnning");
        elapsedtime = life_time;
        while(elapsedtime > 0)
        {
            
          

            elapsedtime -= Time.deltaTime;
            yield return null;
        }
        if(elapsedtime <0)
        {
         
       fade();
        }
       
    }
    void shrink()
    {
        StartCoroutine(shrink_font(0.01f,text));
    }
    void fade()
    {

        StartCoroutine(FadeTextToZeroAlpha(1.25f,text));
       
    }

    IEnumerator shrink_font(float t, TextMeshProUGUI i)
    {
        while(i.fontSize > font_size)
        {
            i.fontSize -= (Time.deltaTime / t);
            yield return null;
        }


    }

    
    public IEnumerator FadeTextToZeroAlpha(float t, TextMeshProUGUI i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        if(i.color.a <= 0.0f)
        {
            Debug.LogError("destroying");
            Destroy(this.gameObject);
        }
    }
}
