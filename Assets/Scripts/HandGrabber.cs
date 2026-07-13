using UnityEngine;

public class HandGrabber : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private LimbInputSource inputSource;

    [Header("Hand")]
    public Rigidbody handRb;

    [Header("Grip Strength")]
    public float breakForce = 1500f;
    public float breakTorque = 1500f;

    private Rigidbody nearbyRb;
    private FixedJoint gripJoint;

    private void Awake()
    {
        if (handRb == null)
            handRb = GetComponent<Rigidbody>();

        if (inputSource == null)
            inputSource = GetComponentInParent<LimbInputSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody otherRb = other.attachedRigidbody;

        if (otherRb == null)
            return;

        if (otherRb == handRb)
            return;

        nearbyRb = otherRb;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == nearbyRb &&
            gripJoint == null)
        {
            nearbyRb = null;
        }
    }

    private void Update()
    {
        if (inputSource == null ||
            !inputSource.IsConfigured)
        {
            ReleaseGrip();
            return;
        }

        bool gripPressed =
            inputSource.ShoulderIsPressed();

        if (gripPressed &&
            gripJoint == null &&
            nearbyRb != null)
        {
            CreateGrip();
        }

        if (!gripPressed)
            ReleaseGrip();
    }

    private void CreateGrip()
    {
        if (handRb == null || nearbyRb == null)
            return;

        gripJoint =
            handRb.gameObject.AddComponent<FixedJoint>();

        gripJoint.connectedBody = nearbyRb;
        gripJoint.breakForce = breakForce;
        gripJoint.breakTorque = breakTorque;
    }

    private void ReleaseGrip()
    {
        if (gripJoint == null)
            return;

        Destroy(gripJoint);
        gripJoint = null;
    }
}