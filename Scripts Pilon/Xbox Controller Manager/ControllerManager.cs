using XboxCtrlrInput;
using UnityEngine;
using System.Collections;

public class ControllerManager : MonoBehaviour
{
    public enum PlayerNumber { player_1, player_2, player_3, player_4 };

    public float moveSpeed = 2f;
    public float moveSpeedModifier;
    public float moveSpeedModifierDuration;
    public PlayerNumber playerNum;
    public int controllerNum;

    private bool canJump;

    // Use this for initialization
    void Awake()
    {
        gameObject.tag = "Player";
        switch (playerNum)
        {
            case PlayerNumber.player_1:
                controllerNum = 1;
                break;
            case PlayerNumber.player_2:
                controllerNum = 2;
                break;
            case PlayerNumber.player_3:
                controllerNum = 3;
                break;
            case PlayerNumber.player_4:
                controllerNum = 4;
                break;
        }

        XCI.DEBUG_LogControllerNames();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LeftAxisManager();
        JumpManager();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            canJump = true;
        }
    }

    void LeftAxisManager()
    {
        Vector3 newPos = transform.position;
        float axisX = XCI.GetAxis(XboxAxis.LeftStickX, controllerNum);
        float axisY = XCI.GetAxis(XboxAxis.LeftStickY, controllerNum);
        float newPosX = newPos.x + (axisX * moveSpeed * Time.deltaTime);
        float newPosZ = newPos.z + (axisY * moveSpeed * Time.deltaTime);

        newPos = new Vector3(newPosX, transform.position.y, newPosZ);
        transform.position = newPos;
    }

    void JumpManager()
    {
        if (XCI.GetButtonDown(XboxButton.A, controllerNum) && canJump)
        {
            canJump = false;
            rigidbody.AddForce(0f, 7.5f, 0f, ForceMode.Impulse);
        }
    }
}
