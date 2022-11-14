using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{

    bool[,] riverLineMatrix;

    public int distanceMax;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool[,] makeRiverLine(float[,] terrain, List<Vector2> listBordure)
    {
        //Création de la matrice rivière(droite)
        riverLineMatrix = initRiverMatrix(terrain);

        //Choix du/des blocs de départ
        Vector2 startBloc = listBordure[Random.Range(0, listBordure.Count)];

        //Pour chaque bloc de départ
        //For(int i = 0; i < listBlocDepart; i++){}
        bool arret = false;
        int distance = 0;
        while (!arret)
        {
            //terrain[startBloc.x, startBloc.y]
            if (distance >= distanceMax)
                arret = true;

            Vector2 nextBloc = getNextBloc(terrain, startBloc, 5);

            if (nextBloc != startBloc && !riverLineMatrix[(int)nextBloc.x, (int)nextBloc.y])
            {
                riverLineMatrix[(int)nextBloc.x, (int)nextBloc.y] = true;
                startBloc = nextBloc;
            }
            else
                distance = distanceMax;

            distance++;
        }
        return riverLineMatrix;
    }

    Vector2 getNextBloc(float[,] terrain, Vector2 startBloc, int rayonMax)
    {
        List<Vector2> possibleBloc = new List<Vector2>();
        bool blocFind = false;

        for(int r = 1; r < rayonMax; r++)
        {
            for(int i = -rayonMax; i < rayonMax; i++)
            {
                for (int j = -rayonMax; j < rayonMax; j++)
                {
                    if(Mathf.Abs(i) == r || Mathf.Abs(j) == r)
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
                int random = Random.Range(0, possibleBloc.Count);
                if (r == 1)
                {
                    return possibleBloc[random];
                }
                else
                {
                    //return new Vector2(possibleBloc[random].x - r + 1, possibleBloc[random].y - r + 1);
                    return startBloc;
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
