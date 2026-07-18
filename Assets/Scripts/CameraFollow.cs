using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Position")]
    public float distanceBehind = 9.5f;
    public float height = 7f;
    public float sideOffset = 0f;

    [Header("Smoothness")]
    public float positionSmoothTime = 0.25f;
    public float rotationSmoothSpeed = 5f;

    [Header("Look Target")]
    public float lookHeight = 2f;

    private Vector3 positionVelocity;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Get the player's forward direction without following body tilt.
        Vector3 flatForward = target.forward;
        flatForward.y = 0f;

        if (flatForward.sqrMagnitude < 0.001f)
            flatForward = Vector3.forward;

        flatForward.Normalize();

        Vector3 flatRight = Vector3.Cross(Vector3.up, flatForward);

        // Position the camera behind, above and optionally beside the player.
        Vector3 desiredPosition =
            target.position
            - flatForward * distanceBehind
            + Vector3.up * height
            + flatRight * sideOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        // Smoothly rotate the camera towards the player.
        Vector3 lookPosition = target.position + Vector3.up * lookHeight;
        Vector3 lookDirection = lookPosition - transform.position;

        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(
                lookDirection,
                Vector3.up
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotationSmoothSpeed * Time.deltaTime
            );
        }
    }
}