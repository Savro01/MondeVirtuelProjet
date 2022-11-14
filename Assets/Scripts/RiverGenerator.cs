using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : MonoBehaviour
{

    bool[,] riverLineMatrix;

    public int hauteurMax;
    public int distanceMax;
    public int tentativeMax;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void makeRiverLine(float[,] terrain, List<Vector2> listBordure)
    {
        /*Cr�er une matrice de la m�me taille que celle de la map
        Trouver un point de d�part parmis les les blocs �rivi�res� (hauteur < 4)
        Tant que hauteur_choisie < hauteur < max et que distance parcourue<distance_max et tentative
        Chercher le bloc le plus haut dans un rayon autour de ce bloc (rayon = 1)
        Si pas de bloc plus haut et rayon<rayonMax
        Alors on augmente le rayon de 1
        On cherche le bloc plus haut dans ce rayon
        Recommencer boucle
        Si un bloc plus haut
        Alors le bloc le plus haut devient un bloc rivi�re
        Si ce bloc est � une hauteur relative sup�rieure � 1
        Alors les blocs sous ce bloc deviennent rivi�re
        Le bloc le plus haut devient le nouveau bloc de d�part
        Recommencer la boucle*/

        //Cr�ation de la matrice rivi�re(droite)
        riverLineMatrix = new bool[terrain.GetLength(0), terrain.GetLength(1)];

        //Choix du/des blocs de d�part
        Vector2 startBloc = listBordure[Random.Range(0, listBordure.Count)];

        //Pour chaque bloc de d�part
        //For(int i = 0; i < listBlocDepart; i++){}
        bool arret = false;
        int tentative = 0;
        while (!arret)
        {
            //terrain[startBloc.x, startBloc.y]
            if (tentative >= tentativeMax)
                arret = true;

            //getHeightestBloc();

        }

            
            
    }

    Vector2 getHeightestBloc(float[,] terrain, Vector2 startBloc)
    {
        if (terrain[(int)startBloc.x, (int)startBloc.y] < terrain[(int)startBloc.x, (int)startBloc.y])
        {

        }
        return Vector2.zero;
    }
}
