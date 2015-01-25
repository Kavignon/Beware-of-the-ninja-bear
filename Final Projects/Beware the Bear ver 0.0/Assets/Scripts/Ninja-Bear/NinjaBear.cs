using UnityEngine;
using System.Collections;
public class NinjaBear : MonoBehaviour
{

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

	public float gameDurationTimer = 0;
	public float timeLimit = 10;// This is the time limit
	
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
		if (gameDurationTimer <= timeLimit)
		{
			gameDurationTimer+=Time.deltaTime;
			
			
		}
		if (gameDurationTimer >= 10) 
		{
			SummonNinjaBear();
		}
		
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
	/*
    public int waitingPeriodForSummoning = 5000; // A user shall wait 5 seconds before the ninja bear can be summoned

    public bool canBeSummoned;
    Animator ninjaBearAnimation;
    AudioSource ninjaBearAudio;
    public AudioClip smokeClip;
    public AudioClip damageClip;
    GameObject player; 								// Reference to the player GameObject.
    HealthManager playerHealth;                  // Reference to the player's health.
    bool playerInRange;                      	  // Whether player is within the trigger collider and can be attacked.
    float timer;
    float ninjaStrenght;
    AttackEnum randomAttack;
    Transform bearLocation;
    ArrayList players;
    Player wantedPlayer;


    // Use this for initialization
    void Start()
    {
        canBeSummoned = false;
        playerInRange = false;
        ninjaStrenght = 35;
        timer += Time.deltaTime;
        bearLocation = transform;
        player = new GameObject();

    }

    void Awake()
    {
        // Setting up the references.
        ninjaBearAnimation = GetComponent<Animator>();
        ninjaBearAudio = GetComponent<AudioSource>();
        //basic behavior
        player = GameObject.FindGameObjectWithTag("Player");
        //real behaviour
        players = new ArrayList()
		{
			GameObject.FindGameObjectWithTag ("Player1"),
			GameObject.FindGameObjectWithTag ("Player2"),
			GameObject.FindGameObjectWithTag ("Player3"),
			GameObject.FindGameObjectWithTag ("Player4")
		};

        playerHealth = player.GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time > timer)
        {
            canBeSummoned = true;
            timer = Time.time + waitingPeriodForSummoning;
        }
    }
    //Activate the animation of throwing a smoke bomb
    void ThrowSmokeBomb()
    {
        ninjaBearAnimation.SetTrigger("ThrowSmokeBomb");
        ninjaBearAudio.clip = smokeClip;
        TeleportNinjaInFrontPlayer();
        PerformAttack();
    }

    //Perform the selected randomise attack to surprise the players
    void PerformAttack()
    {
        timer = 0.0f;
        SelectRandomAttack();
        ninjaBearAnimation.SetTrigger(randomAttack.ToString());
        ninjaBearAudio.clip = damageClip;
        //basic behaviour
        //playerHealth.TakeDamageFromBear ((int)ninjaStrenght);

    }

    //Attack a player if he gets in contact with the ninja
    void OnTriggerEnter(Collider other)
    {
        // If the entering collider is the player...
        if (other.gameObject == player)
        {
            // ... the player is in range.
            playerInRange = true;
            //attack the player
            PerformAttack();
        }
    }

    //Summon the ninja bear to attack a player if it's possible
    void SummonNinjaBear()
    {
        if (canBeSummoned)
        {
            ThrowSmokeBomb();
        }
    }

    //Randomise the ninja's attack by going through the attack enumeration
    void SelectRandomAttack()
    {
        System.Array values = System.Enum.GetValues(typeof(AttackEnum));
        System.Random random = new System.Random();
        randomAttack = (AttackEnum)values.GetValue(random.Next(values.Length));

        switch (randomAttack)
        {

            case AttackEnum.BITE:
                ninjaStrenght *= (float)1.25;
                break;
            case AttackEnum.SCRATCH:
                ninjaStrenght *= (float)0.85;
                break;
            case AttackEnum.SHURIKEN:
                break;
        }
    }

    AttackOptionEnum RandomiseAttackOption()
    {
        System.Array values = System.Enum.GetValues(typeof(AttackOptionEnum));
        System.Random random = new System.Random();
        AttackOptionEnum randomAttack = (AttackOptionEnum)values.GetValue(random.Next(values.Length));
        return randomAttack;
    }

    void TeleportNinjaInFrontPlayer()
    {
        //To be implemented
    }

    void RandomSurpriseAttack()
    {
        //Random.Range does not seem to work
        //Surprising the players with automatic surprise attack at any time during the game (once)
        //Using the attack option of either one (random) or all of them.
        SelectAttackPattern(RandomiseAttackOption());
    }

    void SelectAttackPattern(AttackOptionEnum attackOption)
    {
        if (attackOption.ToString() == AttackOptionEnum.AttackAllPlayers.ToString())
        {
            foreach (Player p in players)
            {
                PerformSurpriseAtttack(p);
            }
        }
        else if (attackOption.ToString() == AttackOptionEnum.AttackAPlayer.ToString())
        {
            int selectedPlayer = Random.Range(0, 3);
            //hard coded value for now
            int selectedPlayerIndex = 2;
            //Player selectedPlayer = (Player)players.IndexOf(selectedPlayer); not working for now ...
            PerformSurpriseAtttack(wantedPlayer); //should be selectedPlayer !!!
        }
        else if (attackOption.ToString() == AttackOptionEnum.AttackFatPlayers.ToString())
        {
            foreach (Player p in players)
            {
                //if (p.playerHealth.isFat)
                //{
                    PerformSurpriseAtttack(p);
                //}
            }
        }
        else
        {
            foreach (Player p in players)
            {
                //if (p.playerHealth.isHealthy)
                //{
                    PerformSurpriseAtttack(p);
                //}
            }
        }

    }

    void PerformSurpriseAtttack(Player player)
    {
        timer = 0.0f;
        SelectRandomAttack();
        ninjaBearAnimation.SetTrigger(randomAttack.ToString());
        ninjaBearAudio.clip = damageClip;
        //player.playerHealth.TakeDamageFromBear ((int)ninjaStrenght);

    }
    */
}
