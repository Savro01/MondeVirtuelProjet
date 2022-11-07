using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMatrixTerrain : MonoBehaviour
{
    //Create a matrix containing height position for each cube in plan
    // noiseMap : matrix 1000x1000 containing values between 0 and 1
    // scaling : parameter to choose the resolution of terrain (divide the noiseMap), possible values : [1,2,4,8]
    // RETURN : the matrix of the terrain
    public float[,] CreateTerrainMatrix(float[,] noiseMap, int scaling)
    {

        //Creating a matrix
        float[,] terrainMatrix = new float[noiseMap.GetLength(0) / scaling, noiseMap.GetLength(1) / scaling];
           
        //On X
        for(int x = 0; x < terrainMatrix.GetLength(0); x++)
        {
            //On Z
            for(int z = 0; z < terrainMatrix.GetLength(1); z++)
            {
                //Getting height for the cube
                float height = noiseMap[x, z] * 100;

                //Setting value 
                terrainMatrix[x, z] = height;
            }
        }

        return terrainMatrix;
    }

    public void Start()
    {
        float[,] noiseMap = new float[100,100];

        for(int i = 0; i < noiseMap.GetLength(0); i++)
        {
            for(int j = 0; j < noiseMap.GetLength(0); j++)
            {              
                noiseMap[i,j] = Random.Range(0f,1f);
            }
        }

        float[,] terrainMatrix = CreateTerrainMatrix(noiseMap, 1);
        Debug.Log("Matrix created");

    }
}
