using XboxCtrlrInput;
using UnityEngine;
using System.Collections;

public class ControllerManager : MonoBehaviour
{
    public enum PlayerNumber { player_1, player_2, player_3, player_4 };

    public float moveSpeed = 20f;
    public float moveSpeedModifier;
    public float moveSpeedModifierDuration;
    public PlayerNumber playerNum;
    public int controllerNum;

    // Vibration Modifiers
    public float vibrationDuration;
    public float leftMotor;
    public float rightMotor;
    public float vibrationItensity;

    public bool canEat;
    private bool canVibrate;

    public Animator anim;
    private Vector3 moveDir;
    float eating;

    // Use this for initialization
    void Awake()
    {
        anim = GetComponent<Animator>();
        vibrationItensity = .5f;
        vibrationDuration = 3;
        leftMotor = 0f;
        rightMotor = 0f;
        gameObject.tag = "Player";
        moveDir = Vector3.zero;
        canEat = false;

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
        VibrationManager();
    }

    void LeftAxisManager()
    {
        Vector3 newPos = transform.position;
        float axisX = XCI.GetAxis(XboxAxis.LeftStickX, controllerNum);
        float axisY = XCI.GetAxis(XboxAxis.LeftStickY, controllerNum);
        float axis = Mathf.Abs(axisX) > Mathf.Abs(axisY) ? axisX : axisY;

        anim.SetFloat("Speed", Mathf.Abs(axis));

        float newPosX = newPos.x + (axisX * moveSpeed * Time.deltaTime);
        float newPosZ = newPos.z + (axisY * moveSpeed * Time.deltaTime);
        
        newPos = new Vector3(newPosX, transform.position.y, newPosZ);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(axisX, axisY) * Mathf.Rad2Deg, transform.eulerAngles.z);
        transform.position = newPos;
    }
    

    void VibrationManager()
    {
        if (canVibrate)
        {
            leftMotor = vibrationItensity;
            rightMotor = vibrationItensity;
            canVibrate = false;
        }

        leftMotor = Mathf.Lerp(leftMotor, 0f, Time.deltaTime * vibrationDuration);
        rightMotor = Mathf.Lerp(rightMotor, 0f, Time.deltaTime * vibrationDuration);

        XCI.SetVibration(controllerNum, leftMotor, rightMotor);
    }

    void EatManager()
    {
        if (XCI.GetButtonDown(XboxButton.A, controllerNum) && canEat)
        {
            anim.SetFloat("Eating", 1);
            XCI.SetVibration(controllerNum, 0, 0);
            canVibrate = true;
            rigidbody.AddForce(0f, 7.5f, 0f, ForceMode.Impulse);


        }
    }
}
