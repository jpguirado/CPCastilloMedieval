using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTelaComparer : IComparer<EdgeTelas>
{
    public int Compare(EdgeTelas a, EdgeTelas b)
    {
        if (a.vertexA > b.vertexA)
        {
            return 1;
        }
        else if (a.vertexA < b.vertexA)
        {
            return -1;
        }
        else if (a.vertexA == b.vertexA)
        {
            if (a.vertexB > b.vertexB)
            {
                return 1;
            }
            else if (a.vertexB < b.vertexB)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        return -1;
    }
}
