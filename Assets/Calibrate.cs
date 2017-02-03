using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuix;

public class Calibrate : MonoBehaviour {

    public SteamVROmniController omni;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space))
        {
            omni.AlignOmni();
            omni.Calibrate();
        }
	}
}
