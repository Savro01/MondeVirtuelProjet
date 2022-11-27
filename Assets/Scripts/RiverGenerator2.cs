using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator2 : MonoBehaviour
{
    bool[,] riverLineMatrix;

    public int distanceMax;
    public int nbRiver;

    [Range(1,8)]
    public int rayon = 1;

    [Range(1, 8)]
    public int elevation = 1;


    List<Vector2> startBlocPossible;

    enum Direction { N, S, W, E, NW, NE, SW, SE };

    Dictionary<(int,int), List<((int,int),Direction)>> NeighboursGreaterThanCurrent;

    private void Start()
    {
        NeighboursGreaterThanCurrent = new Dictionary<(int, int), List<((int, int), Direction)>>();
    }


    public bool[,] makeRiversLine(float[,] terrain, List<Vector2> listBordure)
    {
        //Création de la matrice rivière(droite)
        riverLineMatrix = initRiverMatrix(terrain);
        startBlocPossible = new List<Vector2>(listBordure);
        
        for (int i = 0; i < nbRiver; i++)
        {
            //Si plusieurs fois le meme bloc, + grosse riviére
            Vector2 startBloc = startBlocPossible[Random.Range(0, startBlocPossible.Count)];
            //Vector2 startBloc = listBordure[i];
            startBlocPossible.Remove(startBloc);
            makeRiverLine(terrain, startBloc);
            
        }

        return riverLineMatrix;
    }

    public void makeRiverLine(float[,] terrain, Vector2 startBloc)
    {
        int distance = 0;

        while (distance < distanceMax)
        {
            Vector2 nextBloc = getNextBloc(terrain, startBloc, rayon);
            
            if (nextBloc != startBloc)
            {
                riverLineMatrix[(int)nextBloc.x, (int)nextBloc.y] = true;
                linkPath(startBloc, nextBloc);
                startBloc = nextBloc;
                distance++;
            }
            else
                break;
        }
    }

    void linkPath(Vector2 startBloc, Vector2 nextBloc)
    {
        Vector2 current = startBloc;

        while (current != nextBloc)
        {
            Vector2 diff = nextBloc - current;
            int signX = diff.x < 0 ? -1 : diff.x > 0 ? 1 : 0;
            int signY = diff.y < 0 ? -1 : diff.y > 0 ? 1 : 0;
            current = current + new Vector2(signX, signY);
            riverLineMatrix[(int)current.x, (int)current.y] = true;
        }
    }

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

                if (other.x > 0 && other.x < width && other.y > 0 && other.y < lenght)
                {
                    if(NeighboursGreaterThanCurrent.ContainsKey(((int)other.x, (int)other.y)))
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

    bool isInCircle(Vector2 center, Vector2 cubeCenter, int radius)
    {
        return Mathf.Pow((cubeCenter.x - center.x), 2) + Mathf.Pow((cubeCenter.y - center.y), 2) <= Mathf.Pow(radius, 2);
    }



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

                NeighboursGreaterThanCurrent[(i, j)] = new List<((int, int), Direction)>();

                if (i > 0 && j > 0 && i < width - 1 && j < length - 1)
                {
                    for (int k = -1; k <= 1; k++) {
                        for (int l = -1; l <= 1; l++)
                        {
                            if (Mathf.Abs(k) + Mathf.Abs(l) != 0)
                            {
                                if (terrain[i, j] < terrain[i + k, j + l] && terrain[i,j] % elevation == 0)
                                {
                                    Direction direction;
                                    if (k == -1) {
                                        direction = l == -1 ? Direction.NW : l == 0 ? Direction.W : Direction.SW; 
                                    }else if(k == 0)
                                    {
                                        direction = l == -1 ? Direction.N : Direction.S;
                                    }else
                                    {
                                        direction = l == -1 ? Direction.NE : l == 0 ? Direction.E : Direction.SE;
                                    }
                                    NeighboursGreaterThanCurrent[(i, j)].Add(((i + k, j + l), direction));
                                    //mat[i, j] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return mat;
    }
}
