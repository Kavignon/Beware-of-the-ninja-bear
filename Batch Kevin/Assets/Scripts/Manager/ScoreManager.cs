using UnityEngine;
using System.Collections;
using System;

public class ScoreManager : MonoBehaviour {


	public GameObject[] players;
	public float gameDurationTimer = 0;
	public float timeLimit = 600; // This is the time limit
	public float findDoorTimeLimit=15;
	public float findDoorTimer;
	public float findTheDoorBonus=375;
	public int getThroughDoorBonus=250;
	public bool foundDoor=false;
	public bool getThroughDoor=false;
	public int findDoorSpeed=2;
	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag ("Player");
	}
	

	
	void Update(){
		gameDurationTimer += Time.deltaTime;
		if (gameDurationTimer >= timeLimit)
		{
			findDoorTimer+=Time.deltaTime;
			DecreaseFindDoorBonus();
		}
		AddEndGameBonus();
	}

	void AddRemainingHealthToScore()
	{

			foreach(GameObject go in players)
			{
				go.GetComponent<Player>().currentScore+= go.GetComponent<Player>().playerHealth.currentHealth;
			}

	}
	//We need better logic than this. 
	void AddHasFoundDoorBonus(){
		foreach (GameObject go in players) 
		{
			if(foundDoor)
			{
				go.GetComponent<Player>().currentScore+=(int)findTheDoorBonus;
				foundDoor=false;
			}
		}
	}

	void AddGetThroughDoorBonus()
	{
		foreach (GameObject go in players) {
			if (foundDoor)
			{
				go.GetComponent<Player> ().currentScore += getThroughDoorBonus;
				foundDoor = false;
			}
		}
	}

	void AddEndGameBonus()
	{
		if (gameDurationTimer >= timeLimit) {
			AddRemainingHealthToScore ();
			AddHasFoundDoorBonus ();
			AddGetThroughDoorBonus ();
		}
	
		InvokeRepeating("DecreaseFindDoorBonus", 0.0f, 1.0f);
	}

	void DecreaseFindDoorBonus()
	{
		findTheDoorBonus -= 25;
	}


}
