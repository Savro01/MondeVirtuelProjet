using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RiverGenerator2))]
public class MapGenerator : MonoBehaviour
{
    // Param�tres de la noise map
    [Range(1, 1000)]
    public int mapWidth;

    [Range(1, 1000)]
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

    public bool addRiver = false;
    bool addRiverRemember;

    // Param�tre du terrain
    public Material grass;
    public Material water;
    public Material sand;
    public Material rock;
    public Material snow;

    float[,] terrainMatrix;
    bool[,] riverMatrix;
    List<Vector2> bordures;
    List<Vector2> effectiveBordures;

    RiverGenerator2 riverGenerator;
    Dictionary<Vector3, GameObject> objects = new Dictionary<Vector3, GameObject>();

    void Start()
    {
        riverGenerator = GetComponent<RiverGenerator2>();
        addRiverRemember = addRiver;
        GenerateMap();
    }

    //Returns the coordinates of the cubes which are in border of water.
    List<Vector2> creationBorduresEau()
    {
        List<Vector2> bordures = new List<Vector2>();
        float matriceX = terrainMatrix.GetLength(0);
        float matriceY = terrainMatrix.GetLength(1);
        for (int i = 1; i < matriceX-1; i++)
        {
            for (int j = 1; j < terrainMatrix.GetLength(1)-1; j++)
            {
                if (terrainMatrix[i, j] < 4 && (terrainMatrix[i, j + 1] == 4 || terrainMatrix[i + 1, j] == 4 || terrainMatrix[i - 1, j] == 4 || terrainMatrix[i, j - 1] == 4))
                {
                    //bordures.Add(new Vector2(((float)i / (float)matriceX) * 100, ((float)j / (float)matriceY) * 100));
                    bordures.Add(new Vector2(i, j));
                }
            }
        }
        return bordures;
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMap2 = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed+1, noiseScale, octaves+1, persistance, lacunarity, offset);

        terrainMatrix = CreateTerrainMatrix(noiseMap, noiseMap2, 1);
        bordures = creationBorduresEau();
        riverMatrix = riverGenerator.makeRiversLine(terrainMatrix, bordures);
        bordures = riverGenerator.getStartBlocPossible();
        effectiveBordures = riverGenerator.geteffectiveStartBloc();
        creationMapCube(bordures);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }

    private void Update()
    {
        if (addRiver != addRiverRemember)
        {
            addRiverRemember = addRiver;
            riverMatrix = riverGenerator.makeRiversLine(terrainMatrix, bordures);
            resetColor();
        }
    }

    //Create a matrix containing height position for each cube in plan
    // noiseMap : matrix 150x150 containing values between 0 and 1
    // scaling : parameter to choose the resolution of terrain (divide the noiseMap), possible values : [1,2,4,8]
    // RETURN : the matrix of the terrain
    public float[,] CreateTerrainMatrix(float[,] noiseMap1, float[,] noiseMap2, int scaling)
    {

        //Creating a matrix
        float[,] terrainMatrix = new float[noiseMap1.GetLength(0) / scaling, noiseMap1.GetLength(1) / scaling];

        //On X
        for (int x = 0; x < terrainMatrix.GetLength(0); x++)
        {
            //On Z
            for (int z = 0; z < terrainMatrix.GetLength(1); z++)
            {
                //Getting height for the cube
                float height;
                height = Mathf.Round(noiseMap1[x, z] * x/3 + 2 - noiseMap2[x, z] * (terrainMatrix.GetLength(1)-z) /20);
                if (height < 4)
                {
                    height = 3;
                }
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


    void creationMapCube(List<Vector2> bordures)
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
                    colorCubeGestion(cube, i, j);
                    objects.Add(new Vector3(i,j,k), cube);
                }
            }
        }
    }

    void colorCubeGestion(GameObject cube, int x, int z)
    {
        if (bordures.Contains(new Vector2(x, z)))
            cube.GetComponent<MeshRenderer>().material = snow;
        else if (effectiveBordures.Contains(new Vector2(x, z)))
            cube.GetComponent<MeshRenderer>().material = rock;
        else if (!riverMatrix[x, z])
        {
            if (cube.transform.position.y < 4 * cube.transform.localScale.y)
                cube.GetComponent<MeshRenderer>().material = water;
            else if (cube.transform.position.y < 6 * cube.transform.localScale.y)
                cube.GetComponent<MeshRenderer>().material = sand;
            else if (cube.transform.position.y < 40 * cube.transform.localScale.y)
                cube.GetComponent<MeshRenderer>().material = grass;
            else if (cube.transform.position.y < 55 * cube.transform.localScale.y)
                cube.GetComponent<MeshRenderer>().material = rock;
            else
                cube.GetComponent<MeshRenderer>().material = snow;
        }
        else
            cube.GetComponent<MeshRenderer>().material = water;
    }

    void resetColor()
    {
        foreach(Vector3 key in objects.Keys)
        {
            colorCubeGestion(objects[key], (int)key.x, (int)key.y);
        }
    }
}
