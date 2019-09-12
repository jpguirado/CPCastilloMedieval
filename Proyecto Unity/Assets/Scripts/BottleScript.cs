using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleScript : MonoBehaviour {


    public GameObject personaje;
    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = personaje.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        anim.SetTrigger("Recoger");
    }
}
