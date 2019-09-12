using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionScriptIK : MonoBehaviour
{
    public Transform target; //Objetivo a recoger
    public Transform hand; //Mano donde se va a coger el objeto
    public GameObject objetoARecoger; //GameObject a recoger para poder añadir rigidbody y lanzarlo

    private Animator anim; //Animator del personaje
    private float weight; // Peso aumentado durante la animación para coger de forma más precisa el objeto
    private float lanzar; //Parametro controlado durante la animación de lanzar para saber cuando soltar el objeto.
    private bool lanzado = false;
    private bool añadido = false;

    public float speed = 10; //Velocidad de lanzamiento del objeto
    private bool pick = false; 

    // Use this for initialization
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        weight = anim.GetFloat("IKpickup"); //Parametro controlado durante la animación de recoger para controlar el peso del IK
        anim.SetIKPosition(AvatarIKGoal.RightHand, target.position);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
        anim.SetLookAtPosition(target.position);
        anim.SetLookAtWeight(weight);
        lanzar = anim.GetFloat("Lanzar"); //Parametro controlado durante la animación de recoger para controlar el lanzamiento del objeto
    }

    private void LateUpdate()
    {
        if (weight > 0.95) //Si ya esta cerca de lo objeto, lo coge y lo pone en la mano, actualizando los padres de su transformada
        {
            pick = true;
            target.parent = hand;
            Transform[] list = hand.GetComponentsInChildren<Transform>();
            foreach (Transform transform in list)
            {
                transform.localRotation = Quaternion.Euler(0, 0, -30);
            }
            target.localPosition = new Vector3(0.1642f, -0.0333f, -0.025f);
            target.localRotation = Quaternion.Euler(-53, 129, -123);
        }
        if (pick)
        {
            Transform[] list = hand.GetComponentsInChildren<Transform>();
            foreach (Transform transform in list)
            {
                transform.localRotation = Quaternion.Euler(0, 0, -30);
            }
            target.localPosition = new Vector3(0.1642f, -0.0333f, -0.025f);
            target.localRotation = Quaternion.Euler(35, 111, -258);
        }

        if (lanzar > 0.99) //Si el valor de la animacion de lanzar alcanza este punto.
        {
            pick = false;
            lanzado = true;
        }
        if (lanzado)
        {
            if (!añadido)
            {
                //Añadimos un RigidBody para que caiga y le añadimos una velocidad inicial
                objetoARecoger.AddComponent<Rigidbody>();
                añadido = true;
                target.parent = null;
                objetoARecoger.GetComponent<Rigidbody>().velocity = transform.forward * speed;
            }
            lanzado = false;
        }
    }
}
