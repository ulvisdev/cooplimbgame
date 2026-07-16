using System.Collections;
using UnityEngine;

public class ItemRespawner : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Respawning")]
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private float takenDistance = 0.5f;

    private GameObject currentItem;
    private bool isRespawning;

    private void Start()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        SpawnItem();
    }

    private void Update()
    {
        if (isRespawning)
            return;

        // The item was destroyed, such as when added to a pizza.
        if (currentItem == null)
        {
            StartCoroutine(RespawnAfterDelay());
            return;
        }

        // The item was picked up or moved away.
        float distanceFromSpawn = Vector3.Distance(
            currentItem.transform.position,
            spawnPoint.position
        );

        if (distanceFromSpawn >= takenDistance)
        {
            // Stop tracking the taken item.
            // It remains in the world for the player to use.
            currentItem = null;

            StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        isRespawning = true;

        yield return new WaitForSeconds(respawnDelay);

        SpawnItem();

        isRespawning = false;
    }

    private void SpawnItem()
    {
        currentItem = Instantiate(
            itemPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }
}