using UnityEngine;
using UnityEngine.InputSystem;

public class HandGrabber : MonoBehaviour
{
    public Rigidbody handRb;
    public bool isLeftHand = true;

    private Rigidbody nearbyRb;
    private FixedJoint gripJoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
            nearbyRb = other.attachedRigidbody;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == nearbyRb && gripJoint == null)
            nearbyRb = null;
    }

    void Update()
    {
        var pad = Gamepad.current;
        if (pad == null) return;

        bool gripPressed = isLeftHand
            ? pad.leftShoulder.isPressed
            : pad.rightShoulder.isPressed;

        if (gripPressed && gripJoint == null && nearbyRb != null)
        {
            gripJoint = handRb.gameObject.AddComponent<FixedJoint>();
            gripJoint.connectedBody = nearbyRb;
            gripJoint.breakForce = 1500f;
            gripJoint.breakTorque = 1500f;
        }

        if (!gripPressed && gripJoint != null)
        {
            Destroy(gripJoint);
            gripJoint = null;
        }
    }
}