using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapGenerationCube : MonoBehaviour
{
    public Material grass;
    public Material water;
    public Material sand;
    public Material rock;
    public Material snow;

    Vector3[,] matriceTestMap = new Vector3[100, 100];
    // Start is called before the first frame update
    void Start()
    {
        initMatriceTest();
        creationMapCube();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void initMatriceTest()
    {
        for (int i = 0; i < matriceTestMap.GetLength(0); i++)
        {
            for (int j = 0; j < matriceTestMap.GetLength(1); j++)
            {
                float x = Random.Range(-50.0f, 50.0f);
                float y = Random.Range(-1.0f, 20.0f);
                float z = Random.Range(-50.0f, 50.0f);

                matriceTestMap[i, j] = new Vector3(x, y, z);
            }
        }
    }
    void creationMapCube()
    {
        Vector3 cubeData = Vector3.zero;
        GameObject cube;
        for (int i = 0; i < matriceTestMap.GetLength(0); i++)
        {
            for (int j = 0; j < matriceTestMap.GetLength(1); j++)
            {
                cubeData = matriceTestMap[i, j];
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(cubeData.x, cubeData.y, cubeData.z);
                cube.transform.parent = transform;
                colorCubeGestion(cube);
            }
        }
    }

    void colorCubeGestion(GameObject cube)
    {
        if (cube.transform.position.y < 0)
            cube.GetComponent<MeshRenderer>().material = water;
        else if (cube.transform.position.y < 2)
            cube.GetComponent<MeshRenderer>().material = sand;
        else if (cube.transform.position.y < 7)
            cube.GetComponent<MeshRenderer>().material = grass;
        else if (cube.transform.position.y < 15)
            cube.GetComponent<MeshRenderer>().material = rock;
        else
            cube.GetComponent<MeshRenderer>().material = snow;
    }
}
