using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamepad : MonoBehaviour {

    public Transform viveRig;
    public float speed;

    private float xAxis, zAxis;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        xAxis = Input.GetAxis("Horizontal");
        zAxis = Input.GetAxis("Vertical");

        if( zAxis > .5f )
        {
            this.transform.Translate(viveRig.forward*speed);
        }
        else if (zAxis < -.5f)
        {
            this.transform.Translate(viveRig.forward * -1f * speed);
        }
    }
}
