using UnityEngine;

public class FootGroundStick : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private LimbInputSource inputSource;

    [Header("Foot")]
    public Rigidbody footRb;

    [Header("Ground Stick")]
    public float liftThreshold = 0.2f;
    public float angleLimit = 10f;

    [Header("Joint Drive")]
    public float positionSpring = 5000f;
    public float positionDamper = 300f;
    public float maximumForce = 10000f;

    private ConfigurableJoint groundJoint;
    private bool touchingGround;

    private void Awake()
    {
        if (footRb == null)
            footRb = GetComponent<Rigidbody>();

        if (inputSource == null)
            inputSource = GetComponentInParent<LimbInputSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            touchingGround = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            touchingGround = false;
    }

    private void Update()
    {
        if (inputSource == null ||
            !inputSource.IsConfigured)
        {
            RemoveGroundJoint();
            return;
        }

        float trigger = inputSource.ReadTrigger();

        bool shouldStick =
            touchingGround &&
            trigger < liftThreshold;

        if (shouldStick && groundJoint == null)
            CreateGroundJoint();

        if (!shouldStick)
            RemoveGroundJoint();
    }

    private void CreateGroundJoint()
    {
        if (footRb == null)
            return;

        groundJoint =
            footRb.gameObject.AddComponent<ConfigurableJoint>();

        groundJoint.connectedBody = null;

        groundJoint.xMotion =
            ConfigurableJointMotion.Locked;

        groundJoint.yMotion =
            ConfigurableJointMotion.Locked;

        groundJoint.zMotion =
            ConfigurableJointMotion.Locked;

        groundJoint.angularXMotion =
            ConfigurableJointMotion.Limited;

        groundJoint.angularYMotion =
            ConfigurableJointMotion.Limited;

        groundJoint.angularZMotion =
            ConfigurableJointMotion.Limited;

        SoftJointLimit lowXLimit =
            new SoftJointLimit
            {
                limit = -angleLimit
            };

        SoftJointLimit highXLimit =
            new SoftJointLimit
            {
                limit = angleLimit
            };

        SoftJointLimit sideLimit =
            new SoftJointLimit
            {
                limit = angleLimit
            };

        groundJoint.lowAngularXLimit = lowXLimit;
        groundJoint.highAngularXLimit = highXLimit;
        groundJoint.angularYLimit = sideLimit;
        groundJoint.angularZLimit = sideLimit;

        groundJoint.rotationDriveMode =
            RotationDriveMode.Slerp;

        JointDrive drive = new JointDrive
        {
            positionSpring = positionSpring,
            positionDamper = positionDamper,
            maximumForce = maximumForce
        };

        groundJoint.slerpDrive = drive;
    }

    private void RemoveGroundJoint()
    {
        if (groundJoint == null)
            return;

        Destroy(groundJoint);
        groundJoint = null;
    }

    private void OnDisable()
    {
        RemoveGroundJoint();
    }
}