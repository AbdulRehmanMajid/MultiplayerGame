using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Aimm_Data",menuName ="Aim_data")]
public class Aim_transition_data : ScriptableObject
{
 public float fpp_offset;
      public float fpp_height;
      public float fpp_distance;
      public float fpp_fov;
       public float offset;
    public float height;
    public float distance;
    public float aim_speed;
    public bool first_person_mode = false;

}
