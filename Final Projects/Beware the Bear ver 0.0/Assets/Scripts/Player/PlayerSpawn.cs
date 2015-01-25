using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{

    public GameObject playerPrefab;
    public int playerNumber;
    public ControllerManager cManager;

    // Use this for initialization
    void Start()
    {
        playerPrefab = GameObject.Find("Player_" + playerNumber);
        cManager = playerPrefab.GetComponent<ControllerManager>();
        switch (playerNumber)
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

        playerPrefab.transform.position = transform.position;

        //GameObject go = Instantiate(playerPrefab, transform.position, Quaternion.identity) as GameObject;
        //go.name = "Player_" + playerNumber;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
