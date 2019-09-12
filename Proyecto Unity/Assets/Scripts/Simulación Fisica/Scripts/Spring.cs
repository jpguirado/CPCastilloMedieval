using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuellesTela
{

    #region InEditorVariables

    public float Stiffness;
    public NodeTelas nodeA;
    public NodeTelas nodeB;

    #endregion

    public ControlTetraedro Manager;

    public float Length0;
    public float Length;
    public Vector3 Pos;
    public float damping1 = 0.01f;
    public float damping2 = 0.01f;
    public EdgeTelas edge;
    public float volumen = 0.0f;



    public MuellesTela(NodeTelas node1, NodeTelas node2, float Stiffness, float dampingSpring, EdgeTelas edge)
    {
        this.nodeA = node1;
        this.nodeB = node2;
        this.Stiffness = Stiffness;
        Length0 = Length = (nodeA.Pos - nodeB.Pos).magnitude;
        this.damping1 = dampingSpring * this.Stiffness;
        this.damping2 = dampingSpring * this.Stiffness;
        this.edge = edge;
    }

    public void ComputeForces()
    {
        Vector3 dir = nodeA.Pos - nodeB.Pos;
        Length = dir.magnitude;
        dir = dir * (1.0f / Length);
        //La fuerza ahora viene determinada por el volumen
        Vector3 Force = -(volumen / Mathf.Pow(Length0, 2)) * Stiffness * (Length - Length0) * ((nodeA.Pos - nodeB.Pos) / Length);
        nodeA.Force += Force;
        nodeB.Force -= Force;
    }

}
