using UnityEngine;
using System.Collections;
using System;

public class EndGameManager : MonoBehaviour {
	public float gameDurationTimer = 0;
	public float timeLimit = 300;// This is the time limit
	public bool isEndGame;
	public GameObject[] currentPlayers;
	public int[] playerRanking;
	public float timer;
	public float reloadTime = 12.0f;
	public string[] playerScores;

	// Use this for initialization
	void Start () {
		isEndGame = false;
		currentPlayers = GameObject.FindGameObjectsWithTag ("Player");
	}

	void FixedUpdate()
	{
		if (gameDurationTimer <= timeLimit && isEndGame==false)
		{
			gameDurationTimer+=Time.deltaTime;
		}
		ManagePlayingPlayers ();
		ManageShowPlayerScore ();
	}

	void ManagePlayingPlayers()
	{
		if (IsOnePlayerRemaining ())
		{
			EndTheGame();
		}
	}

	void ManageShowPlayerScore()
	{
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
			if (currentPlayers[i].GetComponent<Player>().playerHealth.isDead)
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
		for(int i=1;i<5;i++)
		{
			playerScores[i]="The player : "+(i)+ " has :" +currentPlayers[i].GetComponent<Player>().currentScore;
		}
	}

	void AddRemainingHealthToScore()
	{
		foreach(GameObject go in currentPlayers)
		{
			if(go.GetComponent<Player>().playerHealth.currentHealth >0)
			{
				go.GetComponent<Player>().UpdateCurrentScore(go.GetComponent<Player>().playerHealth.currentHealth);
			}
		}
	}
}
