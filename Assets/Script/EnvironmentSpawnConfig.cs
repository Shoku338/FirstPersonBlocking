// EnvironmentSpawnConfig.cs (New ScriptableObject file)

using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentSpawnConfig", menuName = "Custom/Environment Spawn Config")]
public class EnvironmentSpawnConfig : ScriptableObject
{
    // The list that holds the configuration for each object type
    public SpawnableObjectConfig[] SpawnableObjects;
}

// Configuration for a single object type
[System.Serializable]
public class SpawnableObjectConfig
{
    public string ObjectName;          // e.g., "Treasure Chest"
    public GameObject[] Prefabs;       // The list of prefabs for this type

    [Range(0f, 1f)]
    public float SpawnWeight = 0.5f;   // Relative chance of this type being chosen (0 to 1)

    [Range(0f, 1f)]
    public float IllusionChance = 0.5f; // Chance this specific object becomes an illusion (0 to 1)

    // Optional: Max count, min distance, etc.
}