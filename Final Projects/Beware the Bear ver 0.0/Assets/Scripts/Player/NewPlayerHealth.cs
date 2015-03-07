using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class NewPlayerHealth : MonoBehaviour {
	public int startingHealth = 1000;
	public int currentHealth;
	public double lifePercentile;
	public Slider healthSlider;
	public Image Fill;
	public Color MaxHealthColor = Color.green;
	public Color MinHealthColor = Color.red;
	public float sanityDrainSpeed = 1000.0f; //# of milliseconds for 1 Health to drop.
	public string sanity = "Sanity: ";
	public bool isDead = false;
	public bool isHealthyPlayer =false;
	public Animator playerAnimation;
	public AudioSource playerAudio;
	public AudioClip deathClip;

	// Use this for initialization
	void Start () {
		if (Fill == null){
			Fill = GetComponent<Image>();
		}
		healthSlider.wholeNumbers = true;        // I dont want 3.4567 Health but 3 or 4
		healthSlider.minValue = 0f;
		healthSlider.maxValue = startingHealth;
		healthSlider.value = startingHealth;        // start with full health
	}
	

	void FixedUpdate(){
		//Thread.Sleep ((int)sanityDrainSpeed);
		TakeDamage ();
		EvaluateLifePercentile ();
		UpdateHealthBar ();
		UpdatePlayerCurrentState ();
	}

	
	void Awake()
	{
		// Setting up the references.
		playerAnimation = GetComponent <Animator> ();
		playerAudio = GetComponent <AudioSource> ();
		currentHealth = startingHealth;
	}

	void Death(){
		isDead = true;
		playerAnimation.SetTrigger ("Die");
		playerAudio.clip = deathClip;
		playerAudio.Play ();
		Destroy (this.gameObject);
	}

	public void TakeDamage ()
	{
		currentHealth =(int) Mathf.Clamp(currentHealth - ((1000 / sanityDrainSpeed)),0.0f,startingHealth);
		healthSlider.value = currentHealth;
		//ModifyHealthBarColor ();
		if(currentHealth <= 0 && !isDead)
		{
			Death ();
		}
	}

	public void TakeDamageFromBear(int damage)
	{
		currentHealth -= damage;
		healthSlider.value = currentHealth;
		//ModifyHealthBarColor ();
		if(currentHealth <= 0 && !isDead)
		{
			Death ();
		}
	}

	void UpdateHealthBar(){
		Fill = Color.Lerp(MinHealthColor, MaxHealthColor, lifePercentile);
	}


	void EvaluateLifePercentile()
	{
		lifePercentile = (currentHealth >= 0 && currentHealth<=startingHealth) ? (currentHealth / startingHealth) : 0;
	}

	void UpdatePlayerCurrentState()
	{
		if (lifePercentile > 0.6 && lifePercentile<=1) {
			isHealthyPlayer = true;
		} else {
			isHealthyPlayer = false;
		}

	}
}
