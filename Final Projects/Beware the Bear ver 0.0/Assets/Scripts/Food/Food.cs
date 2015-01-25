using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class Food : MonoBehaviour
{
    public int pointValue;
    public float speedModifier;
    public float speedModifierDuration;
    public int healthRestored;

    private GameObject player;

    public int playerNumber;
    public bool canPickup;



    // Use this for initialization
    protected void Awake()
    {
        gameObject.tag = "Food";
        canPickup = false;
        playerNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        PickupManager();
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            player.GetComponent<ControllerManager>().canEat = false;
            playerNumber = 0;
            player.GetComponent<ControllerManager>().anim.SetFloat("Eating", 0);
        }
    }

    protected void TriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            player = col.gameObject;
            playerNumber = player.GetComponent<ControllerManager>().controllerNum;
            player.GetComponent<ControllerManager>().canEat = true;
            canPickup = true;

        }
    }

    void PickupManager()
    {
        if (canPickup)
        {
            if (XCI.GetButtonDown(XboxButton.A, playerNumber))
            {
                player.GetComponent<ControllerManager>().anim.SetFloat("Eating", 1);
                player.GetComponent<HealthManager>().currentHealth += healthRestored;
				AttributeScore();
                Destroy(gameObject);
            }
        }
    }

	public void AttributeScore(){
		}
}
