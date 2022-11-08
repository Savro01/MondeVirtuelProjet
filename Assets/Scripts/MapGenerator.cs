using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Paramétres de la noise map
    [Range(1, 200)]
    public int mapWidth;

    [Range(1, 200)]
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

    // Paramétre du terrain
    public Material grass;
    public Material water;
    public Material sand;
    public Material rock;
    public Material snow;

    float[,] terrainMatrix;

    void Start()
    {
        GenerateMap();

        creationMapCube();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        terrainMatrix = CreateTerrainMatrix(noiseMap, 1);

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

    //Create a matrix containing height position for each cube in plan
    // noiseMap : matrix 1000x1000 containing values between 0 and 1
    // scaling : parameter to choose the resolution of terrain (divide the noiseMap), possible values : [1,2,4,8]
    // RETURN : the matrix of the terrain
    public float[,] CreateTerrainMatrix(float[,] noiseMap, int scaling)
    {

        //Creating a matrix
        float[,] terrainMatrix = new float[noiseMap.GetLength(0) / scaling, noiseMap.GetLength(1) / scaling];

        //On X
        for (int x = 0; x < terrainMatrix.GetLength(0); x++)
        {
            //On Z
            for (int z = 0; z < terrainMatrix.GetLength(1); z++)
            {
                //Getting height for the cube
                float height = Mathf.Round(noiseMap[x, z] * 25);

                //Setting value 
                terrainMatrix[x, z] = height;
            }
        }

        return terrainMatrix;
    }

    float getNeighbourDiff(int i, int j, int matriceX, int matriceZ)
    {
        float min;
        if(i == 0)
        {
            if(j == 0)
            {
                min = Mathf.Min(terrainMatrix[i + 1, j], terrainMatrix[i, j + 1]);
            }else if(j == matriceZ-1)
            {
                min = Mathf.Min(terrainMatrix[i + 1, j], terrainMatrix[i, j - 1]);
            }
            else
            {
                min = Mathf.Min(terrainMatrix[i + 1, j], Mathf.Min(terrainMatrix[i, j - 1], terrainMatrix[i, j + 1]));
            }
        }
        else if (i == matriceX-1)
        {
            if (j == 0)
            {
                min = Mathf.Min(terrainMatrix[i - 1, j], terrainMatrix[i, j + 1]);
            }
            else if (j == matriceZ-1)
            {
                min = Mathf.Min(terrainMatrix[i - 1, j], terrainMatrix[i, j - 1]);
            }
            else
            {
                min = Mathf.Min(terrainMatrix[i - 1, j], Mathf.Min(terrainMatrix[i, j - 1], terrainMatrix[i, j + 1]));
            }
        }
        else if (j == 0)
        {
            min = Mathf.Min(terrainMatrix[i + 1, j], Mathf.Min(terrainMatrix[i - 1, j], terrainMatrix[i, j + 1]));
        }
        else if (j == matriceZ-1)
        {
            min = Mathf.Min(terrainMatrix[i + 1, j], Mathf.Min(terrainMatrix[i - 1, j], terrainMatrix[i, j - 1]));
        }
        else
        {
            min = Mathf.Min(terrainMatrix[i - 1, j], Mathf.Min(terrainMatrix[i + 1, j], Mathf.Min(terrainMatrix[i, j + 1], terrainMatrix[i, j - 1])));
        }
        return terrainMatrix[i, j] > min ? terrainMatrix[i, j] - min : 1;
    }


    void creationMapCube()
    {
        float cubeData;
        GameObject cube;

        int matriceX = terrainMatrix.GetLength(0);
        int matriceZ = terrainMatrix.GetLength(1);
        for (int i = 0; i < matriceX; i++)
        {
            for (int j = 0; j < matriceZ; j++)
            {
                cubeData = terrainMatrix[i, j];
                float diffNeighbour = getNeighbourDiff(i, j, matriceX, matriceZ);

                for (int k = 0; k < diffNeighbour; k++)
                {
                    cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(((float)i /(float)matriceX) * 100, ((cubeData - k) * 100/(float)matriceX), ((float)j /(float)matriceZ) * 100);
                    cube.transform.localScale = new Vector3(100 / (float)matriceX, 100/(float)matriceX, 100/(float)matriceZ);
                    cube.transform.parent = transform;
                    colorCubeGestion(cube);
                }
            }
        }
    }

    void colorCubeGestion(GameObject cube)
    {
        if (cube.transform.position.y < 4)
            cube.GetComponent<MeshRenderer>().material = water;
        else if (cube.transform.position.y < 6)
            cube.GetComponent<MeshRenderer>().material = sand;
        else if (cube.transform.position.y < 14)
            cube.GetComponent<MeshRenderer>().material = grass;
        else if (cube.transform.position.y < 20)
            cube.GetComponent<MeshRenderer>().material = rock;
        else
            cube.GetComponent<MeshRenderer>().material = snow;
    }
}
