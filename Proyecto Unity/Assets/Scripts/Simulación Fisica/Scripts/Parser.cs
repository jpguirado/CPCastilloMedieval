using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Parser
{
    public Vector3[] verticesMalla;
    public Vector4[] indicesMalla;

    /*
     * Devuelve un Vector3[] con las posiciones de los vértices de la malla de tetraedros 
     */

    public Vector3[] GetVertexList(TextAsset mallaVertices)
    {
        string vertices = mallaVertices.text;
        string[] linea = vertices.Split("\n\r".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

        string[] positions = new string[10];
        string currentLine = linea[0];
        positions = currentLine.Split(' ');
        //print(positions[0]);
        int numVertex = int.Parse(positions[0]);
        verticesMalla = new Vector3[numVertex];


        for (int i = 0; i < numVertex; ++i)
        {

            string aux = Regex.Replace(linea[i + 1], @"\s+", " ");
            positions = aux.Split(' ');
            // We only use the first 3, the last 3 are normals which are not used.
            //print(positions[2] + " " + positions[3] + " " + positions[4] + "\n");
            verticesMalla[i] = new Vector3(float.Parse(positions[2]), float.Parse(positions[3]), float.Parse(positions[4]));
        }

        return verticesMalla;
    }

    /*
    * Devuelve un Vector4[] con las relaciones de indices para cada tetraedro de la malla.
    */
    public Vector4[] GetIndexList(TextAsset mallaIndices)
    {

        string indices = mallaIndices.text;
        string[] linea = indices.Split("\n\r".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
        string currentLine = linea[0];
        string[] positions = new string[10];
        positions = currentLine.Split(' ');
        //print(positions[0]);
        int numTetraedros = int.Parse(positions[0]);
        indicesMalla = new Vector4[numTetraedros];

        for (int i = 0; i < numTetraedros; i++)
        {
            string aux = Regex.Replace(linea[i + 1], @"\s+", " ");
            positions = aux.Split(' ');
            // We only use the first 3, the last 3 are normals which are not used.
            //print(positions[2] + " " + positions[3] + " " + positions[4] + " " + positions[5] + "\n");
            indicesMalla[i] = new Vector4(float.Parse(positions[2]), float.Parse(positions[3]), float.Parse(positions[4]), float.Parse(positions[5]));
        }

        return indicesMalla;
    }

}
