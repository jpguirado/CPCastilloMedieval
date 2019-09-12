using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControladorRey : MonoBehaviour {

    //GameObject del enemigo
    [SerializeField] private GameObject enemigoObjetivo;
    
    //Variables para controlar movimiento por click
    public Camera cam;
    public bool destinoClick;

    //Variable modificable desde el editor para habilitar o no el movimiento
    public bool moverse;

    //Agente para manejar el movimiento;
    public NavMeshAgent agent;

    //Animator del personaje
    private Animator animatorPersonaje;

    //Posiciones que definiremos desde el editor para manejar el movimiento
    public Vector3[] posicionesPaseo;
    public Vector3 centroSuelo;

    //Variables para controlar maquina de estados
    private enum TEstado {PASEANDO,FORMACION}
    private TEstado estado = TEstado.PASEANDO;

    //Manejador del enemigo
    private ControladorEnemigo controladorEnemigo;

    //Variable usada para controlar los pasos del paseo del rey
    private int controladorPaseo;

	
	void Start () {

        //Obtenemos control sobre el script del enemigo
        controladorEnemigo = enemigoObjetivo.GetComponent(typeof(ControladorEnemigo)) as ControladorEnemigo;
        if (controladorEnemigo == null)
            Debug.Log("ERROR: OBJETIVO NO CONFIGURADO");

        //Obtenemos el animator
        animatorPersonaje = this.GetComponent<Animator>();

        if (moverse)
        {
            //Establecemos el prier tramo del paseo
            controladorPaseo = 0;
            setDestino(posicionesPaseo[controladorPaseo]);
        }
    }
	
	
	void Update () {
        FSMRey();
	}

    void FSMRey()
    {
        switch (estado)
        {
            case TEstado.PASEANDO:

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
                    if (agent.remainingDistance <= 0.1f)
                    {
                        controladorPaseo = ((controladorPaseo + 1) % (posicionesPaseo.Length));
                        setDestino(posicionesPaseo[controladorPaseo]);
                    }

                }
                if (controladorEnemigo.HayEnemigo())
                {
                    estado = TEstado.FORMACION;
                }
                break;

            case TEstado.FORMACION:
                setDestino(centroSuelo);
                
                if (agent.remainingDistance <= 0.01f)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }

                controlAnimaciones();

                //Si ha muerto ya el enemigo
                if (!controladorEnemigo.HayEnemigo())
                {
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
        //Si se esta moviendio, animación de caminar
        else
        {
            animatorPersonaje.SetFloat("MoveSpeed", 1.0f);
        }
    }
}
