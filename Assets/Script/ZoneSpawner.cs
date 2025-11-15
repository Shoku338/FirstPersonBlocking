using UnityEngine;
using System.Collections.Generic;

public class ZoneSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnRadius = 50f;
    public int maxObjects = 20;
    public LayerMask groundMask;

    [Header("SpawnCongig")]
    public EnvironmentSpawnConfig spawnConfig;

    // The rest of the prefab fields can be REMOVED if you fully use the spawnConfig ScriptableObject
    // However, if you're keeping them for simplicity, they don't hurt anything.
    // ...

    [Header("Illusion Chance (0-1)")]
    public float illusionChance = 0.25f; // This can also be removed if using per-object chance

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool playerIsInside = false; // Flag to track player presence

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if(meshRenderer != null){
            meshRenderer.enabled = false;
        }
        // Initial spawn upon starting the zone
        SpawnEnvironmentObjects();

        // Optional: Ensure the spawner's collider is a trigger for the logic below
        // This assumes the ZoneSpawner GameObject has a Collider attached.
        Collider zoneCollider = GetComponent<Collider>();
        if (zoneCollider != null)
        {
            zoneCollider.isTrigger = true;
        }
    }

    // --- Reshuffling Logic ---

    // 1. Detect when the player leaves the zone
    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting is the player
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;

            // Start the reshuffling process
            ReshuffleEnvironment();
        }
    }

    // 2. Detect when the player enters the zone (optional, for tracking)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
        }
    }

    public void ReshuffleEnvironment()
    {
        // A. Destroy all currently spawned objects
        foreach (GameObject obj in spawnedObjects)
        {
            // Use null check because the IllusionEffect script destroys some objects
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();

        // B. Spawn a new set of objects
        SpawnEnvironmentObjects();

        Debug.Log("Zone environment reshuffled!");
    }

    // --- Spawning Logic (Untouched) ---

    void SpawnEnvironmentObjects()
    {
        // Calculate the total weight of all objects for normalization
        float totalWeight = 0f;
        foreach (var config in spawnConfig.SpawnableObjects)
        {
            if (config.Prefabs.Length > 0) // Safety check: only count objects with prefabs
            {
                totalWeight += config.SpawnWeight;
            }
        }

        for (int i = 0; i < maxObjects; i++)
        {
            // ... (Raycasting and Weighted Selection Logic as before) ...

            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = 200f; // start ray high up

            if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 500f, groundMask))
            {
                // --- 1. Choose Object Type based on Weight ---
                float randomWeight = Random.Range(0f, totalWeight);
                SpawnableObjectConfig chosenConfig = null;
                float currentWeight = 0f;

                foreach (var config in spawnConfig.SpawnableObjects)
                {
                    if (config.Prefabs.Length > 0)
                    {
                        currentWeight += config.SpawnWeight;
                        if (randomWeight <= currentWeight)
                        {
                            chosenConfig = config;
                            break;
                        }
                    }
                }

                if (chosenConfig == null) continue;

                // --- 2. Choose Prefab from the chosen type's list ---
                GameObject prefabToSpawn = chosenConfig.Prefabs[Random.Range(0, chosenConfig.Prefabs.Length)];

                // --- 3. Spawn Illusion or Real ---
                bool isIllusion = Random.value < chosenConfig.IllusionChance;
                var obj = Instantiate(prefabToSpawn, hit.point, Quaternion.identity);

                if (isIllusion)
                {
                    var illusion = obj.AddComponent<IllusionEffect>();
                }

                spawnedObjects.Add(obj);
            }
        }
    }
}