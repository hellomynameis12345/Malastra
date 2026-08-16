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
    public Vector2 offset;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("TerrainGenerator requires a Terrain component.");
            return;
        }

        TerrainData terrainData = terrain.terrainData;

        terrainData.heightmapResolution = heightmapResolution;
        terrainData.size = new Vector3(width, terrainHeight, depth);

        float[,] heights = new float[heightmapResolution, heightmapResolution];

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        System.Random random = new System.Random(seed);

        float offsetX = random.Next(-100000, 100000) + offset.x;
        float offsetZ = random.Next(-100000, 100000) + offset.y;

        // First pass: generate the raw noise.
        float[,] noiseMap = new float[heightmapResolution, heightmapResolution];

        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int z = 0; z < heightmapResolution; z++)
            {
                float amplitude = 1f;
                float frequency = 1f;

                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x / (float)(heightmapResolution - 1)) * width;
                    float sampleZ = (z / (float)(heightmapResolution - 1)) * depth;

                    sampleX = sampleX * noiseScale * frequency + offsetX;
                    sampleZ = sampleZ * noiseScale * frequency + offsetZ;

                    float sample = Mathf.PerlinNoise(sampleX, sampleZ);

                    noiseHeight += sample * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseMap[x, z] = noiseHeight;

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;
            }
        }

        // Second pass: normalize the noise.
        for (int x = 0; x < heightmapResolution; x++)
        {
            for (int z = 0; z < heightmapResolution; z++)
            {
                float normalizedHeight =
                    Mathf.InverseLerp(
                        minNoiseHeight,
                        maxNoiseHeight,
                        noiseMap[x, z]
                    );

                heights[z, x] = normalizedHeight;
            }
        }

        terrainData.SetHeights(0, 0, heights);

        Debug.Log(
            $"Terrain generated. Seed: {seed}, " +
            $"Size: {width}x{depth}, " +
            $"Octaves: {octaves}"
        );
    }
}