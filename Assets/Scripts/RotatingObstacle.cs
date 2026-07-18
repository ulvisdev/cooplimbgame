using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotatingObstacle : MonoBehaviour
{
    [Header("Rotation")]
    public Vector3 localAxis = Vector3.up;
    public float degreesPerSecond = 90f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (localAxis.sqrMagnitude < 0.001f)
            return;

        Quaternion rotationStep = Quaternion.AngleAxis(
            degreesPerSecond * Time.fixedDeltaTime,
            localAxis.normalized
        );

        rb.MoveRotation(rb.rotation * rotationStep);
    }
}