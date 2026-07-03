using UnityEngine;
using UnityEngine.InputSystem;

public class LegGamepadController : MonoBehaviour
{
    public bool isLeftLeg = true;

    [Header("Joints")]
    public ConfigurableJoint hipJoint;
    public ConfigurableJoint kneeJoint;

    [Header("Hip")]
    public float hipDownAngle = 10f;
    public float hipUpAngle = -90f;

    [Header("Knee")]
    public float kneeStraightAngle = 0f;
    public float kneeBentAngle = 110f;

    void Update()
    {
        var pad = Gamepad.current;
        if (pad == null) return;

        float trigger = isLeftLeg ? pad.leftTrigger.ReadValue() : pad.rightTrigger.ReadValue();
        if (trigger < 0.05f) trigger = 0f; // Deadzone for trigger

        Vector2 stick = isLeftLeg ? pad.leftStick.ReadValue() : pad.rightStick.ReadValue();

        float hipAngle = Mathf.Lerp(hipDownAngle, hipUpAngle, trigger);

        float bendAmount = Mathf.Max(0f, -stick.y);
        float kneeAngle = Mathf.Lerp(kneeStraightAngle, kneeBentAngle, bendAmount);

        hipJoint.targetRotation = Quaternion.Euler(hipAngle, 0f, 0f);
        kneeJoint.targetRotation = Quaternion.Euler(kneeAngle, 0f, 0f);
    }
}