////Uncomment to activate Debug logs;
//#define DEBUG
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

#if (DEBUG)
    public int playerNumber;
    public bool canPickup;
#else
    private int playerNumber;
    private bool canPickup;
#endif


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
#if (DEBUG)
            Debug.Log("Wut?");
#endif
            canPickup = false;
            playerNumber = 0;
        }
    }

    protected void TriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
#if (DEBUG)
            Debug.Log("yip yip");
#endif
            player = col.gameObject;
            playerNumber = player.GetComponent<ControllerManager>().controllerNum;
            
            canPickup = true;
            
        }
    }
    
    void PickupManager()
    {
        if (canPickup)
        {
            if (XCI.GetButtonDown(XboxButton.X, playerNumber))
            {
                player.GetComponent<HealthManager>().currentHealth += healthRestored;
                Destroy(gameObject);
            }
        }
    }
}
