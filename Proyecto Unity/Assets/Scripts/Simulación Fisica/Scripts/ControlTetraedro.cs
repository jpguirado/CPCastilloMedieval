using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ControlTetraedro : MonoBehaviour
{
    /*
    *
    * Text assets donde se guarda la información en texto correspondiente a la malla de tetraedros
    * que contiene a la malla del objeto.
    */
    public TextAsset mallaVertices;
    public TextAsset mallaIndices;

    //Arrays donde almacenaremos posición vertices malla de tetraedros y la relación de los indices de los mismos
    public Vector3[] ListaVertices;
    public Vector4[] ListaTetraedros;
    //Estructura de datos donde almacenaremos las aristas para que no se repitan.
    public HashSet<EdgeTelas> EdgeSet;


    public ControlTetraedro()
    {
        Paused = true;
        TimeStep = 0.001f;
        Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        IntegrationMethod = Integration.Explicit;
    }

    /// <summary>
    /// Integration method.
    /// </summary>
    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };

    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public float Stiffness = 1000;

    public float Mass = 0.03f;
    public float dampingNode = 0.1f;
    public float dampingSpring = 0.01f;
    public float densidadMaterial = 0.001f;
    public Vector3 Gravity;
    public Integration IntegrationMethod;
    public GameObject[] fijadores;

    #endregion


    #region OtherVariables

    public NodeTelas[] nodes;
    public List<MuellesTela> springs = new List<MuellesTela>();
    Mesh mesh;
    Vector3[] verticesMalla;
    public Punto[] puntos;

    #endregion


    #region MonoBehaviour

    void Start()
    {
        //Creación del objeto parser, cuyos métodos reciben el textAsset correspondiente y devuelven los arrays necesarios
        Parser parser = new Parser();
        this.ListaVertices = parser.GetVertexList(mallaVertices);
        this.ListaTetraedros = parser.GetIndexList(mallaIndices);
        
        //Creacion de variables

        mesh = GetComponent<MeshFilter>().mesh;
        verticesMalla = mesh.vertices;

        nodes = new NodeTelas[this.ListaVertices.Length];
        puntos = new Punto[verticesMalla.Length];

     
        //Metemos las aristas en un Hash Set para eliminar las duplicadas y no hacer muelles innecesarios.
        EdgeSet = new HashSet<EdgeTelas>();
        for (int i = 0; i < this.ListaTetraedros.Length; i++)
        {
            EdgeSet.Add(new EdgeTelas((int)this.ListaTetraedros[i].x - 1, (int)this.ListaTetraedros[i].w - 1));
            EdgeSet.Add(new EdgeTelas((int)this.ListaTetraedros[i].x - 1, (int)this.ListaTetraedros[i].y - 1));
            EdgeSet.Add(new EdgeTelas((int)this.ListaTetraedros[i].x - 1, (int)this.ListaTetraedros[i].z - 1));
            EdgeSet.Add(new EdgeTelas((int)this.ListaTetraedros[i].w - 1, (int)this.ListaTetraedros[i].z - 1));
            EdgeSet.Add(new EdgeTelas((int)this.ListaTetraedros[i].w - 1, (int)this.ListaTetraedros[i].y - 1));
            EdgeSet.Add(new EdgeTelas((int)this.ListaTetraedros[i].z - 1, (int)this.ListaTetraedros[i].y - 1));
        }
        
        /*
         * Transformación necesaria para que la malla se mantenga en el sitio correcto. 
         */
        for (int i = 0; i < verticesMalla.Length; i++)
        {
            verticesMalla[i].x = verticesMalla[i].x * transform.localScale.x;
            verticesMalla[i].y = verticesMalla[i].y * transform.localScale.y;
            verticesMalla[i].z = verticesMalla[i].z * transform.localScale.z;

            verticesMalla[i].x = verticesMalla[i].x + transform.position.x;
            verticesMalla[i].y = verticesMalla[i].y + transform.position.y;
            verticesMalla[i].z = verticesMalla[i].z + transform.position.z;
        }
        

        //Creacion de los nodos
        for (int i = 0; i < this.ListaVertices.Length; i++)
        {
            nodes[i] = new NodeTelas(this.ListaVertices[i], Mass, dampingNode);
            nodes[i].Manager = this;
            
            //Comprobación de si ese nodo que se acaba de crear esta fijado o no.
            foreach (GameObject box in fijadores)
            {
                Collider col = box.GetComponent<Collider>();
                if (col.bounds.Contains(this.ListaVertices[i]))
                {
                    nodes[i].Fixed = true;
                }
            }
        }

        //Creación de objetos punto por cada uno de los vertices de la malla
        for (int i = 0; i < verticesMalla.Length; i++)
        {
            puntos[i] = new Punto(verticesMalla[i]);
        }

        /*
         * Comprobación del tetraedro contenedor para cada uno de los vertices de la malla, se le asigna a su objeto punto correspondiente.
         */
        for (int i = 0; i < this.ListaTetraedros.Length; i++)
        {
            for (int j = 0; j < verticesMalla.Length; j++)
            {
                if (pointInsideTetrahedron(this.ListaVertices[(int)this.ListaTetraedros[i].x - 1], this.ListaVertices[(int)this.ListaTetraedros[i].y - 1],
                    this.ListaVertices[(int)this.ListaTetraedros[i].z - 1], this.ListaVertices[(int)this.ListaTetraedros[i].w - 1], verticesMalla[j]))
                {
                    puntos[j].indiceTetraedroContenedor = i;
                }
            }
        }

        //Creación de spring por cada objeto arista en nuestro EdgeSet. No se encuentran repetidas
        foreach (EdgeTelas arista in EdgeSet)
        {
            springs.Add(new MuellesTela(nodes[arista.vertexA], nodes[arista.vertexB], Stiffness, dampingSpring, arista));
        }

        /*
         * Asignación de la masa correspondiente a cada nodo y del volumen correspondiente a cada muelle.
         */ 

        for (int i = 0; i < this.ListaTetraedros.Length; i++)
        {
            //Asignación de la masa correspondiente a cada nodo en función del volumen del tetraedro que lo tiene como vértice y la densidad del material.

            Vector3 vec1 = (this.ListaVertices[(int)this.ListaTetraedros[i].y-1] - this.ListaVertices[(int)this.ListaTetraedros[i].x-1]);
            Vector3 vec2 = (this.ListaVertices[(int)this.ListaTetraedros[i].z-1] - this.ListaVertices[(int)this.ListaTetraedros[i].x-1]);
            Vector3 vec3 = (this.ListaVertices[(int)this.ListaTetraedros[i].w-1] - this.ListaVertices[(int)this.ListaTetraedros[i].x-1]);

            float volumen = Mathf.Abs(Vector3.Dot(Vector3.Cross(vec2, vec3), vec1)) / 6;
            float masa = volumen * densidadMaterial;

            nodes[(int)this.ListaTetraedros[i].x-1].Mass += masa;
            nodes[(int)this.ListaTetraedros[i].y-1].Mass += masa;
            nodes[(int)this.ListaTetraedros[i].z-1].Mass += masa;
            nodes[(int)this.ListaTetraedros[i].w-1].Mass += masa;


            //Asignación del volumen correspondinete a cada spring

            //Creamos una lista de edges temporal para almacenar las aristas del tetraedro que estamos tratando.
            List<EdgeTelas> listaEdgeTemporal = new List<EdgeTelas>();


            listaEdgeTemporal.Add(new EdgeTelas((int)this.ListaTetraedros[i].x - 1, (int)this.ListaTetraedros[i].w - 1));
            listaEdgeTemporal.Add(new EdgeTelas((int)this.ListaTetraedros[i].x - 1, (int)this.ListaTetraedros[i].y - 1));
            listaEdgeTemporal.Add(new EdgeTelas((int)this.ListaTetraedros[i].x - 1, (int)this.ListaTetraedros[i].z - 1));
            listaEdgeTemporal.Add(new EdgeTelas((int)this.ListaTetraedros[i].w - 1, (int)this.ListaTetraedros[i].z - 1));
            listaEdgeTemporal.Add(new EdgeTelas((int)this.ListaTetraedros[i].w - 1, (int)this.ListaTetraedros[i].y - 1));
            listaEdgeTemporal.Add(new EdgeTelas((int)this.ListaTetraedros[i].z - 1, (int)this.ListaTetraedros[i].y - 1));

            //Si alguna de las aristas sin repetir coincide con alguna del tetraedro que estamos tratando, le asignamos su parte del volumen del mismo.
            foreach (EdgeTelas edge in listaEdgeTemporal)
            {
                foreach (MuellesTela spring in springs)
                {
                    if (spring.edge.Equals(edge))
                    {
                        spring.volumen += volumen / 6;
                    }
                }
            }
        }

        /*
         * Calculo de los pesos de cada punto en función del volumen total de su tetraedro y del volumen que forma el tetraedro compuesto
         * por el punto y por los otros 3 nodos del tetraedro, excluyendo el vertice del tetraedro del que estamos calculando el peso.
         */

        for (int i = 0; i < puntos.Length; i++)
        {

            float volumenTotal = tetVolume(ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].x - 1], ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].y - 1],
                ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].z - 1], ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].w - 1]);

            //Calculo de Vi
            float V1 = tetVolume(puntos[i].pos, ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].y - 1],
                ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].z - 1], ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].w - 1]);
            float w1 = V1 / volumenTotal;

            float V2 = tetVolume(puntos[i].pos, ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].x - 1],
                ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].z - 1], ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].w - 1]);
            float w2 = V2 / volumenTotal;

            float V3 = tetVolume(puntos[i].pos, ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].x - 1],
                ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].y - 1], ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].w - 1]);
            float w3 = V3 / volumenTotal;

            float V4 = tetVolume(puntos[i].pos, ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].x - 1],
                ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].y - 1], ListaVertices[(int)ListaTetraedros[(puntos[i].indiceTetraedroContenedor)].z - 1]);
            float w4 = V4 / volumenTotal;


            puntos[i].setWeights(w1, w2, w3, w4);

        }

    }

    /*
     * FUNCIONES AUXILIARES
     */ 

    
    //Comprueba en un plano si el punto esta en el mismo lado que el vertice restante
    private bool SameSide(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 point)
    {
        Vector3 normal = Vector3.Cross((v1 - v2), (v3 - v1));
        float dotV4 = Vector3.Dot(normal, (v4 - v1));
        float dotPoint = Vector3.Dot(normal, (point - v1));

        return Mathf.Sign(dotV4) == Mathf.Sign(dotPoint);
    }

    //Se ha de comprobar para todos los planos del tetraedro.
    private bool pointInsideTetrahedron(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector3 point)
    {
        return SameSide(v1, v2, v3, v4, point) &&
            SameSide(v2, v3, v4, v1, point) &&
            SameSide(v3, v4, v1, v2, point) &&
            SameSide(v4, v1, v2, v3, point);
    }

    //Función de cálculo de volumen del tetraedro
    private float tetVolume(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        Vector3 p2p1 = (v2 - v1);
        Vector3 p3p1 = (v3 - v1);
        Vector3 p4p1 = (v4 - v1);

        Vector3 cross = Vector3.Cross(p3p1, p4p1);
        return Mathf.Abs(Vector3.Dot(p2p1, cross)) / 6;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
            Paused = !Paused;

    }

    public void FixedUpdate()
    {
        if (Paused)
            return; // Not simulating

        // Select integration method

        for (int i = 0; i < 10; i++)
        {
            switch (IntegrationMethod)
            {
                case Integration.Explicit: stepExplicit(); break;
                case Integration.Symplectic: stepSymplectic(); break;
                default:
                    throw new System.Exception("[ERROR] Should never happen!");
            }
        }
    }

    #endregion

    /// <summary>
    /// Performs a simulation step in 1D using Explicit integration.
    /// </summary>
    /// 

    private void stepExplicit()
    {

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].Force = Vector3.zero;
            nodes[i].ComputeForces();
        }

        foreach (MuellesTela spring in springs)
        {
            spring.ComputeForces();
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            if (!nodes[i].Fixed)
            {
                nodes[i].Pos += nodes[i].Vel * TimeStep;
                nodes[i].Vel += nodes[i].Force * TimeStep / nodes[i].Mass;
                verticesMalla[i] += this.TimeStep * nodes[i].Vel;
            }

            //Actualizamos la posición de cada punto en función de su peso correspondiente con cada nodo y actualizamos los vertices de la malla.
            for (int j = 0; j < puntos.Length; j++)
            {
                puntos[j].pos = nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].x - 1].Pos * puntos[j].pesos[0] + nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].y - 1].Pos * puntos[j].pesos[1] +
                    nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].z - 1].Pos * puntos[j].pesos[2] + nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].w - 1].Pos * puntos[j].pesos[3];

                verticesMalla[j] = puntos[j].pos;
            }
        }

        for (int i = 0; i < verticesMalla.Length; i++)
        {
            verticesMalla[i] = transform.InverseTransformPoint(verticesMalla[i]);
        }

        mesh.vertices = verticesMalla;

    }


    /// <summary>
    /// Performs a simulation step in 1D using Symplectic integration.
    /// </summary>
    private void stepSymplectic()
    {

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].Force = Vector3.zero;
            nodes[i].ComputeForces();
        }

        foreach (MuellesTela spring in springs)
        {
            spring.ComputeForces();
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            if (!nodes[i].Fixed)
            {
                nodes[i].Vel += nodes[i].Force * TimeStep / nodes[i].Mass;
                nodes[i].Pos += nodes[i].Vel * TimeStep;
            }

            //Actualizamos la posición de cada punto en función de su peso correspondiente con cada nodo y actualizamos los vertices de la malla.
            for (int j = 0; j < puntos.Length; j++)
            {
                puntos[j].pos = nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].x - 1].Pos * puntos[j].pesos[0] + nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].y - 1].Pos * puntos[j].pesos[1] +
                    nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].z - 1].Pos * puntos[j].pesos[2] + nodes[(int)ListaTetraedros[puntos[j].indiceTetraedroContenedor].w - 1].Pos * puntos[j].pesos[3];

                verticesMalla[j] = puntos[j].pos;
            }
        }


        for(int i = 0; i<verticesMalla.Length; i++)
        {
            verticesMalla[i] = transform.InverseTransformPoint(verticesMalla[i]);
        }


        mesh.vertices = verticesMalla;

    }


    /*
     *  DESCOMENTAR PARA PODER VER LOS GIZMOS
     *  
    public void OnDrawGizmos()
    {


        //ROJO = Posición vertices de la malla de tetraedros (FIJOS)
        Gizmos.color = Color.red;
        for (int i = 0; i < ListaVertices.Length; i++)
            Gizmos.DrawSphere(ListaVertices[i], 0.1f);

        //VERDE = Nodos creados, se mueven.
        Gizmos.color = Color.green;
        foreach (NodeTelas nodo in nodes)
        {
            Gizmos.DrawSphere(nodo.Pos, 0.1f);
        }

        //MAGENTA: Vertices de la malla y su movimientos ponderados.
        Gizmos.color = Color.magenta;
        for(int i = 0; i<puntos.Length; i++)
        {
            Gizmos.DrawSphere(puntos[i].pos, 0.1f);
        }


        //AZUL: Arsitas de la malla de tetraedros sin repetir, SPRINGS.
        Gizmos.color = Color.blue;
        for (int i = 0; i < ListaTetraedros.Length; i++)
        {
            foreach (EdgeTelas arista in EdgeSet)
            {
                Gizmos.DrawLine(ListaVertices[arista.vertexA], ListaVertices[arista.vertexB]);
            }


        }

      
    }
    */

}



