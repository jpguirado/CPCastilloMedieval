using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlGuardiaReal : MonoBehaviour {

    //GameObject del enemigo
    [SerializeField] private GameObject enemigoObjetivo;

    //GameObject del rey
    [SerializeField] private GameObject reyObjetivo;


    //Variables para controlar movimiento por click
    public Camera cam;
    public bool destinoClick;

    //Variables de las posiciones
    public Vector3 posicionOriginal;
    public Vector3 posicionDefensa;

    //Variable modificable desde el editor para habilitar o no el movimiento
    public bool moverse;

    //Agente para manejar el movimiento;
    public NavMeshAgent agent;

    //Animator del personaje
    private Animator animatorPersonaje;

    //Variables para controlar maquina de estados
    private enum TEstado { REPOSO, FORMACION}
    private TEstado estado = TEstado.REPOSO;

    //Manejador del enemigo
    private ControladorEnemigo controladorEnemigo;

    //Nombre para diferenciar
    public string nombre;

    private ControlGuardiaReal controladorIndividual;

   
    void Start () {

        //Obtenemos control sobre el script del enemigo
        controladorEnemigo = enemigoObjetivo.GetComponent(typeof(ControladorEnemigo)) as ControladorEnemigo;
        if (controladorEnemigo == null)
            Debug.Log("ERROR: OBJETIVO NO CONFIGURADO");

        //Obtenemos el animator
        animatorPersonaje = this.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        FSMGuardiaReal();
	}


    void FSMGuardiaReal()
    {
        switch (estado)
        {
            case TEstado.REPOSO:
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

#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                agent.Resume();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

                if (agent.remainingDistance <= 1.0f)
                {
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    agent.Stop();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                }

                if (controladorEnemigo.HayEnemigo())
                {
                    estado = TEstado.FORMACION;
                }

            break;

            case TEstado.FORMACION:

                controlAnimaciones();

#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                agent.Resume();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                setDestino(reyObjetivo.transform.position);

                if (Vector3.Distance(this.transform.position, reyObjetivo.transform.position) <= 3.0f)
                {

#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    this.GetComponent<ControlGuardiaReal>().agent.Stop();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

                    var lookPos = posicionOriginal - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

                }

                if (!controladorEnemigo.HayEnemigo())
                {
                    estado = TEstado.REPOSO;
                    
                    setDestino(posicionOriginal);
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
