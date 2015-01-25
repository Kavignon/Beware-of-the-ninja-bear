using UnityEngine;
using System.Collections;
using System;

public class ScoreManager : MonoBehaviour {


	public Array players;
	public float gameDurationTimer = 0;
	public float timeLimit = 10;// This is the time limit
	public float findDoorTimeLimit=15;
	public float findDoorTimer=0;
	public float findTheDoorBonus=375;
	public int getThroughDoorBonus=250;
	public bool foundDoor=false;
	public bool getThroughDoor=false;
	public int findDoorSpeed=2;
	public NinjaBear ninjaBear;

	bool canPerformFirstCallDone=true;	
	bool canPerformSecondCallDone=false;
	bool canPerformThirdCallDone=false;


	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag ("Player");
	
	}
	

	
	void FixedUpdate(){
		gameDurationTimer += Time.deltaTime;
		if (gameDurationTimer <= timeLimit)
		{
			findDoorTimer+=Time.deltaTime;


		}
		if (gameDurationTimer > timeLimit)
		{
			gameDurationTimer=timeLimit;
			findDoorTimer+=Time.deltaTime;
			if(findDoorTimer<=findDoorTimeLimit){
				DecreaseFindDoorBonus();
			}
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
		//if (gameDurationTimer >= timeLimit) {
			AddRemainingHealthToScore ();
			AddHasFoundDoorBonus ();
			AddGetThroughDoorBonus ();
		//}
	
		InvokeRepeating("DecreaseFindDoorBonus", 0.0f, 1.0f);
	}

	void DecreaseFindDoorBonus()
	{
		findTheDoorBonus -= 1;
	}

	void GiveNinjaBonus()
	{
		Instantiate (ninjaBear, new Vector3 (10, 10, 1), Quaternion.identity);
		foreach (Player p in players) 
		{
			if(p.currentScore >=5000)
			{
				if(canPerformFirstCallDone){
					ninjaBear.PerformTestAttackOnPlayer();
					ninjaBear.canBeSummoned=false;
					canPerformFirstCallDone=false;
					break;
				}
			}

			if(p.currentScore >=12500)
			{
				if(canPerformSecondCallDone)
				{
					ninjaBear.canBeSummoned=true;
					ninjaBear.PerformTestAttackOnPlayer();
					ninjaBear.canBeSummoned=false;
					break;
				}
			}
			if(p.currentScore >=25000)
			{
				if(canPerformThirdCallDone)
				{
					ninjaBear.canBeSummoned=true;
					ninjaBear.PerformTestAttackOnPlayer();
					ninjaBear.canBeSummoned=false;
					break;
				}
			}

		}
	}

}
