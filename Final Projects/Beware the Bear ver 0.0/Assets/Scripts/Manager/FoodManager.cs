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
	public int instantiateCount=0;
	public float timer; 
	public float limit=10f;


	void Start()
	{
		InstantiateFood ();
	}

	void InstantiateFood()
	{
		if (foodCount == 25)
		{
			instantiateCount++;
			DeleteAndReload();
		}
		int spawnPointIndex = Random.Range (0, spawnFoodPoints.Length);

		//Create an instance of the food prefab at the randomly selected spawn point's position and rotation.
		Instantiate (food, new Vector3(Random.Range(-1,1),0.1,Random.Range(0,3)), spawnFoodPoints[spawnPointIndex].rotation);
		foodCount += 1;
	}

	void DeleteAndReload()
	{
		if (timer <= limit) {
			timer += Time.deltaTime;
		} 
		else if(instantiateCount<12){
			foodCount=0;
			foreach(Transform t in spawnFoodPoints)
			{
				Destroy(t);
			}
			InstantiateFood();
		}
	}
}
