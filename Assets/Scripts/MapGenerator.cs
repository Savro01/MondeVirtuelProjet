using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Range(1, 100)]
    public int mapWidth;

    [Range(1, 100)]
    public int mapHeight;

    [Range(1.01f,100f)]
    public float noiseScale;

    [Range(1f, 10f)]
    public int octaves;

    [Range(0.01f, 10f)]
    public float persistance;

    [Range(1f, 10f)]
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate = false;

    void Start()
    {
        MapGenerator mapGen = GetComponent<MapGenerator>();
        mapGen.GenerateMap();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

    private void Update()
    {
        if (autoUpdate)
        {
            GenerateMap();
        }
    }
}
