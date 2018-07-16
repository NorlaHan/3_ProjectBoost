using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    Vector3 pos;
    Rocket rocket;
	// Use this for initialization
	void Start () {
        pos = transform.position;
        rocket = GameObject.FindObjectOfType<Rocket>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(rocket.transform.position.x,pos.y,pos.z);
	}
}
