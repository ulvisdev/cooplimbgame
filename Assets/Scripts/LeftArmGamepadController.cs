using UnityEngine;
using UnityEngine.InputSystem;

public class LeftArmGamepadController : MonoBehaviour
{
    [Header("Joints")]
    public ConfigurableJoint shoulderJoint;
    public ConfigurableJoint elbowJoint;

    [Header("Shoulder")]
    public float shoulderDownAngle = 14f;
    public float shoulderUpAngle = -150f;

    [Header("Elbow")]
    public float elbowMaxAngle = 130f;

    [Header("Grip Scale")]
    public Transform handVisual;
    public Vector3 openScale = Vector3.one;
    public Vector3 gripScale = new Vector3(1.25f, 1.25f, 1.25f);
    public float gripSpeed = 12f;

    private Gamepad pad;

    void Update()
    {
        pad = Gamepad.current;
        if (pad == null) return;

        float trigger = pad.leftTrigger.ReadValue();
        Vector2 stick = pad.leftStick.ReadValue();
        bool gripping = pad.leftShoulder.isPressed;

        float shoulderAngle = Mathf.Lerp(shoulderDownAngle, shoulderUpAngle, trigger);
        float elbowAngle = Mathf.Lerp(0f, -130f, (stick.y + 1f) / 2f);

        //SetJointTargetRotation(shoulderJoint, Quaternion.Euler(shoulderAngle, 0f, 0f));
        shoulderJoint.targetRotation = Quaternion.Euler(shoulderAngle, 0f, 0f);
        elbowJoint.targetRotation = Quaternion.Euler(elbowAngle, 0f, 0f);

        if (handVisual != null)
        {
            Vector3 targetScale = gripping ? gripScale : openScale;
            handVisual.localScale = Vector3.Lerp(
                handVisual.localScale,
                targetScale,
                Time.deltaTime * gripSpeed
            );
        }
    }
}