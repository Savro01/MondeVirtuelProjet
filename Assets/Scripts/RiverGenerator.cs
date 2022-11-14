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
        
        Vector2 startBloc = listBordure[55];

        //Pour chaque bloc de départ
        //For(int i = 0; i < listBlocDepart; i++){}
        int distance = 0;
        int rayon = 10;
        while (distance < distanceMax)
        {
            Vector2 nextBloc = getNextBloc(terrain, startBloc, rayon);

            if (nextBloc != startBloc)
            {
                riverLineMatrix[(int)nextBloc.x, (int)nextBloc.y] = true;
                startBloc = nextBloc;
                distance++;
            }
            else
                break;
        }
        return riverLineMatrix;
    }

    Vector2 getNextBloc(float[,] terrain, Vector2 startBloc, int rayonMax)
    {
        Debug.Log("NEXT BLOC");
        Debug.Log(startBloc);
        List<Vector2> possibleBloc = new List<Vector2>();

        for(int r = 1; r <= rayonMax; r++)
        {
            for(int i = -rayonMax; i <= rayonMax; i++)
            {
                for (int j = -rayonMax; j <= rayonMax; j++)
                {
                    if((Mathf.Abs(i) == r || Mathf.Abs(j) == r) && (startBloc.x + i >= 0 && startBloc.x + i < terrain.GetLength(0)) && (startBloc.y + j >= 0 && startBloc.y + j < terrain.GetLength(1)))
                    {
                        if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x + i, (int)startBloc.y + j])
                        {
                            int blablou = (int)startBloc.x + i;
                            int blablouDeux = (int)startBloc.y + j;
                            Debug.Log("Add New bloc : " + blablou + " " + blablouDeux);
                            possibleBloc.Add(new Vector2(startBloc.x + i, startBloc.y + j));
                        }
                    }
                }
            }
            if (possibleBloc.Count > 0)
            {
                int random = Random.Range(0, possibleBloc.Count);
                Vector2 blocPossible = possibleBloc[random];
                Debug.Log(blocPossible);
                if (r == 1)
                {
                    Debug.Log("CHUI A COTE");
                    return blocPossible;
                }
                else
                {
                    //return new Vector2(possibleBloc[random].x - r + 1, possibleBloc[random].y - r + 1);
                    return blocPossible;
                }
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
