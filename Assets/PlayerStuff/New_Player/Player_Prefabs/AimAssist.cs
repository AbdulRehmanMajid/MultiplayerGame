using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    private const string Tag = "enemy"; // Not currently used.

    public Transform playerCamera;
    public float aimAssistRadius = 0.5f;
    public float timeToAim = 0.3f;
    public LayerMask mask;

    public RotationChanges TrackTarget()
    {
        Transform target = SelectTarget();
        if (target == null)
        {
            return RotationChanges.Empty;
        }

        Vector3 targetPos = target.transform.position;
        float totalHorizontalRotationAngles = CalculateTotalRotationAngles(Vector3.up, targetPos);
        float totalVerticalRotationAngles = CalculateTotalRotationAngles(playerCamera.right, targetPos);

        float dx = CalculateDeltaRotationDegrees(totalVerticalRotationAngles, timeToAim, Time.deltaTime, targetPos);
        float dy = CalculateDeltaRotationDegrees(totalHorizontalRotationAngles, timeToAim, Time.deltaTime, targetPos);

        return new RotationChanges(pitchAdditionInDegrees: dx, turnAddition: dy * Vector3.up);
    }

    private Transform SelectTarget()
    {
        Vector3 direction = playerCamera.transform.forward;
        Vector3 startPoint = playerCamera.position;

        if (!Physics.SphereCast(startPoint, aimAssistRadius, direction, out RaycastHit hit, 1000f, mask))
        {
            return null;
        }

        return hit.collider.GetComponent<Zomb_health_collider>() != null ? hit.collider.transform : null;
    }

    private float CalculateDeltaRotationDegrees(float totalRotation, float time, float deltaTime, Vector3 target)
    {
        float adjustedTimeToAim = time * aimAssistRadius;
        float distance = (target - playerCamera.transform.position).magnitude;
        float angularVelocity = Mathf.Atan2(1f, distance) * Mathf.Rad2Deg / adjustedTimeToAim;

        return Mathf.Min(angularVelocity * deltaTime, Mathf.Abs(totalRotation)) * Mathf.Sign(totalRotation);
    }

    private float CalculateTotalRotationAngles(Vector3 planeNormal, Vector3 target)
    {
        Vector3 camForwardProjected = Vector3.ProjectOnPlane(playerCamera.forward, planeNormal);
        Vector3 playerToTargetProjected = Vector3.ProjectOnPlane((target - playerCamera.position).normalized, planeNormal);

        return Vector3.SignedAngle(camForwardProjected, playerToTargetProjected, planeNormal);
    }
}
