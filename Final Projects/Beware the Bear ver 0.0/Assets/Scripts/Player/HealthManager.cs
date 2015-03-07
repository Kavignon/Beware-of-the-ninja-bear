using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading;

public class HealthManager : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    public Slider healthSlider;
    public float sanityDrainSpeed = 1000.0f; //# of milliseconds for 1 Health to drop.
    public string sanity = "Sanity: ";
    public bool isDead = false;

    // Use this for initialization
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.maxValue = startingHealth;
    }

    // Update is called once per frame
    //void Update () {
    void FixedUpdate()
    {
        //Thread.Sleep ((int)sanityDrainSpeed);
        TakeDamage();

        if (DidConsumeFood())
        {
            ConsumeFood();
        }

    }


    void ConsumeFood()
    {
        /*
        If user consume food, increase player's health
        */
        ApplyFoodEffect();
    }

    bool DidConsumeFood()
    {
        return false;
    }

    void ApplyFoodEffect()
    {
    }

    void OnGui()
    {
        GUI.Box(new Rect(350, 10, Screen.width / 2 / (startingHealth / currentHealth), 25), sanity + currentHealth + "/" + startingHealth);
    }

    void Death()
    {
        isDead = true;
        //Ca serait a rajouter : http://unity3d.com/learn/tutorials/projects/survival-shooter/player-health
        /*
        anim.SetTrigger ("Die");
        playerAudio.clip = deathClip;
        playerAudio.Play ();
        */
        //sinon
        Destroy(this.gameObject);
    }

    public void TakeDamage()
    {
        currentHealth = (int)Mathf.Lerp(currentHealth, 0f, Time.deltaTime * sanityDrainSpeed/1000);
        healthSlider.value = currentHealth;
        ModifyHealthBarColor();
        if (currentHealth <= 0 && !isDead)
        {
            Death();
        }
    }


    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void ModifyHealthBarColor()
    {

    }

}
