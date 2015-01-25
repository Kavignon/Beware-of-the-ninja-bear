using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class NewPlayerHealth : MonoBehaviour {
	public int startingHealth = 100;
	public int currentHealth;
	public Slider healthSlider;
	public float sanityDrainSpeed = 1000.0f; //# of milliseconds for 1 Health to drop.
	public string sanity = "Sanity: ";
	public bool isDead = false;
	public bool isFat;
	public bool isCadaveric;
	public bool isHealthy;
	// Use this for initialization
	void Start () {
		EvaluatePlayerState ();
	}
	
	// Update is called once per frame
	//void Update () {
	void FixedUpdate(){
		//Thread.Sleep ((int)sanityDrainSpeed);
		TakeDamage ();
	}

	

	


	void Death(){
		isDead = true;
		//Ca serait a rajouter : http://unity3d.com/learn/tutorials/projects/survival-shooter/player-health
		/*
		anim.SetTrigger ("Die");
		playerAudio.clip = deathClip;
		playerAudio.Play ();
		*/
		//sinon
		Destroy (this.gameObject);
	}

	public void TakeDamage ()
	{

		currentHealth =(int) Mathf.Clamp(currentHealth - ((1000 / sanityDrainSpeed)),0.0f,startingHealth);
		healthSlider.value = currentHealth;
		ModifyHealthBarColor ();
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

	
	public void RestartLevel ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	void ModifyHealthBarColor(){

	}

	void EvaluatePlayerState()
	{
		if ((currentHealth <= 100) && (currentHealth > 75))
		{
			isHealthy=false;
			isFat = true;
			isCadaveric = false;
		}

		else if ((currentHealth < 75) && (currentHealth > 40))
		{
			isHealthy=true;
			isFat = false;
			isCadaveric = false;
		}
		else if((currentHealth < 40) && (currentHealth > 0)){
			isHealthy=false;
			isFat = false;
			isCadaveric = true;
		}
		else{
			Death();
		}
	}

}
