using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ControladorEnemigo : MonoBehaviour
{
    /// ESTOS PARAMETROS SON CONFIGURABLES DESDE EL INSPECTOR DE UNITY
    public float minSegundos = 1.0f;    // Minima cantidad de segundos que tarda en aparecer un enemigo  
    public float maxSegundos = 5.0f;    // Maxima cantidad de segundos que tarda en aparecer un enemigo 

    private float minSegundosOriginal;    
    private float maxSegundosOriginal;    

    //Posiciones donde puede aparecer y la escogida
    public Vector3[] posiciones;
    private Vector3 posicionElegida;

    //Game object del enemigo
    public GameObject enemigo;

    // Variables que no se deben tocar
    private bool generandoEnemigo = false;
    private bool hayEnemigo = false;

    //Saludo maxima y salud actual
    public const int maxHealth = 100;
    public int currentHealth = maxHealth;

    //Barra de vida
    public RectTransform healthBar;

    //Animator del enemigo
    private Animator animatorEnemigo;

    //Agente para manejar el movimiento;
    public NavMeshAgent agent;

    //Posición a la que se dirigirá el enemigo cuando aparezca
    public Vector3 posicionAtaque;

    //Bool que controla si la aparicion se va a producir cuando pulses la tecla E
    public bool generaciónEnemigoTeclado;

  
    void Start()
    {
        minSegundosOriginal = minSegundos;
        maxSegundosOriginal = maxSegundos;

        //Ponen la animación idle del enemigo
        animatorEnemigo = this.GetComponent<Animator>();
        animatorEnemigo.SetFloat("MoveSpeed", 0.0f);

        posicionElegida = posiciones[Random.Range(0, posiciones.Length)];
       
        HazInvisible();
    }

    
    void Update()
    {
        
        if (!generaciónEnemigoTeclado)
        {
            minSegundos = minSegundosOriginal;
            maxSegundos = maxSegundosOriginal;
            if (!generandoEnemigo && !hayEnemigo)
                StartCoroutine(GeneradorEnemigo());
        }
        else //Para producir la aparición del enemigo por teclado
        {
            if (Input.GetKey(KeyCode.E))
            {
                minSegundos = 1.0f;
                maxSegundos = 1.0f;
                if (!generandoEnemigo && !hayEnemigo)
                    StartCoroutine(GeneradorEnemigo());
            }
        }
    }

    //Recibir daño
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EnemigoDerrotado();
            EnemigoDerrotado();
        }

        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }


    private IEnumerator GeneradorEnemigo()
    {
        generandoEnemigo = true;

        // Espera un tiempo aleatorio para generar un fuego nuevo
        yield return new WaitForSeconds(Random.Range(minSegundos, maxSegundos));

        // Genera nueva fuego pasado el tiempo random
        hayEnemigo = true;
        HazVisible();

        generandoEnemigo = false;
    }



    //Hace desaparecer al enemigo deshabilitando su render
    public void HazInvisible()
    {
        //Activamos la animaciond de idle
        animatorEnemigo.SetFloat("MoveSpeed", 0.0f);
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
        this.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        this.GetComponentInChildren<Canvas>().enabled = false;
    }


    public void HazVisible()
    {
        //Activamos la animacion de andar
        animatorEnemigo.SetFloat("MoveSpeed", 1.0f);
        this.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        this.GetComponentInChildren<Canvas>().enabled = true;
        transform.position = posicionElegida;

        //Situa al agente del navmeshes en la posición del enemigo
        agent.Warp(posicionElegida);
        agent.SetDestination(posicionAtaque);
        print("Ha aparecido un enemigo");
    }

    //Indica si hay enemigo
    public bool HayEnemigo()
    {
        return hayEnemigo;
    }

    //Devuelve la posicion del enemigo 
    public Vector3 GetPosicionEnemigo()
    {
        return posicionElegida;
    }

    //Cuando el enemigo esta muerto
    public void EnemigoDerrotado()
    {
        hayEnemigo = false;
  
        posicionElegida = posiciones[Random.Range(0, posiciones.Length)];
        HazInvisible();

        currentHealth = 100;
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
        print("El enemigo ha sido derrotado");
    }

}
