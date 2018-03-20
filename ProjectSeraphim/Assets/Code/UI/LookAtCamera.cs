using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {


    Camera localCam;
    

	void Awake ()
    {
        localCam = Camera.main;
	}
	

	void FixedUpdate ()
    {
        Vector3 v = localCam.transform.position - this.transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(localCam.transform.position - v);
        transform.Rotate(0, 180, 0);
	}
}