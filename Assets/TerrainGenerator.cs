using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Size")]
    public int width = 200;
    public int depth = 200;
    public int heightmapResolution = 257;
    public float terrainHeight = 30f;

    [Header("Noise")]
    public float noiseScale = 0.01f;
    public int octaves = 5;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    public float lacunarity = 2f;

    [Header("World")]
    public int seed = 12345;
    public Vector2 worldOffset;

    private Terrain terrain;
    private TerrainData terrainData;

    void Awake()
    {
        SetupTerrain();
    }

    void SetupTerrain()
    {
        terrain = GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("TerrainGenerator requires a Terrain component.");
            return;
        }

        // Every chunk gets its OWN TerrainData.
        if (terrain.terrainData != null)
        {
            terrainData = Instantiate(terrain.terrainData);
            terrainData.name = "Chunk Terrain Data";
        }
        else
        {
            terrainData = new TerrainData();
        }

        terrainData.heightmapResolution = heightmapResolution;
        terrainData.size = new Vector3(
            width,
            terrainHeight,
            depth
        );

        terrain.terrainData = terrainData;

        TerrainCollider collider =
            GetComponent<TerrainCollider>();

        if (collider != null)
        {
            collider.terrainData = terrainData;
        }
    }

    public void GenerateTerrain()
    {
        if (terrain == null || terrainData == null)
        {
            Debug.LogError("Terrain has not been set up.");
            return;
        }

        float[,] heights =
            new float[
                heightmapResolution,
                heightmapResolution
            ];

        float seedOffsetX = seed * 0.12345f;
        float seedOffsetZ = seed * 0.67891f;

        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int z = 0; z < heightmapResolution; z++)
            {
                float worldX =
                    worldOffset.x +
                    (x / (float)(heightmapResolution - 1)) * width;

                float worldZ =
                    worldOffset.y +
                    (z / (float)(heightmapResolution - 1)) * depth;

                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX =
                        worldX *
                        noiseScale *
                        frequency +
                        seedOffsetX;

                    float sampleZ =
                        worldZ *
                        noiseScale *
                        frequency +
                        seedOffsetZ;

                    float sample =
                        Mathf.PerlinNoise(
                            sampleX,
                            sampleZ
                        );

                    noiseHeight +=
                        sample * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[z, x] =
                    Mathf.Clamp01(
                        noiseHeight / 2f
                    );
            }
        }

        terrainData.SetHeights(
            0,
            0,
            heights
        );
    }
}
