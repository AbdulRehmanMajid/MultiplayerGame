using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    private const string Tag = "enemy";

    public Transform playerCamera;
    public float aimAssistRadius = 0.5f;
    public float timeToAim = 0.3f;
    public LayerMask mask;
    //public GameObject target;
   

    public RotationChanges TrackTarget()
    {
    
    

        var target = SelectTarget();
        
        // if we didn't find a target, just return an empty result that does not change the rotations
        if (!target)
        {
            return RotationChanges.Empty;
        }

        var targetPos = target.transform.position;
        var totalHorizontalRotationAngles = CalculateTotalRotationAngles(Vector3.up, targetPos);
        var totalVerticalRotationAngles = CalculateTotalRotationAngles(playerCamera.right, targetPos);

        var dx = CalculateDeltaRotationDegrees(totalVerticalRotationAngles, timeToAim, Time.deltaTime, targetPos);
        var dy = CalculateDeltaRotationDegrees(totalHorizontalRotationAngles, timeToAim, Time.deltaTime, targetPos);

        // return the pitch addition in degrees and the turn addition in euler angles (rotated along Y axis)
        return new RotationChanges(pitchAdditionInDegrees: dx, turnAddition: dy * Vector3.up);
    }
        private Transform SelectTarget()
{
    // find start point and direction for the spherecast
    var direction = playerCamera.transform.forward;
    var startPoint = playerCamera.position;

    // shoot the spherecast
    if (!Physics.SphereCast(startPoint, aimAssistRadius, direction, out var hit, 1000f,mask))
    {
        // if we didn't hit anything then we won't do any aimbotting
        return null;
    }

    // we found a target only if the collider we hit is tagged as 'enemy'
    return hit.collider.GetComponent<Zomb_health_collider>() ? hit.collider.transform : null;
}

   

    private float CalculateDeltaRotationDegrees(float totalRotation, float time, float deltaTime, Vector3 target)
    {
        // normalize for the aim assist radius or a short radius will have slower aim speed
        var adjustedTimeToAim = time * aimAssistRadius;
        
        // get the distance from player to target
        var distance = (target - playerCamera.transform.position).magnitude;
        
        // calculate the angle from its opposite and adjacent sides and divide by time to get angular velocity
        var angularVelocity = Mathf.Atan2(1f, distance) * Mathf.Rad2Deg / adjustedTimeToAim;
        
        // to avoid overshoot, if we are closer to aiming at the target than one step, we just snap to it
        return Mathf.Min(angularVelocity * deltaTime, Mathf.Abs(totalRotation)) * Mathf.Sign(totalRotation);
    }

    private float CalculateTotalRotationAngles(Vector3 planeNormal, Vector3 target)
    {
        // project to a plane defined by its normal vector, for both the actual and desired look directions
        var camForwardProjected = Vector3.ProjectOnPlane(playerCamera.forward, planeNormal);
        var playerToTargetProjected = Vector3.ProjectOnPlane((target - playerCamera.position).normalized, planeNormal);
        
        // calculate the signed angle between the actual and desired look directions
        return Vector3.SignedAngle(camForwardProjected, playerToTargetProjected, planeNormal);
    }
}
