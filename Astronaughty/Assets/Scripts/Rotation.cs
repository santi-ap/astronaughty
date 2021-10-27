using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rotation : MonoBehaviour
{
    public bool clockWise;
    public float speed = 75f; //Speed that object will rotate at
    public Score scoreScript;

    // Start is called before the first frame update
    void Start()
    {
        scoreScript = GameObject.Find("ScoreCanvas").GetComponent<Score>();
    }

    // Update is called once per frame
    void Update()
    {
        float score = scoreScript.score;
        if (clockWise)
        {
            transform.Rotate(0, 0, -(speed * ((Mathf.Sqrt(score) / 100) + 1)) * Time.deltaTime);// rotate right
        }
        else
        {
            transform.Rotate(0, 0, (speed * ((Mathf.Sqrt(score) / 100) + 1)) * Time.deltaTime);// rotate left
        }
    }
}
