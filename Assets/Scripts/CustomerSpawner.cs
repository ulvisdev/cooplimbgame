using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer")]
    public GameObject customerPrefab;

    [Header("Possible Spawn Locations")]
    public Transform[] spawnPoints;

    [Header("Spawn Timing")]
    public float minimumSpawnDelay = 3f;
    public float maximumSpawnDelay = 8f;

    private readonly List<Transform> availableSpawnPoints = new();

    private void Start()
    {
        availableSpawnPoints.AddRange(spawnPoints);
        StartCoroutine(SpawnCustomers());
    }

    private IEnumerator SpawnCustomers()
    {
        while (true)
        {
            float delay = Random.Range(
                minimumSpawnDelay,
                maximumSpawnDelay
            );

            yield return new WaitForSeconds(delay);

            SpawnCustomer();
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefab == null)
        {
            Debug.LogWarning("Customer prefab is not assigned.");
            return;
        }

        if (availableSpawnPoints.Count == 0)
        {
            Debug.Log("All customer locations are currently occupied.");
            return;
        }

        int randomIndex = Random.Range(
            0,
            availableSpawnPoints.Count
        );

        Transform selectedPoint = availableSpawnPoints[randomIndex];

        availableSpawnPoints.RemoveAt(randomIndex);

        GameObject customer = Instantiate(
            customerPrefab,
            selectedPoint.position,
            selectedPoint.rotation,
            transform
        );

        CustomerOrder customerOrder =
            customer.GetComponent<CustomerOrder>();

        if (customerOrder != null)
            customerOrder.RandomizeCustomer();

        CustomerSpawnLocation location =
            customer.GetComponent<CustomerSpawnLocation>();

        if (location == null)
            location = customer.AddComponent<CustomerSpawnLocation>();

        location.SetSpawnInformation(this, selectedPoint);
    }

    public void ReleaseSpawnPoint(Transform spawnPoint)
    {
        if (spawnPoint == null)
            return;

        if (!availableSpawnPoints.Contains(spawnPoint))
            availableSpawnPoints.Add(spawnPoint);
    }
}