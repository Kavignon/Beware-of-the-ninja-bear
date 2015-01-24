using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	public float currentSpeed; //the player's current speed

	public float minSpeed=2.0f; // the minimal speed permitted for the player

	public float maxSpeed=25.0f; //the maximal permitted speed for the player

	private int x,y,z;  // player's position in space

	public int currentScore; // player's current score

	public Transform myTransform; //modifying the player's position 

	// Use this for initialization
	void Start () {
		//setting a random spawning point for the player
		x=y = Random.Range (-14, 14);
		z = 3;

		myTransform = transform;

		// Spawn point
		myTransform.position = new Vector3 (x, y, z);
		currentSpeed = Random.Range (minSpeed, maxSpeed);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//Affect the player's speed after absorbing food
	void AffectingSpeed(){

	}
}
