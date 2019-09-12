using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorFuego : MonoBehaviour {


    /// ESTOS PARAMETROS SON CONFIGURABLES DESDE EL INSPECTOR DE UNITY
    public float minSegundos;    // Minima cantidad de segundos que tarda en aparecer un fuego  
    public float maxSegundos;    // Maxima cantidad de segundos que tarda en aparecer un fuego 

    private float minSegundosOriginal;    
    private float maxSegundosOriginal;    

    //Posiciones en las que aparece el fuego y donde se guardará la elegida.
    public Vector3[] posiciones;
    private Vector3 posicionElegida;

    //GameObject del fuego
    public GameObject fuego;

    //Vector3 que usaremos para cambiar la escala del fuego para cambiarlo de tamaño
    public Vector3 tamañofuego;

    // Variables que no se deben tocar
    private bool generandoFuego = false;
    private bool hayFuego = false;

    //Controlar la aparición del fuego por teclado
    public bool generacionFuegoTeclado;

    void Start () {

        minSegundosOriginal = minSegundos;
        maxSegundosOriginal = maxSegundos;

        HazInvisible();
        posicionElegida = posiciones[Random.Range(0, posiciones.Length)];
	}
	
	void Update () {

        //Si no hay fuego  y no estamos generandolo, lanzamos la rutina de generación

        if (!generacionFuegoTeclado)
        {
            minSegundos = minSegundosOriginal;
            maxSegundos = maxSegundosOriginal;
            if (!generandoFuego && !hayFuego)
                StartCoroutine(GeneradorFuego());

        }
        else //Se generara el fuego pulsando la tecla F
        {
            if (Input.GetKey(KeyCode.F))
            {
                minSegundos = 1.0f;
                maxSegundos = 1.0f;
                if (!generandoFuego && !hayFuego)
                    StartCoroutine(GeneradorFuego());
            }
        }

    }

    
    private IEnumerator GeneradorFuego()
    {
        generandoFuego = true;

        // Espera un tiempo aleatorio para generar un fuego nuevo
        yield return new WaitForSeconds(Random.Range(minSegundos, maxSegundos));

        // Genera nueva fuego pasado el tiempo random
        hayFuego = true;
        HazVisible();

        
        generandoFuego = false;
    }

    
    public void HazInvisible()
    {
        transform.position = new Vector3(transform.position.x, -50.0f, transform.position.z);
    }

    
    public void HazVisible()
    {
        float cantidadTamañoFuego = Random.Range(1.5f, 2.5f);
       
        tamañofuego = new Vector3(cantidadTamañoFuego, cantidadTamañoFuego, cantidadTamañoFuego);
        transform.localScale = new Vector3(cantidadTamañoFuego, cantidadTamañoFuego, cantidadTamañoFuego);

        transform.position = posicionElegida;
        print("Ha aparecido un fuego");
    }

    public bool HayFuego()
    {
        return hayFuego;
    }

    public Vector3 GetPosicionFuego()
    {
        return posicionElegida;
    }

    public void ApagaFuego()
    {
        hayFuego = false;
        HazInvisible();
        posicionElegida = posiciones[Random.Range(0, (posiciones.Length) - 1)];
    }
}
