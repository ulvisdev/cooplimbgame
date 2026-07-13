using UnityEngine;
using UnityEngine.InputSystem;

public class LimbInputSource : MonoBehaviour
{
    private PlayerInput playerInput;
    private ControlSide controlSide;

    public bool IsConfigured =>
        playerInput != null &&
        GetPairedGamepad() != null;

    public void Configure(
        PlayerInput newPlayerInput,
        ControlSide newControlSide)
    {
        playerInput = newPlayerInput;
        controlSide = newControlSide;
    }

    public Vector2 ReadStick()
    {
        Gamepad gamepad = GetPairedGamepad();

        if (gamepad == null)
            return Vector2.zero;

        return controlSide == ControlSide.Left
            ? gamepad.leftStick.ReadValue()
            : gamepad.rightStick.ReadValue();
    }

    public float ReadTrigger()
    {
        Gamepad gamepad = GetPairedGamepad();

        if (gamepad == null)
            return 0f;

        return controlSide == ControlSide.Left
            ? gamepad.leftTrigger.ReadValue()
            : gamepad.rightTrigger.ReadValue();
    }

    public bool ShoulderIsPressed()
    {
        Gamepad gamepad = GetPairedGamepad();

        if (gamepad == null)
            return false;

        return controlSide == ControlSide.Left
            ? gamepad.leftShoulder.isPressed
            : gamepad.rightShoulder.isPressed;
    }

    public bool ShoulderPressedThisFrame()
    {
        Gamepad gamepad = GetPairedGamepad();

        if (gamepad == null)
            return false;

        return controlSide == ControlSide.Left
            ? gamepad.leftShoulder.wasPressedThisFrame
            : gamepad.rightShoulder.wasPressedThisFrame;
    }

    public float ReadTwistInput()
    {
        Gamepad gamepad = GetPairedGamepad();

        if (gamepad == null)
            return 0f;

        float twistInput = 0f;

        if (gamepad.buttonWest.isPressed)
            twistInput -= 1f;

        if (gamepad.buttonEast.isPressed)
            twistInput += 1f;

        return twistInput;
    }

    private Gamepad GetPairedGamepad()
    {
        if (playerInput == null)
            return null;

        foreach (InputDevice device in playerInput.devices)
        {
            if (device is Gamepad gamepad)
                return gamepad;
        }

        return null;
    }
}