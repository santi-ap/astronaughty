using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Score : MonoBehaviour
{
    public GameObject gameManager;
    public float airTimeThreshold;
    public float airTimeThresholdX4;
    public float airTimeThresholdX8;
    public float airTimeThresholdX16;
    public GameObject targetMultiplier;
    bool isX4 = false;
    bool isX8 = false;
    bool isX16 = false;
    public float score;// the score
    public float targetScore = 100f;//the score i need to get to to trigger animation
    float targetScoreIncrease; //support value for targetScore
    public GameObject playerObject;
    public GameObject ScoreText;
    GameObject scoreAnimation;
    public GameObject x2;
    public GameObject newRecordText;
    public float fadeSpeed; //used by the animation to make the score animation fade at a certain speed
    bool scaling = false; //used by the animation to make the score animation object to animate
    float airTimeTimer = 0f;
    bool flagForAirTime = false;
    int scoreCoefficient = 1;
    public int secondScoreCoefficient = 1;
    float currentY = 0f; //how many points has the player earned by moving upwards?
    public float currentYScore = 0f; //how many points has the player earned by moving upwards?
    float lastRecordedHighscore;
    public float airTimeTotalScore = 0f; //how many points has the player earned just by airtime bonus?

    void Start()
    {
        //this makes sure i get notified every time i surpass a multiplier of the target score
        targetScoreIncrease = targetScore;
        //now lets see whats the highscore
        if (PlayerPrefs.HasKey("HighScore"))
        {
            //save it for later
            lastRecordedHighscore = PlayerPrefs.GetFloat("HighScore");
        }
        else
        {
            //save it as 500
            lastRecordedHighscore = 500f;
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    void FixedUpdate()
    {
        //these lines check if the current y of the player is the highest recorded
        float currentYTemp = playerObject.transform.position.y;
        float airTimeDelta = 0f;//this is used by the airTimeTimer to see how many points have been scored
        if (currentYTemp > currentY)
        {
            //this is for the airTime
            //we get the difference between the old score and the new
            airTimeDelta = (currentYTemp - currentY) * 10;
            airTimeTotalScore += (airTimeDelta * (scoreCoefficient - 1)); //this number will be used by the score breakdown
            score += airTimeDelta * scoreCoefficient * secondScoreCoefficient;
            currentYScore = currentYTemp * 10;
            currentY = currentYTemp; //this is used to make sure the user only gets points when going above his last highest point
            this.SetScore(score);
        }

        if (score > targetScore)
        {

            if (score > 2000)
            {
                secondScoreCoefficient = 2;
                if (score > 6000)
                {
                    secondScoreCoefficient = 4;
                    if (score > 14000)
                    {
                        secondScoreCoefficient = 8;
                        if (score > 30000)
                        {
                            secondScoreCoefficient = 16;
                        }
                    }
                }
            }
            switch (secondScoreCoefficient)
            {
                case 1:
                    this.targetMultiplier.GetComponent<Text>().text = "x1";
                    this.targetMultiplier.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);

                    break;
                case 2:
                    this.targetMultiplier.GetComponent<Text>().text = "x2";
                    this.targetMultiplier.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);

                    break;
                case 4:
                    this.targetMultiplier.GetComponent<Text>().text = "x4";
                    this.targetMultiplier.GetComponent<Text>().color = new Color(1f, 0.93f, 0.67f, 1f);
                    break;
                case 8:
                    this.targetMultiplier.GetComponent<Text>().text = "x8";
                    this.targetMultiplier.GetComponent<Text>().color = new Color(1f, 0.75f, 0.25f, 1f);
                    break;
                case 16:
                    this.targetMultiplier.GetComponent<Text>().text = "x16";
                    this.targetMultiplier.GetComponent<Text>().color = new Color(1f, 0.52f, 0f, 1f);
                    break;
            }
            ////Debug.Log("Target!");
            this.AnimateScore();
            targetScore += targetScoreIncrease;
        }

        if (scaling)
        {
            //increase size of duplicate text
            scoreAnimation.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            Color oldColor = scoreAnimation.GetComponent<Text>().color;
            scoreAnimation.GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a - 0.05f);
            //check if object has become transparent
            if (scoreAnimation.GetComponent<Text>().color.a < 0.2f)
            {
                //if so, destroy it and set flag to false
                Destroy(scoreAnimation);
                scaling = false;
            }
        }

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            //check if player has no father to start the AirTimeTimer
            if (playerObject.transform.parent == null && !gameManager.GetComponent<GameManager>().gameEnded)
            {

                if (!flagForAirTime && airTimeTimer > airTimeThreshold)
                {
                    Text textComponent = x2.GetComponent<Text>();
                    textComponent.fontSize = 18;
                    textComponent.color = new Color(1f, 1f, 1f, 1f);
                    x2.GetComponent<Text>().text = "x2";
                    flagForAirTime = true;
                    scoreCoefficient = 2;
                    x2.GetComponent<xTwoAnimation>().FadeIn();
                }
                //if the player has a higher coefficient than 1, it means we can start checking for the x4 multiplier
                if (scoreCoefficient > 1)
                {
                    Text textComponent = x2.GetComponent<Text>();
                    if (!isX4 && airTimeTimer > airTimeThresholdX4 && airTimeTimer < airTimeThresholdX8)
                    {
                        textComponent.fontSize = 18;
                        isX4 = true;
                        //change the score coefficient
                        scoreCoefficient = 4;

                        //change text of x2 object to x4
                        textComponent.text = "x4";
                        //change color of object
                        textComponent.color = new Color(1f, 0.93f, 0.67f, 1f);
                    }
                    else if (!isX8 && airTimeTimer > airTimeThresholdX8 && airTimeTimer < airTimeThresholdX16)
                    {
                        textComponent.fontSize = 18;
                        isX8 = true;
                        //change the score coefficient
                        scoreCoefficient = 8;

                        //change text of x2 object to x8
                        textComponent.text = "x8";
                        //change color of object
                        textComponent.color = new Color(1f, 0.75f, 0.25f, 1f);
                    }
                    else if (!isX16 && airTimeTimer > airTimeThresholdX16)
                    {
                        textComponent.fontSize = 18;
                        isX16 = true;
                        //change the score coefficient
                        scoreCoefficient = 16;
                        //change text of x2 object to x16
                        textComponent.text = "x16";
                        //change color of object
                        textComponent.color = new Color(1f, 0.52f, 0f, 1f);
                    }
                }

                if (x2.activeInHierarchy)
                {
                    x2.GetComponent<xTwoAnimation>().AnimateScore();
                }
                //keep the timer running
                airTimeTimer += airTimeDelta;
            }
            else if (airTimeTimer != 0f || scoreCoefficient != 1)
            {
                airTimeTimer = 0f;
                scoreCoefficient = 1;
                if (flagForAirTime)
                {

                    isX4 = false;
                    isX8 = false;
                    isX16 = false;
                    x2.GetComponent<xTwoAnimation>().FadeOut();
                    flagForAirTime = false;
                    Text textComponent = x2.GetComponent<Text>();
                    textComponent.fontSize = 18;
                    textComponent.color = new Color(1f, 1f, 1f, 1f);
                    x2.GetComponent<Text>().text = "x2";
                }
            }
        }

    }

    void SetScore(float score)
    { //setting the text ui to a two decimal float, the y.
        if (!gameManager.GetComponent<GameManager>().gameEnded)
        {
            ScoreText.GetComponent<Text>().text = score.ToString("0");
        }
        //checking if the player beat the highscore
        if (score > lastRecordedHighscore && !newRecordText.activeInHierarchy)
        {
            //make the new record text appear
            newRecordText.GetComponent<xTwoAnimation>().FadeIn();
        }
    }

    //animation for pulsating score
    void AnimateScore()
    {
        //checking if its the only instance of scoreAnimation 
        if (scoreAnimation == null)
        {
            //duplicate the score object        
            scoreAnimation = GameObject.Instantiate(ScoreText);
            scoreAnimation.transform.position = ScoreText.transform.position;
            //setting it as the correct child of scoreCanvas
            scoreAnimation.transform.SetParent(ScoreText.transform.parent.transform);
            //increase size and increase transparency
            scaling = true;
        }
    }

   
}
