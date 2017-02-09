using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    public Transform player;
    public Transform maze;

    private int x, z;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        x = Mathf.RoundToInt(player.position.x / maze.localScale.x);
        z = Mathf.RoundToInt(player.position.z / maze.localScale.y);
        print("pos: " + x + " , " + z);
	}
}
