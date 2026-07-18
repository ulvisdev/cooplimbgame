using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Respawnable respawnable =
            other.GetComponentInParent<Respawnable>();

        if (respawnable != null)
            respawnable.Respawn();
    }
}