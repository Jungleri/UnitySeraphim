using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteInterpolate : MonoBehaviour {


    private Vector3 targetPos;
    private Quaternion targetRot;

    private float dampingFactor = 1f;


    void Start ()
    {
        targetPos = this.transform.position;
        targetRot = this.transform.rotation;
	}


    public void SetTransform(Vector3 _pos, Quaternion _rot, bool interpolate)
    {
        if(interpolate)
        {
            targetPos = _pos;
            targetRot = _rot;
        }
        else
        {
            this.transform.position = _pos;
            this.transform.rotation = _rot;
        }
    }


    void Update()
    {
        this.transform.position = Vector3.Lerp(targetPos, targetPos, Time.deltaTime * dampingFactor);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * dampingFactor);
    }
}
