using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class River
/// </summary>
public class River
{
    //Position of main blocs allow to constitute the river
    List<Vector2> blocs;
    //Dictionary that contains for each affluent's position of bloc, the precedent position to make a link
    Dictionary<Vector2, Vector2> affluents;

    float threshold;

    /// <summary>
    /// River constructor
    /// </summary>
    public River(float threshold)
    {
        this.blocs = new List<Vector2>();
        this.affluents = new Dictionary<Vector2, Vector2>();
        this.threshold = threshold;
    }

    /// <summary>
    /// Allows to add the position of the new main bloc in the list 
    /// </summary>
    /// <param name="new_bloc">position of the new main bloc to add</param>
    public void addBlock(Vector2 new_bloc)
    {
        if (!blocs.Contains(new_bloc))
        {
            if (blocs.Count > 1)
            {
                //Check if the path between the position of the new bloc and the position of the bloc located in the position - 2 in the list
                //is shorter than the path between the position of the new bloc and the position of the bloc located in the position - 1
                if ((new_bloc - blocs[blocs.Count-2]).magnitude < (new_bloc - blocs[blocs.Count - 1]).magnitude)
                {
                    float random_value = Random.value;
                    if (random_value <= threshold)
                    {
                        affluents[blocs[blocs.Count - 1]] = blocs[blocs.Count - 2];
                    }
                    blocs.RemoveAt(blocs.Count - 1);
                }
            }
            blocs.Add(new_bloc);
        }
    }


    /// <summary>
    /// Allows to get the list of main blocs of the river
    /// </summary>
    /// <returns>list that constains the main blocs of the river</returns>
    public List<Vector2> getBlocs()
    {
        return this.blocs;
    }

    /// <summary>
    /// Allows to get the Dictionnary containing the position of blocs to make affulents of the river
    /// </summary>
    /// <returns>Dictionnary containing the blocs to make affulents of the river</returns>
    public Dictionary<Vector2, Vector2> getAffluents()
    {
        return this.affluents;
    }

}
