using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Noise : References : https://www.youtube.com/watch?v=WP-Bm65Q-1Y 
/// </summary>
public static class Noise
{
    /// <summary>
    /// Allows to generate a noise map describe by many parameters 
    /// </summary>
    /// <param name="mapWidth">Width of the noisemap</param>
    /// <param name="mapHeight">Lenght of the noisemap</param>
    /// <param name="seed">Seed of the noisemap</param>
    /// <param name="scale">number that determines at what distance to view the noisemap</param>
    /// <param name="octaves">the number of levels of detail you want you perlin noise to have</param>
    /// <param name="persistance">number that determines how much each octave contributes to the overall shape</param>
    /// <param name="lacunarity">number that determines how much detail is added or removed at each octave</param>
    /// <param name="offset">offset compared to the origin of the noisemap</param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

         return noiseMap;
    }
}
