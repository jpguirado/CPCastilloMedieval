using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMovimientoCaballeros : MonoBehaviour {

    private Animator animatorPersonaje;
    public Camera cam;
    private bool ida;

    public UnityEngine.AI.NavMeshAgent agent;

    public Vector3 origen;
    public Vector3 destino;
    public Vector3 destinoOriginal;

    //Para activar el movimiento o no del personaje
    public bool moverse;

    //Para activar el que funcione clicando en la pantalla o no
    public bool destinoClick;

    //Para pintar gizmos o no
    public bool drawGizmos;

    
    void Start()
    {
        animatorPersonaje = this.GetComponent<Animator>();
        if (moverse)
        {
            agent.SetDestination(destino);
        }
        ida = true;
    }

    void Update()
    {
        //Codigo que controla el movimiento por clicks en pantalla

        if (destinoClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    agent.SetDestination(hit.point);
                    print(hit.point);
                }
            }
        }
        else
        {
            if (moverse)
            {
                //Si estas en la ida y ya estas llegando, vuelve al origen
                if (agent.remainingDistance <= 0.1f && ida)
                {
                    agent.SetDestination(origen);
                    ida = false;
                }

                //Si estas en la vuelta y ya estas llegando, vuelve al destino
                else if (agent.remainingDistance <= 0.1f && !ida)
                {
                    agent.SetDestination(destino);
                    ida = true;
                }
            }
        }

        //Si esta parado, animacion de idle
        if (agent.velocity.x == 0 && agent.velocity.y == 0 && agent.velocity.z == 0)
        {
            animatorPersonaje.SetFloat("MoveSpeed", 0.0f);

        }
        //Si se esta moviendio, animación de caminar
        else
        {
            animatorPersonaje.SetFloat("MoveSpeed", 1.0f);
        }

    }

    public void setDestino(Vector3 nuevoDestino)
    {
        agent.SetDestination(nuevoDestino);
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(origen, 1.0f);

            Gizmos.color = Color.red;

            Gizmos.DrawSphere(destino, 1.0f);
        }
    }
}
