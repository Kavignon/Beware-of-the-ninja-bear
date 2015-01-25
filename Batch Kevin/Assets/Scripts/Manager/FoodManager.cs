using UnityEngine;
using System.Collections;

public class FoodManager : MonoBehaviour {
	public System.Array randomFoodArray;
	public Transform myTransform;
	int x,y,z;
	public GameObject food;
	public Transform[] spawnFoodPoints;
	public float spawnTime=0.1f;
	public int foodCount=0;


	IEnumerator Start()
	{
		GenerateFood();
		while (true) {
			while (spawnFoodPoints.Length>0) // Wait for food to be absorb
			{
				yield return null;
			}
			yield return new WaitForSeconds(2);
		}
	}

	void GenerateFood()
	{
		InvokeRepeating ("InstantiateFood", spawnTime,spawnTime);
		if (foodCount >= 3) {
			CancelInvoke ("InstantiateFood");
		}
	}

	void InstantiateFood()
	{
		int spawnPointIndex = Random.Range (0, spawnFoodPoints.Length);
		//Create an instance of the food prefab at the randomly selected spawn point's position and rotation.
		Instantiate (food, spawnFoodPoints[spawnPointIndex].position, spawnFoodPoints[spawnPointIndex].rotation);
		foodCount += 1;
	}



}
