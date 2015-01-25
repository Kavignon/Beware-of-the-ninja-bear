using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {


    private Text text;

	public float _playerCurrentSpeed; //the player's current speed
	
	public float _playerMinSpeed=2.0f; // the minimal speed permitted for the player
	
	public float _playerMaxSpeed=25.0f; //the maximal permitted speed for the player
	
	private int x,y,z;  // player's position in space
	
	public int currentScore; // player's current score
	public NewPlayerHealth playerHealth;
	public int playerNumber;
	
	
	
	public Transform myTransform; //modifying the player's position 
	
	
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
	}
	
	void FixedUpdate()
	{
		//Impose a scope for the player's speed. 
		//It will be checked at every second to make sure.
		
		if (_playerCurrentSpeed > _playerMaxSpeed) {
			_playerCurrentSpeed=_playerMaxSpeed;
		}
		
		if (_playerCurrentSpeed < _playerMinSpeed) {
			_playerCurrentSpeed=_playerMinSpeed;
		}
	}




}
