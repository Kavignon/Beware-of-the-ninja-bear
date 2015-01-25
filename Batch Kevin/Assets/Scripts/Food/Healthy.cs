using UnityEngine;
using System.Collections;
using System;

public class Healthy : Food {
    
	public HealthyFoodScoreEnum foodScore;
	// Use this for initialization
	void Awake () {
        base.Awake();

        //pointValue = 200;
		AttributeScore ();
        speedModifier = 2f;
        speedModifierDuration = 10f;
        healthRestored = 5f;
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

	
	void AttributeScore()
	{
		Array values = Enum.GetValues(typeof(HealthyFoodScoreEnum));
		System.Random healthyFood = new System.Random();
		foodScore = (HealthyFoodScoreEnum)values.GetValue(healthyFood.Next(values.Length));
		pointValue =(int) foodScore;	}
}
