using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlMovimientoArqueros : MonoBehaviour {

    //Animator del personaje, referencia a la camara y bool para controlar la patrulla
    private Animator animatorPersonaje;
    public Camera cam;
    private bool ida;

    public NavMeshAgent agente;

    public Vector3 origen;
    public Vector3 destino;
    public Vector3 destinoOriginal;

    //Para activar el movimiento o no del personaje
    public bool moverse;

    //Para activar el que funcione clicando en la pantalla o no
    public bool destinoClick;

    
    void Start () {
        animatorPersonaje = this.GetComponent<Animator>();
        if (moverse)
        {
            agente.SetDestination(destino);
        }
        ida = true;
    }
	
	
	void Update () {

        //Codigo que controla el movimiento por clicks en pantalla

        if (destinoClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    agente.SetDestination(hit.point);
                    print(hit.point);
                }
            }
        }
        else
        {
            if (moverse)
            {
                //Si estas en la ida y ya estas llegando, vuelve al origen
                if (agente.remainingDistance <= 0.1f && ida)
                {
                    agente.SetDestination(origen);
                    ida = false;
                }

                //Si estas en la vuelta y ya estas llegando, vuelve al destino
                else if (agente.remainingDistance <= 0.1f && !ida)
                {
                    agente.SetDestination(destino);
                    ida = true;
                }

            }
        }

        //Si esta parado, animacion de idle
        if(agente.velocity.x == 0 && agente.velocity.y == 0 && agente.velocity.z == 0)
        {
            animatorPersonaje.SetFloat("MoveSpeed", 0.0f);

        }
        //Si se esta moviendio, animación de caminar
        else
        {
            animatorPersonaje.SetFloat("MoveSpeed", 1.0f);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            animatorPersonaje.SetTrigger("DispararEnemigo");
        }
    }


    public void setDestino(Vector3 nuevoDestino)
    {
        agente.SetDestination(nuevoDestino);
    }


    public void disparar()
    {
        animatorPersonaje.SetTrigger("DispararEnemigo");
    }

    public void enemigoMuerto()
    {
        animatorPersonaje.SetTrigger("EnemigoMuerto");
    }

}
