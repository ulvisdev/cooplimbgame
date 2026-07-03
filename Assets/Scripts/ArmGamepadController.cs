using UnityEngine;
using UnityEngine.InputSystem;

public class ArmGamepadController : MonoBehaviour
{
    public bool isLeftArm = true;

    [Header("Joints")]
    public ConfigurableJoint shoulderJoint;
    public ConfigurableJoint elbowJoint;

    [Header("Shoulder")]
    public float shoulderDownAngle = 14f;
    public float shoulderUpAngle = -150f;

    [Header("Elbow")]
    public float elbowStraightAngle = 0f;
    public float elbowBentAngle = -130f;

    [Header("Grip Visual")]
    public Transform handVisual;
    public Vector3 openScale = Vector3.one;
    public Vector3 gripScale = new Vector3(1.25f, 1.25f, 1.25f);
    public float gripSpeed = 12f;

    void Update()
    {
        var pad = Gamepad.current;
        if (pad == null) return;

        float trigger = isLeftArm ? pad.leftTrigger.ReadValue() : pad.rightTrigger.ReadValue();
        Vector2 stick = isLeftArm ? pad.leftStick.ReadValue() : pad.rightStick.ReadValue();
        bool gripping = isLeftArm ? pad.leftShoulder.isPressed : pad.rightShoulder.isPressed;

        float shoulderAngle = Mathf.Lerp(shoulderDownAngle, shoulderUpAngle, trigger);
        float elbowAngle = Mathf.Lerp(elbowStraightAngle, elbowBentAngle, (stick.y + 1f) / 2f);

        shoulderJoint.targetRotation = Quaternion.Euler(shoulderAngle, 0f, 0f);
        elbowJoint.targetRotation = Quaternion.Euler(elbowAngle, 0f, 0f);

        if (handVisual != null)
        {
            Vector3 targetScale = gripping ? gripScale : openScale;
            handVisual.localScale = Vector3.Lerp(handVisual.localScale, targetScale, Time.deltaTime * gripSpeed);
        }
    }
}