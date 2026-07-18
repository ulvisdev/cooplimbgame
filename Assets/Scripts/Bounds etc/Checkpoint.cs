using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Exact place and direction objects respawn at.")]
    public Transform respawnPoint;

    private void Reset()
    {
        respawnPoint = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        Respawnable respawnable =
            other.GetComponentInParent<Respawnable>();

        if (respawnable == null)
            return;

        Transform point = respawnPoint != null
            ? respawnPoint
            : transform;

        respawnable.SetCheckpoint(point);
    }
}