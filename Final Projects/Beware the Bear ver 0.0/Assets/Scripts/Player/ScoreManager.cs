using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{

    public Text text;
    public float smooth;
    int score;

    // Use this for initialization
    void Awake()
    {
        smooth = 2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        score = (int)Mathf.Lerp(score + 1, gameObject.GetComponent<Player>().currentScore, Time.deltaTime * smooth);

        text.text = score.ToString();
    }
}
