using UnityEngine;

public class TorsoBalance : MonoBehaviour
{
    public Rigidbody torsoRb;

    [Header("Balance")]
    public bool balanceEnabled = true;
    public float uprightStrength = 250f;
    public float uprightDamping = 35f;

    [Header("Fall Control")]
    public float maxTiltAngle = 55f;

    void Reset()
    {
        torsoRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!balanceEnabled) return;

        float tilt = Vector3.Angle(transform.up, Vector3.up);

        // if tilted too far, player falls
        if (tilt > maxTiltAngle)
            return;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * torsoRb.rotation;

        Quaternion diff = targetRotation * Quaternion.Inverse(torsoRb.rotation);
        diff.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        Vector3 torque = axis.normalized * angle * Mathf.Deg2Rad * uprightStrength;
        torque -= torsoRb.angularVelocity * uprightDamping;

        torsoRb.AddTorque(torque, ForceMode.Acceleration);
    }
}