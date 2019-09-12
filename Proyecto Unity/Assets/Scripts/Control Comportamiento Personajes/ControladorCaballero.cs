using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCaballero : MonoBehaviour
{

    //GameObject del fuego
    [SerializeField] private GameObject fuegoObjetivo;

    //Animator Personaje
    private Animator animatorPersonaje;

    //GameObject de los caballeros disponibles
    [SerializeField] private GameObject[] caballeros;

    //TIPOS, VARIABLES Y CONSTANTES PARA MAQUINA DE ESTADOS
    private enum TEstado { PATRULLANDO, CAMINARFUEGO, APAGANDOFUEGO };
    private TEstado estado = TEstado.PATRULLANDO;


    //Referencias a scripts que tenemos que controlar
    private ControlMovimientoCaballeros movCaballero;
    private ControladorFuego controladorObjetivo;

    //Variables que controlarán que tarde un tiempo determinado en apagar el fuego
    private bool esperarApagarFuego = false;
    private bool solo1rutina = false;

    //Solo se ejecute una vez el calculo de menor distancia del estado patrullando
    private bool Solo1VezPatrullando = true;

    
    void Start()
    {
        //Acceso al script del fuego para controlar su funcionamiento
        controladorObjetivo = fuegoObjetivo.GetComponent(typeof(ControladorFuego)) as ControladorFuego;
        if (controladorObjetivo == null)
            Debug.Log("ERROR: OBJETIVO NO CONFIGURADO");

        animatorPersonaje = this.GetComponent<Animator>();
    }

    
    void Update()
    {
        FSMCaballero();
    }

    void FSMCaballero()
    {
        switch (estado)
        {
            case TEstado.PATRULLANDO:

                //Si ha aparecido un fuego
                if (controladorObjetivo.HayFuego())
                {

                    /*
                     * Necesitamos saber cual de los soldados esta más cerca del fuego, para que vaya el que
                     * este más cerca y sea más eficiente la resolución de este problema
                     */
                    if (Solo1VezPatrullando)
                    {
                        GameObject caballeroMasCercano = caballeros[0];
                        float distanciaMenor = Vector3.Distance(caballeros[0].transform.position, controladorObjetivo.GetPosicionFuego());

                        for (int i = 1; i < caballeros.Length; i++)
                        {
                            float distancia = Vector3.Distance(caballeros[i].transform.position, controladorObjetivo.GetPosicionFuego());

                            if (distancia <= distanciaMenor)
                            {
                                caballeroMasCercano = caballeros[i];
                                distanciaMenor = distancia;
                            }

                        }
                        movCaballero = caballeroMasCercano.GetComponent<ControlMovimientoCaballeros>();
                        caballeroMasCercano.GetComponent<ControladorCaballero>().estado = TEstado.CAMINARFUEGO;
                        movCaballero.setDestino(controladorObjetivo.GetPosicionFuego());
                        Solo1VezPatrullando = false;

                    }
                }
                break;

            case TEstado.CAMINARFUEGO:

                movCaballero = this.GetComponent<ControlMovimientoCaballeros>();
                //Si hemos llegado al fuego
                if (Vector3.Distance(controladorObjetivo.GetPosicionFuego(), movCaballero.transform.position) <= 4.5f)
                {
                    this.GetComponent<ControladorCaballero>().estado = TEstado.APAGANDOFUEGO;
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    movCaballero.agent.Stop();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                }
                break;

            case TEstado.APAGANDOFUEGO:

                //Solo se debe empezar una vez la rutina, para eliminar comportamientos indeseados
                if (!solo1rutina)
                {
                    StartCoroutine(apagarFuego());
                    solo1rutina = true;
                    animatorPersonaje.SetTrigger("ApagandoFuego");
                }


                var lookPos = controladorObjetivo.GetPosicionFuego() - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);


                //Apagamos el fuego y le decimos al caballero que vuelva a su patrulla normal
                if (esperarApagarFuego)
                {
                    controladorObjetivo.ApagaFuego();
                    movCaballero.setDestino(movCaballero.destinoOriginal);
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    movCaballero.agent.Resume();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                    solo1rutina = false;
                    estado = TEstado.PATRULLANDO;
                    Solo1VezPatrullando = true;
                    esperarApagarFuego = false;
                    animatorPersonaje.SetTrigger("FuegoApagado");
                }
                break;
        }
    }


    private IEnumerator apagarFuego()
    {
        //En funcion del tamaño del fuego tardara más o menos.
        yield return new WaitForSeconds(controladorObjetivo.tamañofuego.x * 6.66f);
        esperarApagarFuego = true;
    }
}
