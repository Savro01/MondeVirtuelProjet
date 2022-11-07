using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
public class MapGeneratorEditor : MonoBehaviour
{
    MapGenerator mapGen;

    void Start()
    { 
        MapGenerator mapGen = GetComponent<MapGenerator>();
        mapGen.GenerateMap();
    }
}
