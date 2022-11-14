using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{

    bool[,] riverLineMatrix;

    public int distanceMax;

    public bool[,] makeRiverLine(float[,] terrain, List<Vector2> listBordure)
    {
        //Création de la matrice rivière(droite)
        riverLineMatrix = initRiverMatrix(terrain);

        //Choix du/des blocs de départ
        //Vector2 startBloc = listBordure[Random.Range(0, listBordure.Count)];
        

        //Pour chaque bloc de départ
        //For(int i = 0; i < listBlocDepart; i++){}
        int rayon = 15;

        for (int i = 0; i < 50; i++)
        {
            Vector2 startBloc = listBordure[Random.Range(0,listBordure.Count)];
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

        return riverLineMatrix;
    }

    void linkPath(Vector2 startBloc, Vector2 nextBloc)
    {
        Vector2 current = startBloc;

        while(current != nextBloc)
        {
            Vector2 diff = nextBloc - current;
            int signX = diff.x < 0 ? -1 : diff.x > 0 ? 1 : 0;
            int signY = diff.y < 0 ? -1 : diff.y > 0 ? 1 : 0;
            current = current + new Vector2(signX, signY);
            riverLineMatrix[(int)current.x, (int)current.y] = true;
        }
    }

    Vector2 getNextBloc(float[,] terrain, Vector2 startBloc, int rayonMax)
    {
        List<Vector2> possibleBloc = new List<Vector2>();

        //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //go.transform.position = new Vector3(startBloc.x, terrain[(int)startBloc.x, (int)startBloc.y] + 1, startBloc.y);

        for (int r = 1; r <= rayonMax; r++)
        {
            for(int i = -r; i <= r; i++)
            {
                for (int j = -r; j <= r; j++)
                {
                    if((Mathf.Abs(i) == r || Mathf.Abs(j) == r) && (startBloc.x + i >= 0 && startBloc.x + i < terrain.GetLength(0)) && (startBloc.y + j >= 0 && startBloc.y + j < terrain.GetLength(1)))
                    {
                        if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x + i, (int)startBloc.y + j])
                        {
                            possibleBloc.Add(new Vector2(startBloc.x + i, startBloc.y + j));
                        }
                    }
                }
            }
            if (possibleBloc.Count > 0)
            {
                return possibleBloc[Random.Range(0, possibleBloc.Count)];
            }
        }
        return startBloc;
    }

    bool[,] initRiverMatrix(float[,] terrain)
    {
        bool[,] mat = new bool[terrain.GetLength(0), terrain.GetLength(1)];
        for(int i=0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                mat[i, j] = false;
            }
        }
        return mat;
    }
}
