// using UnityEngine;

// public class LimbJointController : MonoBehaviour
// {
//     public ConfigurableJoint joint;

//     public float moveSpeed = 90f;
//     public float targetAngle = 0f;
//     public float minAngle = -70f;
//     public float maxAngle = 70f;

//     void FixedUpdate()
//     {
//         float input = 0f;

//         if (Input.GetKey(KeyCode.Q)) input = 1f;
//         else if (Input.GetKey(KeyCode.E)) input = -1f;
//         else targetAngle = 0f;

//         targetAngle += input * moveSpeed * Time.fixedDeltaTime;
//         targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

//         joint.targetRotation = Quaternion.Euler(targetAngle, 0f, 0f);
//     }
// }

// using UnityEngine;

// public class LimbJointController : MonoBehaviour
// {
//     public Rigidbody rb;
//     public float torquePower = 30f;

//     void FixedUpdate()
//     {
//         if (Input.GetKey(KeyCode.Z))
//             rb.AddRelativeTorque(Vector3.right * torquePower);

//         if (Input.GetKey(KeyCode.X))
//             rb.AddRelativeTorque(Vector3.left * torquePower);
//     }
// }

using UnityEngine;

public class LimbTargetController : MonoBehaviour
{
    public ConfigurableJoint joint;

    public float targetAngle = 0f;
    public float moveSpeed = 90f;
    public float minAngle = -30f;
    public float maxAngle = 177f;

    void FixedUpdate()
    {
        JointDrive drive = joint.angularXDrive;

        bool pressing = false;

        if (Input.GetKey(KeyCode.A))
        {
            targetAngle += moveSpeed * Time.fixedDeltaTime;
            pressing = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            targetAngle -= moveSpeed * Time.fixedDeltaTime;
            pressing = true;
        }

        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        if (pressing)
        {
            drive.positionSpring = 300f;
            drive.positionDamper = 40f;
            drive.maximumForce = 1000f;
        }
        else
        {
            drive.positionSpring = 0f;
            drive.positionDamper = 0f;
            drive.maximumForce = 0f;
        }

        joint.angularXDrive = drive;
        joint.targetRotation = Quaternion.Euler(targetAngle, 0f, 0f);
    }
}