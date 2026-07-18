using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 2f;
    public float pauseAtEnds = 0.5f;
    public bool startMovingTowardsB = true;

    private Rigidbody rb;
    private bool movingTowardsB;
    private float pauseTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        movingTowardsB = startMovingTowardsB;
    }

    private void FixedUpdate()
    {
        if (pointA == null || pointB == null)
            return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector3 targetPosition = movingTowardsB
            ? pointB.position
            : pointA.position;

        Vector3 newPosition = Vector3.MoveTowards(
            rb.position,
            targetPosition,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        if ((newPosition - targetPosition).sqrMagnitude < 0.0001f)
        {
            movingTowardsB = !movingTowardsB;
            pauseTimer = pauseAtEnds;
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null)
            return;

        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawWireSphere(pointA.position, 0.25f);
        Gizmos.DrawWireSphere(pointB.position, 0.25f);
    }
}