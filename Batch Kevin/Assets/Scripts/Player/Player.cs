using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {


	public float _playerCurrentSpeed; //the player's current speed

	public float _playerMinSpeed=2.0f; // the minimal speed permitted for the player

	public float _playerMaxSpeed=25.0f; //the maximal permitted speed for the player

	private int x,y,z;  // player's position in space

	public int currentScore; // player's current score
	public NewPlayerHealth playerHealth;



	public Transform myTransform; //modifying the player's position 
	public float barDisplay = 0; 
	private Vector2 _healthBarPosition = new Vector2(20,40); 
	private Vector2 _healthBarSize = new Vector2(60,20); 
	private Texture2D progressBarEmpty;
	private Texture2D progressBarFull;

	// Use this for initialization
	void Start () {
		//setting a random spawning point for the player
		x=y = Random.Range (-14, 14);
		z = 3;

		myTransform = transform;

		// Spawn point
		myTransform.position = new Vector3 (x, y, z);
		_playerCurrentSpeed = Random.Range (_playerMinSpeed, _playerMaxSpeed);
	}
	
	// Update is called once per frame
	void Update () {
		barDisplay = Time.deltaTime * (float)0.05;
	}
	//Affect the player's speed after absorbing food
	void AffectingSpeed(){

	}



}
