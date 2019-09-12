using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionScript : MonoBehaviour
{
    private Animator anim; //Animator del personaje
    public float damping = 0.15f;
    private float suma = 0.0f; //Variable encargada de controlar las transiciones entre animaciones idle.
    private float contadorTiempo = 0.0f;//Variable encargada de acumular tiempo paa saltar, si es necesario al cuarto idle por inactividad.
    private bool subiendo = true;

    // Use this for initialization
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Obtenemos la entrada vertical/horizontal de WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool corriendo = false; //Booleano para controlar saltos

        /*
        //Control de Teclas
        */

        //Correr, aumenta vertical * 2 para que lo inteprete el Blend Tree
        if (Input.GetKey(KeyCode.LeftShift))
        {
            vertical *= 2;
            corriendo = true;
        }
        //Sigilo
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.LeftAlt) && vertical>0.0f)
        {
            anim.SetBool("Stealth", true);
        }
        else
        {
            anim.SetBool("Stealth", false);
        }
        //Agachado
        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetTrigger("Agachado");
        }

        //Saltar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (vertical == 0) //Si esta parado
            {
                anim.SetTrigger("Saltar"); //Salto normal
            }
            else
            {
                if (vertical > 0 && corriendo) //Si esta corriendo
                    anim.SetTrigger("SaltoMov");
            }
        }

        this.transform.Rotate(0, horizontal, 0); //Facilita la rotación y la hace más realista
        //Valores que controlaran nuestro BlendTree en el animator del personaje.
        anim.SetFloat("Vertical", vertical, damping, Time.deltaTime);
        anim.SetFloat("Horizontal", horizontal, damping, Time.deltaTime);


        //Control de transiciones entre animaciones idle
        //Va variando entre un intervalo el valor del float que controla el BlendTree para que las transiciones entre idles sean suaves
        if (subiendo)
        {
            anim.SetFloat("IdleController", (suma += 0.002f), 0.001f, Time.deltaTime);
            if (suma >= 1.0f)
                subiendo = false;
        }
        else if (!subiendo)
        {
            anim.SetFloat("IdleController", (suma -= 0.002f), 0.001f, Time.deltaTime);
            if (suma <= 0.0f)
                subiendo = true;
        }


        if (!Input.anyKey)//Si no esta siendo presionada ninguna tecla
        {
            //Aumentamos el contador con el tiempo en segundos de cada frame
            contadorTiempo += Time.deltaTime;
            if(contadorTiempo > 30.0f)//Despues de 30 segundos inactivo, activa la animación de idle especial
            {
                anim.SetBool("30sTimer", true);
            }
        }
        else if(Input.anyKey) //Si se ha presionado alguna tecla, se vuelve al estado normal.
        {
            contadorTiempo = 0.0f;
            anim.SetBool("30sTimer", false);
        }
       
        

    }
}
