using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class Xp_tracker : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public int CountFPS = 30;
    public float Duration = 1f;
    public string NumberFormat = "N0";
    private int _value;
    public float animation_text_size;
    public float def_size;
     public float disappear_max;
    public float disappeartimer;
    public float scale_increase;
    public int Value
   
    {
        get
        {
            return _value;
        }
        set
        {
            UpdateText(value);
            _value = value;
        }
    }
    private Coroutine CountingCoroutine;

    private void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
        def_size = Text.fontSize;
          StartCoroutine(shrink_font());
        
    }

    private void UpdateText(int newValue)
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }

        CountingCoroutine = StartCoroutine(CountText(newValue));
    }
    public void do_anim()
    {
        disappeartimer = disappear_max;
      

        
    }
    IEnumerator shrink_font()
    {
        while(true)
        {
        if(disappeartimer > disappear_max * .5f)
        {
            
            Text.fontSize  += scale_increase *Time.deltaTime;

        }
        else
        {
            if(disappeartimer>0)
            {
            Text.fontSize -= scale_increase *Time.deltaTime;
            }

        }
        if(disappeartimer>0)
        {
        disappeartimer -=Time.deltaTime;
        }
        if(disappeartimer<0)
        {
            disappeartimer=0;
        }
        yield return new WaitForSeconds(0.01f);
        }


    }

    private IEnumerator CountText(int newValue)
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
        int previousValue = _value;
        int stepAmount;

        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = -20, previousValue = 0. CountFPS = 30, and Duration = 1; (-20- 0) / (30*1) // -0.66667 (ceiltoint)-> 0
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = 20, previousValue = 0. CountFPS = 30, and Duration = 1; (20- 0) / (30*1) // 0.66667 (floortoint)-> 0
        }

        if (previousValue < newValue)
        {
            while(previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue)
                {
                    previousValue = newValue;
                }

                Text.SetText(previousValue.ToString(NumberFormat));

                yield return Wait;
            }
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount; // (-20 - 0) / (30 * 1) = -0.66667 -> -1              0 + -1 = -1
                if (previousValue < newValue)
                {
                    previousValue = newValue;
                }

                Text.SetText(previousValue.ToString(NumberFormat));

                yield return Wait;
            }
        }
    }
    public void fade()
    {
        StartCoroutine(FadeTextToZeroAlpha(0.5f,Text));
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
