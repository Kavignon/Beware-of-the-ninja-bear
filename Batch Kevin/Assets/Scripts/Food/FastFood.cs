using UnityEngine;
using System.Collections;
using System;

public class FastFood : Food {

	public FastFoodScoreEnum foodScore;
    
	// Use this for initialization
	void Awake () {
        base.Awake();
		AttributeScore ();
        speedModifier = .5f;
        speedModifierDuration = 10f;
        healthRestored = 15f;
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
		Array values = Enum.GetValues(typeof(FastFoodScoreEnum));
		System.Random healthyFood = new System.Random();
		foodScore = (FastFoodScoreEnum)values.GetValue(healthyFood.Next(values.Length));
		pointValue =(int) foodScore;	

	}
}
