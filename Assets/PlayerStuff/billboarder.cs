using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class billboarder : MonoBehaviour
{
    public Transform camera;
    public GameObject target;
    public TextMeshProUGUI damage_Text;
    
    public float disappertimer = 1f;
    public float disapperspeed = 3f;
    public float moveupSpeed = 20f;
    
    public float disappear_max = 0.6f;
    
    public float max_random = 0.2f;
    public float random_X = 0.2f;
    public float scale_increase = 2f;
    
    private Color textmesh_color;

    void Start()
    {
        textmesh_color = damage_Text.color;
        disappertimer = disappear_max;
        random_X = Random.Range(-max_random, max_random);
        
        if (target != null && camera != null)
        {
            Vector3 screenPos = camera.GetComponent<Camera>().WorldToScreenPoint(target.transform.position);
            damage_Text.transform.position = new Vector3(screenPos.x, screenPos.y, 0f);
        }
    }

    void Update()
    {
        // Move the text upward and add a slight random horizontal movement.
        damage_Text.transform.position += new Vector3(random_X, moveupSpeed, 0f) * Time.deltaTime;
       
        // Increase or decrease font size over time.
        if (disappertimer > disappear_max * 0.5f)
        {
            damage_Text.fontSize += scale_increase * Time.deltaTime;
        }
        else
        {
            damage_Text.fontSize -= scale_increase * Time.deltaTime;
        }
        
        disappertimer -= Time.deltaTime;
        if (disappertimer < 0)
        {
            // Fade out the text.
            textmesh_color.a -= disapperspeed * Time.deltaTime;
            damage_Text.color = textmesh_color;
            
            if (textmesh_color.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

