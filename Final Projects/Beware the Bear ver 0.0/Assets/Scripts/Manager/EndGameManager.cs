using UnityEngine;
using System.Collections;
using System;

public class EndGameManager : MonoBehaviour {


	public float gameDurationTimer = 0;
	public float timeLimit = 60;// This is the time limit
	public bool isEndGame;
	public GameObject[] currentPlayers;
	public int[] playerRanking;
	public float timer;
	public float reloadTime =5;
	public string[] playerScores;

	// Use this for initialization
	void Start () {
		isEndGame = false;
		currentPlayers = GameObject.FindGameObjectsWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		if (gameDurationTimer <= timeLimit && isEndGame==false)
		{
			gameDurationTimer+=Time.deltaTime;
		}

		if (IsOnePlayerRemaining ()) {
			EndTheGame();
		}

		if (gameDurationTimer >= timeLimit) 
		{
			EndTheGame();
			if (timer <= reloadTime && isEndGame==true)
			{
				timer+=Time.deltaTime;
			}
			else
			{
				Application.LoadLevel(Application.loadedLevel);
			}

		}
	}

	void EndTheGame()
	{
		isEndGame = true;
		gameDurationTimer = timeLimit;
		AddRemainingHealthToScore ();
		EstablishRanking ();
		ShowRanking ();
	}

	bool IsOnePlayerRemaining()
	{
		int countPlayerDead = 0;
		bool isOnePlayerRemaining = false;
		for (int i = 0; i < 4; i++)
		{
			if(countPlayerDead ==3)
			{
				isOnePlayerRemaining=true;
				break;
			}
			if (currentPlayers[i].GetComponent<Player>().playerHealth.currentHealth ==0)
			{
				countPlayerDead++;
			}
		}
		return isOnePlayerRemaining;
	}

	void EstablishRanking()
	{

		int[] playerScore = { 
			currentPlayers[0].GetComponent<Player>().currentScore,
			currentPlayers[1].GetComponent<Player>().currentScore, 
			currentPlayers[2].GetComponent<Player>().currentScore, 
			currentPlayers[3].GetComponent<Player>().currentScore};
		Array.Sort(playerScore);

		playerRanking = playerScore;
	}

	void ShowRanking()
	{
		for(int i=0;i>4;i++)
		{
			playerScores[i]="The player : "+(i+1)+ " has :" +currentPlayers[i].GetComponent<Player>().currentScore;
		}

	}

	void AddRemainingHealthToScore()
	{
		
		foreach(GameObject go in currentPlayers)
		{
			if(go.GetComponent<Player>().playerHealth.currentHealth >0)
			{
			go.GetComponent<Player>().currentScore+= go.GetComponent<Player>().playerHealth.currentHealth;
			}
		}
		
	}



}
