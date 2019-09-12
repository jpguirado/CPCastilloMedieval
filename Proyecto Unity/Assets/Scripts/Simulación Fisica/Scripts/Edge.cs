using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTelas {

    public int vertexA;
    public int vertexB;
    

    public EdgeTelas(int vertexA, int vertexB)
    {
        if(vertexA < vertexB)
        {
            this.vertexA = vertexA;
            this.vertexB = vertexB;
        }
        else if (vertexB < vertexA)
        {
            this.vertexA = vertexB;
            this.vertexB = vertexA;
        }
    }

    /*
     * Métodos implementados para poder crear un HashSet de Edge y quitar las aristas repetidas. 
     */

    public override bool Equals(object obj)
    {
        EdgeTelas arista = (EdgeTelas)obj;        
        return (vertexA == arista.vertexA && vertexB == arista.vertexB) || vertexA == arista.vertexB && vertexB == arista.vertexA;
    }

    public override int GetHashCode()
    {
        int result = 31 * (vertexA + vertexB);
        return result;
    }
}

