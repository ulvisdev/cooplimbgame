using UnityEngine;

public class ArmGamepadController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private LimbInputSource inputSource;

    [Header("Joints")]
    public ConfigurableJoint shoulderJoint;
    public ConfigurableJoint elbowJoint;

    [Header("Shoulder Forward/Up")]
    public float shoulderDownAngle = 14f;
    public float shoulderUpAngle = -160f;

    [Header("Shoulder Side Motion")]
    public float shoulderSideMaxAngle = 75f;
    public bool invertShoulderSide = false;

    [Header("Shoulder Twist Optional")]
    public float shoulderTwistMaxAngle = 25f;
    public bool useShoulderTwist = false;

    [Header("Elbow")]
    public float elbowStraightAngle = 0f;
    public float elbowBentAngle = -145f;

    [Header("Smoothing")]
    public float rotationSmoothSpeed = 12f;

    [Header("Grip Visual")]
    public Transform handVisual;
    public Vector3 openScale = Vector3.one;
    public Vector3 gripScale =
        new Vector3(1.25f, 1.25f, 1.25f);

    public float gripSpeed = 12f;

    private Quaternion shoulderTarget;
    private Quaternion elbowTarget;

    private void Start()
    {
        if (shoulderJoint != null)
            shoulderTarget = shoulderJoint.targetRotation;

        if (elbowJoint != null)
            elbowTarget = elbowJoint.targetRotation;
    }

    private void Update()
    {
        if (inputSource == null ||
            !inputSource.IsConfigured)
        {
            return;
        }

        if (shoulderJoint == null ||
            elbowJoint == null)
        {
            return;
        }

        float trigger = inputSource.ReadTrigger();

        if (trigger < 0.05f)
            trigger = 0f;

        Vector2 stick = inputSource.ReadStick();
        bool gripping = inputSource.ShoulderIsPressed();

        float shoulderForwardAngle = Mathf.Lerp(
            shoulderDownAngle,
            shoulderUpAngle,
            trigger
        );

        float sideInput = stick.x;

        if (invertShoulderSide)
            sideInput *= -1f;

        float shoulderSideAngle =
            sideInput * shoulderSideMaxAngle;

        float shoulderTwistAngle = 0f;

        if (useShoulderTwist)
        {
            shoulderTwistAngle =
                inputSource.ReadTwistInput() *
                shoulderTwistMaxAngle;
        }

        float bendAmount = Mathf.Clamp01(-stick.y);

        float elbowAngle = Mathf.Lerp(
            elbowStraightAngle,
            elbowBentAngle,
            bendAmount
        );

        Quaternion forwardRot = Quaternion.AngleAxis(
            shoulderForwardAngle,
            Vector3.right
        );

        Quaternion sideRot = Quaternion.AngleAxis(
            shoulderSideAngle,
            Vector3.forward
        );

        Quaternion twistRot = Quaternion.AngleAxis(
            shoulderTwistAngle,
            Vector3.up
        );

        Quaternion wantedShoulder =
            twistRot * sideRot * forwardRot;

        Quaternion wantedElbow = Quaternion.Euler(
            elbowAngle,
            0f,
            0f
        );

        shoulderTarget = Quaternion.Slerp(
            shoulderTarget,
            wantedShoulder,
            Time.deltaTime * rotationSmoothSpeed
        );

        elbowTarget = Quaternion.Slerp(
            elbowTarget,
            wantedElbow,
            Time.deltaTime * rotationSmoothSpeed
        );

        shoulderJoint.targetRotation = shoulderTarget;
        elbowJoint.targetRotation = elbowTarget;

        if (handVisual != null)
        {
            Vector3 targetScale =
                gripping ? gripScale : openScale;

            handVisual.localScale = Vector3.Lerp(
                handVisual.localScale,
                targetScale,
                Time.deltaTime * gripSpeed
            );
        }
    }
}