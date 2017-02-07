using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamepad : MonoBehaviour {

    public Transform viveRig;
    public float speed;

    private float xAxis, zAxis;
    private Vector3 tmp;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        tmp = viveRig.forward;
        tmp.y = 0f;

        if( zAxis > .5f )
        {
            this.transform.Translate(tmp*speed);
        }
        else if (zAxis < -.5f)
        {
            this.transform.Translate(tmp * -1f * speed);
        }
    }
}
