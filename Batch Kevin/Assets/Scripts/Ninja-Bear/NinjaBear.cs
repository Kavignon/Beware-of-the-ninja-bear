using UnityEngine;
using System.Collections;


public class NinjaBear : MonoBehaviour {
    public bool canBeSummoned=true;
    Animator ninjaBearAnimation;
    AudioSource ninjaBearAudio;
    public AudioClip smokeClip;
    public AudioClip damageClip;
    GameObject player; 								// Reference to the player GameObject.
    NewPlayerHealth playerHealth;                  // Reference to the player's health.
    float timer;
    float ninjaStrenght;
    //AttackEnum randomAttack;
    GameObject[] players;

    // Use this for initialization
    void Start()
    {
        canBeSummoned = false;
        ninjaStrenght = 35;
        timer += Time.deltaTime;
        players = GameObject.FindGameObjectsWithTag("Player");

    }

    public void PerformTestAttackOnPlayer()
    {
		int indexPlayer = Random.Range (0, players.Length);
		players[indexPlayer].GetComponent<Player>().playerHealth.TakeDamageFromBear((int)ninjaStrenght);
    }

	void PerformPatternAttack(Player p)
	{
		p.playerHealth.TakeDamageFromBear ((int)ninjaStrenght);
	}

	void Awake()
	{
		// Setting up the references.
		ninjaBearAnimation = GetComponent <Animator> ();
		ninjaBearAudio = GetComponent <AudioSource> ();
	}

	// Update is called once per frame
	void Update () {

		
	}

	public void SummonNinjaBear()
	{
		if (canBeSummoned) {
			ninjaBearAnimation.SetTrigger ("ThrowSmokeBomb");
			ninjaBearAudio.clip = smokeClip;
			PerformTestAttackOnPlayer();
		}
	}

	AttackOptionEnum RandomiseAttackOption(){
		System.Array values = System.Enum.GetValues(typeof(AttackOptionEnum));
		System.Random random = new System.Random();
		AttackOptionEnum randomAttack = (AttackOptionEnum)values.GetValue(random.Next(values.Length));
		return randomAttack;
	}


	void RandomSurpriseAttack(){
		//Random.Range does not seem to work
		//Surprising the players with automatic surprise attack at any time during the game (once)
		//Using the attack option of either one (random) or all of them.
		//SelectAttackPattern (RandomiseAttackOption ());
	}

	/*void SelectAttackPattern(AttackOptionEnum attackOption){
		if (attackOption.ToString () == AttackOptionEnum.AttackAllPlayers.ToString()) {
			foreach (Player p in players) {
				PerformPatternAttack (p);
			}
		} else if (attackOption.ToString () == AttackOptionEnum.AttackAPlayer.ToString()) {
			int selectedPlayer = Random.Range(0, 3);
			Player player = (Player)players.IndexOf(selectedPlayer,);
			PerformPatternAttack (player); //should be selectedPlayer !!!
		} else if (attackOption.ToString () == AttackOptionEnum.AttackFatPlayers.ToString()) {
			foreach (Player p in players) {
				if (p.playerHealth.isFat) {
					PerformPatternAttack (p);
				}
			}
		} else {
			foreach (Player p in players) {
				if (p.playerHealth.isHealthy) {
					PerformPatternAttack (p);
				}
			}
		}
		
	}*/
	/*

	//Activate the animation of throwing a smoke bomb
	void ThrowSmokeBomb(){
		ninjaBearAnimation.SetTrigger ("ThrowSmokeBomb");
		ninjaBearAudio.clip = smokeClip;
		TeleportNinjaInFrontPlayer();
		PerformAttack();
	}

	//Perform the selected randomise attack to surprise the players
	void PerformAttack(){
		timer = 0.0f;
		SelectRandomAttack();
		ninjaBearAnimation.SetTrigger (randomAttack.ToString());
		ninjaBearAudio.clip = damageClip;
		//basic behaviour
		playerHealth.TakeDamageFromBear ((int)ninjaStrenght);

	}


	//Summon the ninja bear to attack a player if it's possible
	void SummonNinjaBear()
	{
		if (canBeSummoned) {
			ThrowSmokeBomb();
		}
	}

	//Randomise the ninja's attack by going through the attack enumeration
	void SelectRandomAttack(){
		Array values = Enum.GetValues(typeof(AttackEnum));
		System.Random random = new System.Random();
		randomAttack = (AttackEnum)values.GetValue(random.Next(values.Length));

		switch (randomAttack) {

		case AttackEnum.BITE:
			ninjaStrenght*=(float)1.25;
			break;
		case AttackEnum.SCRATCH:
			ninjaStrenght*=(float)0.85;
			break;
		case AttackEnum.SHURIKEN:
			break;
		}
	}

	AttackOptionEnum RandomiseAttackOption(){
		Array values = Enum.GetValues(typeof(AttackOptionEnum));
		System.Random random = new System.Random();
		AttackOptionEnum randomAttack = (AttackOptionEnum)values.GetValue(random.Next(values.Length));
		return randomAttack;
	}

	void TeleportNinjaInFrontPlayer(){
		//To be implemented
	}

	void RandomSurpriseAttack(){
		//Random.Range does not seem to work
		//Surprising the players with automatic surprise attack at any time during the game (once)
		//Using the attack option of either one (random) or all of them.
		SelectAttackPattern (RandomiseAttackOption ());
	}

	void SelectAttackPattern(AttackOptionEnum attackOption){
		if (attackOption.ToString () == AttackOptionEnum.AttackAllPlayers.ToString()) {
						foreach (Player p in players) {
								PerformSurpriseAtttack (p);
						}
		} else if (attackOption.ToString () == AttackOptionEnum.AttackAPlayer.ToString()) {
            int selectedPlayer = Random.Range(0, 3);
			Player selectedPlayer = (Player)players.IndexOf(selectedPlayer);
			PerformSurpriseAtttack (wantedPlayer); //should be selectedPlayer !!!
		} else if (attackOption.ToString () == AttackOptionEnum.AttackFatPlayers.ToString()) {
			foreach (Player p in players) {
				if (p.playerHealth.isFat) {
					PerformSurpriseAtttack (p);
					}
			}
		} else {
			foreach (Player p in players) {
				if (p.playerHealth.isHealthy) {
					PerformSurpriseAtttack (p);
				}
			}
		}

	}

	void PerformSurpriseAtttack(Player player)
	{
		timer = 0.0f;
		SelectRandomAttack();
		ninjaBearAnimation.SetTrigger (randomAttack.ToString());
		ninjaBearAudio.clip = damageClip;
		player.playerHealth.TakeDamageFromBear ((int)ninjaStrenght);

	}
	*/
}
