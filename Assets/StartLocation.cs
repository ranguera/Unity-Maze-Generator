using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLocation : MonoBehaviour {

    public Transform Omnipos;
    public Transform[] startingPositions;

	// Use this for initialization
	void Start () {
        Omnipos.position = startingPositions[Random.Range(0, startingPositions.Length)].position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
