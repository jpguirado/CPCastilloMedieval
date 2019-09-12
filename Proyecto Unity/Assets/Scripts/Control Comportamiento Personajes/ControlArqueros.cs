using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlArqueros : MonoBehaviour
{

    //GameObject del enemigo
    [SerializeField] private GameObject enemigoObjetivo;
    [SerializeField] private GameObject posicionApuntar;

    //GameObject de los arqueros disponibles
    [SerializeField] private GameObject [] arqueros;

    //TIPOS, VARIABLES Y CONSTANTES PARA MAQUINA DE ESTADOS
    private enum TEstado { PATRULLANDO,APUNTARENEMIGO,MATARENEMIGO};
    private TEstado estado = TEstado.PATRULLANDO;


    //Variables con las que accederemos al script del arquero encargado de matar al enemigo
    private ControlMovimientoArqueros movArquero;
    private ControladorEnemigo controladorObjetivo;


    //Nombre usado para debug
    public string nombre;

    //Referencia al propio script
    private ControlArqueros esteArquero;

    //Variables usadas para controlar las ejecuciones de las distintas instancias de los soldados
    //Y que funcionen segun lo esperado
    private bool arqueroEscogido = false;
    private bool esperarADisparar = false;
    private bool esperarHastaMuerto = false;

    
    void Start () {
        //Obtenemos control sobre el script del enemigo
        controladorObjetivo = enemigoObjetivo.GetComponent(typeof(ControladorEnemigo)) as ControladorEnemigo;
        if (controladorObjetivo == null)
            Debug.Log("ERROR: OBJETIVO NO CONFIGURADO");

    }
	
	void Update () {
        FSMArquero();
    }

    void FSMArquero()
    {
        switch (estado)
        {
            case TEstado.PATRULLANDO:

                //Se ha detectado un enemigo
                if (controladorObjetivo.HayEnemigo())
                {
                    /*
                     * Necestiamos saber que arquero esta mas cerca para que comience a atacar al enemigo
                     */

                    GameObject arqueroMasCercano = arqueros[0];
                    float distanciaMenor = Vector3.Distance(arqueros[0].transform.position, controladorObjetivo.GetPosicionEnemigo());

                    for (int i = 1; i<arqueros.Length; i++)
                    {
                        float distancia = Vector3.Distance(arqueros[i].transform.position, controladorObjetivo.GetPosicionEnemigo());

                        if(distancia <= distanciaMenor)
                        {
                            arqueroMasCercano = arqueros[i];
                            distanciaMenor = distancia;
                        }

                    }

                    movArquero = arqueroMasCercano.GetComponent<ControlMovimientoArqueros>();

                    esteArquero = arqueroMasCercano.GetComponent<ControlArqueros>();

                    //Necesitamos que este se ejecute sólo una vez para evitar conflictos de estados
                    if (!arqueroEscogido)
                    {
                        esteArquero.estado = TEstado.APUNTARENEMIGO;
                        arqueroEscogido = true;
                    }

                    
                }
                break;

            case TEstado.APUNTARENEMIGO:

                //Miramos hacia el enemigo
                movArquero = this.GetComponent<ControlMovimientoArqueros>();
                var lookPos = posicionApuntar.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

                //Paramos el movimiento
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                movArquero.agente.Stop();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

                //Pasamos al estado de matar
                estado = TEstado.MATARENEMIGO;
                
                break;

            case TEstado.MATARENEMIGO:
                
                //Para que solo ejecute la subtrutina y el trigger una vez
                if (!esperarADisparar)
                {
                    StartCoroutine(DispararEnemigo());
                    movArquero.disparar();
                    esperarADisparar = true;
                }

                mirarEnemigo();

                //4 segundos tarda en matarlo
                if (esperarHastaMuerto)
                {
                    //Vuelve a su estado normal
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
                    movArquero.agente.Resume();
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
                    movArquero = this.GetComponent<ControlMovimientoArqueros>();
                    movArquero.setDestino(movArquero.destinoOriginal);
                    arqueroEscogido = false;
                    movArquero.enemigoMuerto();
                    estado = TEstado.PATRULLANDO;
                    esperarHastaMuerto = false;
                    esperarADisparar = false;
                }
                break;
        }
    }

    private IEnumerator DispararEnemigo()
    {
        //10 segundos hasta matar al enemigo, le va bajando la vida
        for(int i = 0; i<10; i++)
        {
            yield return new WaitForSeconds(1);
            controladorObjetivo.TakeDamage(10);
        }

        esperarHastaMuerto = true;
    }

    void mirarEnemigo()
    {
        movArquero = this.GetComponent<ControlMovimientoArqueros>();
        var lookPos = posicionApuntar.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
    }

}
