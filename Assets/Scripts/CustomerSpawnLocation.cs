using UnityEngine;

public class CustomerSpawnLocation : MonoBehaviour
{
    private CustomerSpawner customerSpawner;
    private Transform occupiedSpawnPoint;
    private bool released;

    public void SetSpawnInformation(
        CustomerSpawner spawner,
        Transform spawnPoint
    )
    {
        customerSpawner = spawner;
        occupiedSpawnPoint = spawnPoint;
    }

    public void ReleaseLocation()
    {
        if (released)
            return;

        released = true;

        if (customerSpawner != null)
        {
            customerSpawner.ReleaseSpawnPoint(
                occupiedSpawnPoint
            );
        }
    }

    private void OnDestroy()
    {
        ReleaseLocation();
    }
}