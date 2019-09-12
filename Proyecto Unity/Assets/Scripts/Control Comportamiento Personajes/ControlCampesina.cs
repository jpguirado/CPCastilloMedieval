using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlCampesina : MonoBehaviour {

    //GameObject del fuego
    [SerializeField] private GameObject fuegoObjetivo;

    //TIPOS, VARIABLES Y CONSTANTES PARA MAQUINA DE ESTADOS
    private enum TEstado {REPOSO,PASEANDO,HUYENDO};
    private TEstado estado;

    //Referencias a scripts que tenemos que controlar
    private ControladorFuego controladorObjetivo;

    //Variables para controlar movimiento por click
    public Camera cam;
    public bool destinoClick;

    //Variable modificable desde el editor para habilitar o no el movimiento
    public bool moverse;

    //Agente para manejar el movimiento;
    public NavMeshAgent agent;

    //Animator del personaje
    private Animator animatorPersonaje;

    //Variables para controlar el movimiento
    public Vector3 posicionHuida;
    public Vector3 origen;
    public Vector3 destino;

    //Variables para controlar movimiento y llamadas que deben producirse solo una vez
    private bool huyendo = false;
    private bool cambioReposoPaseando = false;
    private bool solo1LlamadaSubrutina = true;
    private bool ida = true;

    //Para dibujar gizmos de las posiciones
    public bool drawGizmos;

    void Start () {

        controladorObjetivo = fuegoObjetivo.GetComponent(typeof(ControladorFuego)) as ControladorFuego;
        if (controladorObjetivo == null)
            Debug.Log("ERROR: OBJETIVO NO CONFIGURADO");

        //Obtenemos el animator
        animatorPersonaje = this.GetComponent<Animator>();

        int eleccionEstado = Random.Range(0, 2);

        if (eleccionEstado == 0)
            estado = TEstado.REPOSO;
        else
            estado = TEstado.PASEANDO;

    }
	
	// Update is called once per frame
	void Update () {
        FSMCampesina();
	}


    void FSMCampesina()
    {
        switch (estado)
        {
            case TEstado.REPOSO:

                huyendo = false;
                controlAnimaciones();

                if (destinoClick)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            setDestino(hit.point);
                            print(hit.point);
                        }
                    }
                }

                if (moverse)
                {
                    setDestino(origen);

                    if (solo1LlamadaSubrutina)
                    {
                        StartCoroutine(TiempoCambioReposoPaseando());
                        solo1LlamadaSubrutina = false;
                    }
                    if (controladorObjetivo.HayFuego())
                    {
                        setDestino(posicionHuida);
                        estado = TEstado.HUYENDO;
                        solo1LlamadaSubrutina = true;
                    }
                    else if (cambioReposoPaseando)
                    {
                        setDestino(destino);
                        estado = TEstado.PASEANDO;
                        solo1LlamadaSubrutina = true;
                    }
                }

                break;

            case TEstado.PASEANDO:

                huyendo = false;
                controlAnimaciones();

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

                if (solo1LlamadaSubrutina)
                {
                    StartCoroutine(TiempoCambioReposoPaseando());
                    solo1LlamadaSubrutina = false;
                }
                if (controladorObjetivo.HayFuego())
                {
                    setDestino(posicionHuida);
                    estado = TEstado.HUYENDO;
                    solo1LlamadaSubrutina = true;
                }
                else if (!cambioReposoPaseando)
                {
                    setDestino(origen);
                    estado = TEstado.REPOSO;
                    solo1LlamadaSubrutina = true;
                }
                break;

            case TEstado.HUYENDO:
                agent.speed = 5;
                huyendo = true;
                controlAnimaciones();

                if (!controladorObjetivo.HayFuego())
                {
                    setDestino(origen);
                    estado = TEstado.PASEANDO;
                }
                break;
        }
    }

    public void setDestino(Vector3 nuevoDestino)
    {
        agent.SetDestination(nuevoDestino);
    }

    void controlAnimaciones()
    {
        //Si esta parado, animacion de idle
        if (agent.velocity.x == 0 && agent.velocity.y == 0 && agent.velocity.z == 0)
        {
            animatorPersonaje.SetFloat("MoveSpeed", 0.0f);

        }
        else if (huyendo) //Animación de correr
        {
            animatorPersonaje.SetFloat("MoveSpeed", 1.0f);
        }
        //Si se esta moviendio, animación de caminar
        else
        {
            animatorPersonaje.SetFloat("MoveSpeed", 0.33f);
        }
        
    }

    private IEnumerator TiempoCambioReposoPaseando()
    {
       //Espera entre 5 y 15 segundos para cambiar de estado
        yield return new WaitForSeconds(Random.Range(5, 15));
        if (cambioReposoPaseando)
            cambioReposoPaseando = false;
        else
            cambioReposoPaseando = true;

    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(origen, 1.0f);

            Gizmos.color = Color.red;

            Gizmos.DrawSphere(destino, 1.0f);

            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(posicionHuida, 1.0f);
        }
    }
}


