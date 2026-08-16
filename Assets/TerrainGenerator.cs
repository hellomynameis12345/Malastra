using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 100;
    public int depth = 100;
    public float heightMultiplier = 10f;
    public float noiseScale = 20f;

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;

        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, heightMultiplier, depth);

        float[,] heights = new float[width + 1, depth + 1];

        for (int x = 0; x <= width; x++)
        {
            for (int z = 0; z <= depth; z++)
            {
                float xCoord = (float)x / width * noiseScale;
                float zCoord = (float)z / depth * noiseScale;

                float height = Mathf.PerlinNoise(xCoord, zCoord);

                heights[x, z] = height;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}