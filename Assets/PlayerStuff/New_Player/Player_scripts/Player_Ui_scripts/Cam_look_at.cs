using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Cam_look_at : MonoBehaviour
{
    public GameObject player;
    public bool is_dead;
    private quaternion def_pos;
    public bool Aim_assist = true;
    public float Aim_assist_speed = 0.1f;
    public GameObject Aim_assist_target;
    public float assis_delay = 0.01f;
    public bool assist_type_2 = false;

    void Start()
    {
        def_pos = transform.localRotation;
        StartCoroutine(AimAssistCoroutine());
    }
   
    void Update()
    {
        // When dead, look at the player.
        if (is_dead && player != null)
        {
            transform.LookAt(player.transform.position);
        }

        // When using assist type 2, smoothly rotate towards the aim assist target.
        if (assist_type_2 && Aim_assist_target != null)
        {
            Vector3 direction = Aim_assist_target.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * Aim_assist_speed);
        }
    }

    IEnumerator AimAssistCoroutine()
    {
        while (true)
        {
            if (Aim_assist && Aim_assist_target != null)
            {
                transform.LookAt(Aim_assist_target.transform.position);
            }
            yield return new WaitForSeconds(assis_delay);
        }
    }

    public void Isalive()
    {
        transform.localRotation = def_pos;
    }
}
