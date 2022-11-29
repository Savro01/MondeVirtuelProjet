using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River
{
    List<Vector2> blocs;

    public River()
    {
        this.blocs = new List<Vector2>();
    }

    public void addBlock(Vector2 new_bloc)
    {
        if (!blocs.Contains(new_bloc))
        {
            if (blocs.Count > 1)
            {
                if((new_bloc - blocs[blocs.Count-2]).magnitude < (new_bloc - blocs[blocs.Count - 1]).magnitude)
                {
                    blocs.RemoveAt(blocs.Count - 1);
                }
            }
            blocs.Add(new_bloc);
        }
    }

    public List<Vector2> getBlocs()
    {
        return this.blocs;
    }

}
