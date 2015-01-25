using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{

    public GameObject player;
    public Transform[] spawnPoints;
    public int playerNumber;
    public ControllerManager cManager;

    // Use this for initialization
    void Start()
    {
        Spawn();

    }

    // Update is called once per frame
    void Spawn()
    {

        for (int i = 0; i <= spawnPoints.Length; i++)
        {
            GameObject go = Instantiate(player, spawnPoints[i].position, spawnPoints[i].rotation) as GameObject;
            cManager = go.GetComponent<ControllerManager>();
            switch (i+1)
            {
                case 1:
                    cManager.playerNum = ControllerManager.PlayerNumber.player_1;
                    break;
                case 2: cManager.playerNum = ControllerManager.PlayerNumber.player_2;
                    break;
                case 3: cManager.playerNum = ControllerManager.PlayerNumber.player_3;
                    break;
                case 4: cManager.playerNum = ControllerManager.PlayerNumber.player_4;
                    break;
            }
        }

        //GameObject go = Instantiate(playerPrefab, transform.position, Quaternion.identity) as GameObject;
        //go.name = "Player_" + playerNumber;
    }
}
