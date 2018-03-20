using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Actor))]
public class PlayerController : MonoBehaviour
{
    bool bIsLocal = false;
    float baseMoveSpeed = 1;
    Rigidbody rb;



    void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	

	void Update ()
    {
	    if(Input.GetAxisRaw("Vertical") != 0f || Input.GetAxisRaw("Horizontal") != 0f)
        {
            float movementModifier = 0.5f;

            Vector3 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if(Input.GetAxis("MoveModifier") != 0f)
            {
                movementModifier = 1f;
            } 

            ControllerMove(move, movementModifier);
        }
        Debug.Log(Input.GetAxisRaw("Vertical"));
	}

    void ControllerMove(Vector2 dir, float speed)
    {
        Vector3 targetPos = new Vector3(dir.x + this.transform.position.x, this.transform.position.y, dir.y + this.transform.position.z);
        if(Vector3.Angle(this.transform.forward, targetPos) > 60)
        {
            targetPos *= 0.5f;
        }
        Debug.Log(targetPos);
        rb.MovePosition(targetPos);
    }
}
