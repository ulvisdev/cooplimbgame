using System.Collections;
using UnityEngine;

public class Respawnable : MonoBehaviour
{
    [Header("Respawn Anchor")]
    [Tooltip("Usually the torso Rigidbody.")]
    public Rigidbody anchorRigidbody;

    [Header("Starting Spawn")]
    public Transform startingSpawnPoint;

    [Header("Settings")]
    public float heightOffset = 1f;
    public float respawnCooldown = 0.5f;

    private Rigidbody[] rigidbodies;

    private Vector3 respawnPosition;
    private Quaternion respawnRotation;

    private bool isRespawning;

    private void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);

        if (anchorRigidbody == null && rigidbodies.Length > 0)
            anchorRigidbody = rigidbodies[0];

        if (startingSpawnPoint != null)
        {
            respawnPosition = startingSpawnPoint.position;
            respawnRotation = startingSpawnPoint.rotation;
        }
        else if (anchorRigidbody != null)
        {
            respawnPosition = anchorRigidbody.position;
            respawnRotation = anchorRigidbody.rotation;
        }
        else
        {
            respawnPosition = transform.position;
            respawnRotation = transform.rotation;
        }
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint == null)
            return;

        respawnPosition = checkpoint.position;
        respawnRotation = checkpoint.rotation;

        Debug.Log(
            $"{name} checkpoint set to {respawnPosition}",
            checkpoint
        );
    }

    public void Respawn()
    {
        if (!isRespawning)
            StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (anchorRigidbody == null)
        {
            Debug.LogError(
                $"{name} has no anchor Rigidbody assigned.",
                this
            );

            isRespawning = false;
            yield break;
        }

        bool[] originalKinematicStates =
            new bool[rigidbodies.Length];

        // Freeze all bodies while teleporting.
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            originalKinematicStates[i] =
                rigidbodies[i].isKinematic;

            rigidbodies[i].isKinematic = true;
        }

        Vector3 targetPosition =
            respawnPosition + Vector3.up * heightOffset;

        Vector3 movementOffset =
            targetPosition - anchorRigidbody.position;

        // Move the entire ragdoll by the same amount.
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].position += movementOffset;
        }

        Physics.SyncTransforms();

        yield return new WaitForFixedUpdate();

        // Restore each Rigidbody's original physics state.
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody rb = rigidbodies[i];

            rb.isKinematic = originalKinematicStates[i];

            // Velocity can only be changed after restoring a dynamic body.
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        yield return new WaitForSeconds(respawnCooldown);

        isRespawning = false;
    }
}