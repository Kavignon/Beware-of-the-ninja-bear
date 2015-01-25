using UnityEngine;
using System.Collections;

public class FastFood : Food {
    
	// Use this for initialization
	void Awake () {
        base.Awake();

        pointValue = 200;
        speedModifier = .5f;
        speedModifierDuration = 10f;
        healthRestored = 1500;
	}
	
	// Update is called once per frame

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            TriggerEnter(col);
            Debug.Log("Healthy Pickup");
        }
    }
}
