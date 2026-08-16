using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [Header("World")]
    public int seed = 12345;

    [Header("Chunks")]
    public int chunkSize = 200;
    public int viewDistance = 2;

    [Header("Player")]
    public Transform player;

    [Header("Terrain")]
    public TerrainGenerator terrainPrefab;

    private Dictionary<Vector2Int, TerrainGenerator> chunks =
        new Dictionary<Vector2Int, TerrainGenerator>();

    private Vector2Int currentPlayerChunk;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("WorldManager: Player is not assigned.");
            return;
        }

        currentPlayerChunk = GetPlayerChunk();

        UpdateChunks();
    }

    void Update()
    {
        Vector2Int newPlayerChunk = GetPlayerChunk();

        if (newPlayerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = newPlayerChunk;
            UpdateChunks();
        }
    }

    Vector2Int GetPlayerChunk()
    {
        int chunkX =
            Mathf.FloorToInt(player.position.x / chunkSize);

        int chunkZ =
            Mathf.FloorToInt(player.position.z / chunkSize);

        return new Vector2Int(chunkX, chunkZ);
    }

void UpdateChunks()
{
    for (int x = -viewDistance; x <= viewDistance; x++)
    {
        for (int z = -viewDistance; z <= viewDistance; z++)
        {
            Vector2Int coordinate =
                new Vector2Int(
                    currentPlayerChunk.x + x,
                    currentPlayerChunk.y + z
                );

            CreateChunk(coordinate);
        }
    }

    RemoveFarChunks();
}

    void CreateChunk(Vector2Int coordinate)
    {
        if (chunks.ContainsKey(coordinate))
            return;

        TerrainGenerator chunk =
            Instantiate(
                terrainPrefab,
                new Vector3(
                    coordinate.x * chunkSize,
                    0,
                    coordinate.y * chunkSize
                ),
                Quaternion.identity
            );

        chunk.seed = seed;

        chunk.worldOffset = new Vector2(
            coordinate.x * chunkSize,
            coordinate.y * chunkSize
        );

        // Generate AFTER setting the chunk's world offset.
        chunk.GenerateTerrain();

        chunks.Add(coordinate, chunk);

        Debug.Log($"Generated chunk {coordinate}");
    }

    void RemoveFarChunks()
    {
        List<Vector2Int> chunksToRemove =
            new List<Vector2Int>();

        foreach (var chunk in chunks)
        {
            int distanceX =
                Mathf.Abs(
                    chunk.Key.x - currentPlayerChunk.x
                );

            int distanceZ =
                Mathf.Abs(
                    chunk.Key.y - currentPlayerChunk.y
                );

            if (distanceX > viewDistance ||
                distanceZ > viewDistance)
            {
                chunksToRemove.Add(chunk.Key);
            }
        }

        foreach (Vector2Int coordinate in chunksToRemove)
        {
            Destroy(chunks[coordinate].gameObject);
            chunks.Remove(coordinate);

            Debug.Log($"Unloaded chunk {coordinate}");
        }
    }
}
