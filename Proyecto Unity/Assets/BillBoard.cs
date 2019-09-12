using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour {


    public Camera camara;

    void Update()
    {
        transform.LookAt(camara.transform);
    }
}
