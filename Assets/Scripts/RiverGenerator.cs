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
        Debug.Log("Init makeRiverLine");
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
            Debug.Log("Dans le while");
            //terrain[startBloc.x, startBloc.y]
            if (distance >= distanceMax)
                arret = true;

            Vector2 nextBloc = getNextBloc(terrain, startBloc, 5);

            if (nextBloc.x != -1 && nextBloc.y != -1)
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
            //Cotés
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x + 1, (int)startBloc.y])
            {
                possibleBloc.Add(new Vector2(startBloc.x + 1, startBloc.y));
                blocFind = true;
            }
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x, (int)startBloc.y + 1])
            {
                possibleBloc.Add(new Vector2(startBloc.x, startBloc.y + 1));
                blocFind = true;
            }
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x - 1, (int)startBloc.y])
            {
                possibleBloc.Add(new Vector2(startBloc.x - 1, startBloc.y));
                blocFind = true;
            }
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x, (int)startBloc.y - 1])
            {
                possibleBloc.Add(new Vector2(startBloc.x, startBloc.y - 1));
                blocFind = true;
            }

            //Diagonales
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x + 1, (int)startBloc.y + 1])
            {
                possibleBloc.Add(new Vector2(startBloc.x + 1, startBloc.y + 1));
                blocFind = true;
            }
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x - 1, (int)startBloc.y + 1])
            {
                possibleBloc.Add(new Vector2(startBloc.x - 1, startBloc.y + 1));
                blocFind = true;
            }
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x - 1, (int)startBloc.y - 1])
            {
                possibleBloc.Add(new Vector2(startBloc.x - 1, startBloc.y - 1));
                blocFind = true;
            }
            if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x - 1, (int)startBloc.y + 1])
            {
                possibleBloc.Add(new Vector2(startBloc.x - 1, startBloc.y + 1));
                blocFind = true;
            }

            if (blocFind && r == 1)
                return possibleBloc[Random.Range(0, possibleBloc.Count)];
            else if(blocFind && r != 1)
            {
                return new Vector2(possibleBloc[Random.Range(0, possibleBloc.Count)].x - r + 1, possibleBloc[Random.Range(0, possibleBloc.Count)].y - r + 1);
            }

        }
        return new Vector2(-1, -1);
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
