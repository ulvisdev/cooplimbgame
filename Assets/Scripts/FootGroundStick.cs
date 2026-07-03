using UnityEngine;
using UnityEngine.InputSystem;

public class FootGroundStick : MonoBehaviour
{
    public bool isLeftFoot = true;
    public Rigidbody footRb;
    public float liftThreshold = 0.2f;
    public float angleLimit = 10f;

    private ConfigurableJoint groundJoint;
    private bool touchingGround;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            touchingGround = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            touchingGround = false;
    }

    void Update()
    {
        var pad = Gamepad.current;
        if (pad == null) return;

        float trigger = isLeftFoot ? pad.leftTrigger.ReadValue() : pad.rightTrigger.ReadValue();
        bool shouldStick = touchingGround && trigger < liftThreshold;

        if (shouldStick && groundJoint == null)
            CreateGroundJoint();

        if (!shouldStick && groundJoint != null)
        {
            Destroy(groundJoint);
            groundJoint = null;
        }
    }

    void CreateGroundJoint()
    {
        groundJoint = footRb.gameObject.AddComponent<ConfigurableJoint>();
        groundJoint.connectedBody = null;

        groundJoint.xMotion = ConfigurableJointMotion.Locked;
        groundJoint.yMotion = ConfigurableJointMotion.Locked;
        groundJoint.zMotion = ConfigurableJointMotion.Locked;

        groundJoint.angularXMotion = ConfigurableJointMotion.Limited;
        groundJoint.angularYMotion = ConfigurableJointMotion.Limited;
        groundJoint.angularZMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = angleLimit;

        groundJoint.lowAngularXLimit = limit;
        groundJoint.highAngularXLimit = limit;
        groundJoint.angularYLimit = limit;
        groundJoint.angularZLimit = limit;

        groundJoint.rotationDriveMode = RotationDriveMode.Slerp;

        JointDrive drive = new JointDrive();
        drive.positionSpring = 5000f;
        drive.positionDamper = 300f;
        drive.maximumForce = 10000f;

        groundJoint.slerpDrive = drive;
    }
}

