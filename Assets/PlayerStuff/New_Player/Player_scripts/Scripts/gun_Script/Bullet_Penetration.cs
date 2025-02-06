using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Penetration : MonoBehaviour
{
 

    public float penetrationAmount;
    public Vector3 endPoint;
    public Vector3? penetrationPoint;
    public Vector3? impactPoint;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
        UpdatePenetration();
        }
    }

    public void UpdatePenetration()
    {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            impactPoint = hit.point;
            Ray penRay = new Ray(hit.point + ray.direction * penetrationAmount, -ray.direction);
            RaycastHit penHit;
            if (hit.collider.Raycast(penRay, out penHit, penetrationAmount))
            {
                penetrationPoint = penHit.point;
                endPoint = this.transform.position + this.transform.forward * 1000;
            }
            else
            {
                endPoint = impactPoint.Value + ray.direction * penetrationAmount;
                penetrationPoint = endPoint;
            }
        }
        else
        {
            endPoint = this.transform.position + this.transform.forward * 1000;
            penetrationPoint = null;
            impactPoint = null;
        }
    }

    private void OnDrawGizmos()
    {
       // UpdatePenetration();

        if(!penetrationPoint.HasValue || !impactPoint.HasValue)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, endPoint);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, impactPoint.Value);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(impactPoint.Value, penetrationPoint.Value);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(penetrationPoint.Value ,endPoint);
        }
    }

}
