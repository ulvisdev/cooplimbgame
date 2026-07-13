using UnityEngine;

public class LegGamepadController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private LimbInputSource inputSource;

    [Header("Joints")]
    public ConfigurableJoint hipJoint;
    public ConfigurableJoint kneeJoint;

    [Header("Hip Forward/Up")]
    public float hipDownAngle = 10f;
    public float hipUpAngle = -110f;

    [Header("Hip Side Motion")]
    public float hipSideMaxAngle = 55f;
    public bool invertHipSide = false;

    [Header("Hip Twist Optional")]
    public float hipTwistMaxAngle = 15f;
    public bool useHipTwist = false;

    [Header("Knee")]
    public float kneeStraightAngle = 0f;
    public float kneeBentAngle = 135f;

    [Header("Smoothing")]
    public float rotationSmoothSpeed = 12f;

    private Quaternion hipTarget;
    private Quaternion kneeTarget;

    private void Start()
    {
        if (hipJoint != null)
            hipTarget = hipJoint.targetRotation;

        if (kneeJoint != null)
            kneeTarget = kneeJoint.targetRotation;
    }

    private void Update()
    {
        if (inputSource == null ||
            !inputSource.IsConfigured)
        {
            return;
        }

        if (hipJoint == null ||
            kneeJoint == null)
        {
            return;
        }

        float trigger = inputSource.ReadTrigger();

        if (trigger < 0.05f)
            trigger = 0f;

        Vector2 stick = inputSource.ReadStick();

        float hipForwardAngle = Mathf.Lerp(
            hipDownAngle,
            hipUpAngle,
            trigger
        );

        float sideInput = stick.x;

        if (invertHipSide)
            sideInput *= -1f;

        float hipSideAngle =
            sideInput * hipSideMaxAngle;

        float hipTwistAngle = 0f;

        if (useHipTwist)
        {
            hipTwistAngle =
                inputSource.ReadTwistInput() *
                hipTwistMaxAngle;
        }

        float bendAmount = Mathf.Clamp01(-stick.y);

        float kneeAngle = Mathf.Lerp(
            kneeStraightAngle,
            kneeBentAngle,
            bendAmount
        );

        Quaternion forwardRot = Quaternion.AngleAxis(
            hipForwardAngle,
            Vector3.right
        );

        Quaternion sideRot = Quaternion.AngleAxis(
            hipSideAngle,
            Vector3.forward
        );

        Quaternion twistRot = Quaternion.AngleAxis(
            hipTwistAngle,
            Vector3.up
        );

        Quaternion wantedHip =
            twistRot * sideRot * forwardRot;

        Quaternion wantedKnee = Quaternion.Euler(
            kneeAngle,
            0f,
            0f
        );

        hipTarget = Quaternion.Slerp(
            hipTarget,
            wantedHip,
            Time.deltaTime * rotationSmoothSpeed
        );

        kneeTarget = Quaternion.Slerp(
            kneeTarget,
            wantedKnee,
            Time.deltaTime * rotationSmoothSpeed
        );

        hipJoint.targetRotation = hipTarget;
        kneeJoint.targetRotation = kneeTarget;
    }
}