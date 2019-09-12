using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punto
{

    public Vector3 pos;
    public int indiceTetraedroContenedor;
    public float[] pesos;
    

    public Punto(Vector3 pos)
    {
        this.pos = pos;
        this.pesos = new float[4];
    }


    public void setWeights(float wn1, float wn2, float wn3, float wn4)
    {
        this.pesos[0] = wn1;
        this.pesos[1] = wn2;
        this.pesos[2] = wn3;
        this.pesos[3] = wn4;

    }
}
