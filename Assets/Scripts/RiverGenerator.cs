using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{
    //The river matrice, use to tell wich bloc is part of a river
    bool[,] riverLineMatrix;
    //A matrice used to tell wich bloc is accessible or not to create a river
    bool[,] riverLineIrradMatrix;

    //The number of river to create and the distance maximum of each river
    public int distanceMax;
    public int nbRiver;

    //The radius of minimal separation between two river
    [Range(4, 20)]
    public int rayonSeparation = 10;

    //The distance to get the next bloc of a river
    [Range(1,8)]
    public int rayon = 1;

    //Generate elevation lines each blocs of this parameter
    [Range(1, 8)]
    public int elevation = 1;

    [Range(0f, 1f)]
    public float threshold;

    //The list of possible bloc to start a river (= the list of borders)
    List<Vector2> startBlocPossible;

    //The list of each bloc of the map located with an height that correspond to a multiple of elevation parameter and near an other bloc located upper than one 
    Dictionary<(int,int), List<(int,int)>> NeighboursGreaterThanCurrent;

    private void Awake()
    {
        NeighboursGreaterThanCurrent = new Dictionary<(int, int), List<(int, int)>>();
    }

    /// <summary>
    /// Create all the river of the maps
    /// </summary>
    /// <param name="terrain">The matrice terrain</param>
    /// <param name="listBordure">The list of border near water</param>
    /// <returns></returns>
    public bool[,] makeRiversLine(float[,] terrain, List<Vector2> listBordure)
    {
        //Cr�ation de la matrice rivi�re(droite)
        riverLineMatrix = initRiverMatrix(terrain);
        riverLineIrradMatrix = initRiverMatrix(terrain);
        startBlocPossible = new List<Vector2>(listBordure);
        
       
        for (int i = 0; i < nbRiver; i++)
        {
            if (startBlocPossible.Count != 0)
            {
                //Get a bloc randomly in the possible start bloc (= the border bloc)
                Vector2 startBloc = startBlocPossible[Random.Range(0, startBlocPossible.Count)];
                //Update the list of possible start bloc and create a single river
                RemoveNearBorder(startBloc, startBlocPossible, 0);
                makeRiverLine(terrain, startBloc);
            }
            else
                break;
        }
        

        return riverLineMatrix;
    }

    /// <summary>
    /// Create a single river of the map
    /// </summary>
    /// <param name="terrain">The matrice terrain</param>
    /// <param name="startBloc">The start bloc of the river</param>
    public void makeRiverLine(float[,] terrain, Vector2 startBloc)
    {
        int distance = 0;
        River river = new River(threshold);
        river.addBlock(startBloc);

        while (distance < distanceMax)
        {
            Vector2 nextBloc = getNextBloc(terrain, startBloc, rayon);
            
            if (nextBloc != startBloc)
            {
                river.addBlock(nextBloc);
                startBloc = nextBloc;
                distance++;
            }
            else
                break;
        }


        if (river.getBlocs().Count > 1)
        {
            riverLineMatrix[(int)river.getBlocs()[0].x, (int)river.getBlocs()[0].y] = true;
            for (int i = 1; i < river.getBlocs().Count; i++)
            {
                riverLineMatrix[(int)river.getBlocs()[i].x, (int)river.getBlocs()[i].y] = true;
                irradBlocNear(terrain, (int)river.getBlocs()[i].x, (int)river.getBlocs()[i].y, rayonSeparation);
                linkPath(river.getBlocs()[i - 1], river.getBlocs()[i], terrain);
            }
        }

        foreach(Vector2 affluent in river.getAffluents().Keys)
        {
            rayon = 8;
            elevation = 2;
            linkPath(river.getAffluents()[affluent], affluent, terrain);
            makeRiverLine(terrain, affluent);

        }

    }

    /// <summary>
    /// Link the path between two bloc of a river
    /// </summary>
    /// <param name="startBloc">The first bloc to link</param>
    /// <param name="nextBloc">The second bloc to link</param>
    /// <param name="terrain">The matrice terrain</param>
    void linkPath(Vector2 startBloc, Vector2 nextBloc, float[,] terrain)
    {
        Vector2 current = startBloc;

        while (current != nextBloc)
        {
            Vector2 diff = nextBloc - current;
            int signX = diff.x < 0 ? -1 : diff.x > 0 ? 1 : 0;
            int signY = diff.y < 0 ? -1 : diff.y > 0 ? 1 : 0;

            if (Mathf.Abs(signX) == 1 && Mathf.Abs(signY) == 1 && (terrain[(int)current.x, (int)current.y] + 2 == terrain[(int)current.x + signX, (int)current.y + signY]))
            {
                riverLineMatrix[(int)current.x + signX, (int)current.y] = true;
            }

            current = current + new Vector2(signX, signY);
            riverLineMatrix[(int)current.x, (int)current.y] = true;
        }
    }

    /// <summary>
    /// Get the next bloc in the path of the river
    /// </summary>
    /// <param name="terrain">The matrice terrain</param>
    /// <param name="startBloc">The bloc before the bloc we search</param>
    /// <param name="radius">The radius of scan to find a new bloc</param>
    /// <returns></returns>
    Vector2 getNextBloc(float[,] terrain, Vector2 startBloc, int radius)
    {
        int width = terrain.GetLength(0);
        int lenght = terrain.GetLength(1);

        Vector2 nextBloc = new Vector2(-1, -1);
        float ratioKeep = -1;

        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                Vector2 other = new Vector2(startBloc.x + i, startBloc.y + j);

                if (other.x > 0 && other.x < width && other.y > 0 && other.y < lenght && !riverLineIrradMatrix[(int)other.x,(int)other.y])
                {
                    if(NeighboursGreaterThanCurrent.ContainsKey(((int)other.x, (int)other.y)) && NeighboursGreaterThanCurrent[((int)other.x, (int)other.y)].Count > 0)
                    {
                        bool res = isInCircle(startBloc, other, radius);
                        if (res)
                        {
                            float heightStart = terrain[(int)startBloc.x, (int)startBloc.y];
                            float heightOther = terrain[(int)other.x, (int)other.y];
                            float ratio = (heightOther - heightStart) / (Mathf.Abs(i) + Mathf.Abs(j));

                            if ((ratioKeep < 0 && heightStart < heightOther && ratio > 0) || (ratioKeep > ratio && heightStart < heightOther && ratio > 0))
                            {
                                nextBloc = other;
                                ratioKeep = ratio;
                                break;
                            }
                        }
                    }
                }           
            }
        }
        if (nextBloc == new Vector2(-1, -1) || (terrain[(int)nextBloc.x, (int)nextBloc.y] <= terrain[(int)startBloc.x, (int)startBloc.y]))
        {
            return startBloc;
        }
        return nextBloc;
    }

    /// <summary>
    /// Return if a cube is in the radius of the circle with another for origin
    /// </summary>
    /// <param name="center">The center of the circle</param>
    /// <param name="cubeCenter">The cube we want to know if he's inside</param>
    /// <param name="radius">The radius of the circle</param>
    /// <returns></returns>
    bool isInCircle(Vector2 center, Vector2 cubeCenter, int radius)
    {
        return Mathf.Pow((cubeCenter.x - center.x), 2) + Mathf.Pow((cubeCenter.y - center.y), 2) <= Mathf.Pow(radius, 2);
    }

    /// <summary>
    /// Initialise the river matrice with false everywhere
    /// </summary>
    /// <param name="terrain">The terrain matrice</param>
    /// <returns></returns>
    bool[,] initRiverMatrix(float[,] terrain)
    {
        bool[,] mat = new bool[terrain.GetLength(0), terrain.GetLength(1)];
        int width = mat.GetLength(0);
        int length = mat.GetLength(1);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                mat[i, j] = false;

                NeighboursGreaterThanCurrent[(i, j)] = new List<(int, int)>();

                if (i > 0 && j > 0 && i < width - 1 && j < length - 1)
                {
                    for (int k = -1; k <= 1; k++) {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (Mathf.Abs(k) + Mathf.Abs(l) != 0)
                            {
                                if (terrain[i, j] < terrain[i + k, j + l] && terrain[i,j] % elevation == 0)
                                {
                                    NeighboursGreaterThanCurrent[(i, j)].Add((i + k, j + l));
                                }
                            }
                        }
                    }
                }
            }
        }
        return mat;
    }

    /// <summary>
    /// Remove the current bloc and bloc around of it of the startbloc list
    /// </summary>
    /// <param name="startBloc">The bloc to remove</param>
    /// <param name="startBlocPossible">The startbloc list</param>
    /// <param name="index">The index of the fonction, to stop the recursivity</param>
    void RemoveNearBorder(Vector2 startBloc, List<Vector2> startBlocPossible, int index)
    {
        //Retire le bloc courant
        startBlocPossible.Remove(startBloc);
        index++;
        //Retire r�cursivement les blocs proches
        if (index < rayonSeparation)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector2 nextStartBloc = new Vector2(startBloc.x + i, startBloc.y + j);
                    if (startBlocPossible.Contains(nextStartBloc))
                        RemoveNearBorder(nextStartBloc, startBlocPossible, index);
                }
            }
        }
    }

    /// <summary>
    /// Set the innacessible bloc near a river in riverLineIrradMatrix
    /// </summary>
    /// <param name="terrain">The matrice Terrain</param>
    /// <param name="x">The position x in the matrice of the current bloc</param>
    /// <param name="z">The position y in the matrice of the current bloc</param>
    /// <param name="radius">The radius of the innaccessible zone </param>
    void irradBlocNear(float[,] terrain, int x, int z, int radius)
    {
        int width = terrain.GetLength(0);
        int lenght = terrain.GetLength(1);

        riverLineIrradMatrix[x, z] = true;
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if (x+i > 0 && x+i < width && z+j > 0 && z+j < lenght)
                    riverLineIrradMatrix[x+i, z+j] = true;
            }
        }
    }

    /// <summary>
    /// Return the modified list of borders of water
    /// </summary>
    /// <returns></returns>
    public List<Vector2> getStartBlocPossible()
    {
        return startBlocPossible;
    }
}
