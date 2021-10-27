using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    public GameObject startingSegment;
    public GameObject player;

    public GameObject[] easySegments;
    public GameObject[] mediumSegments;
    public GameObject[] hardSegments;

    public float distanceToGenerate = 20f;
    public int startingSegments = 3;
    public int segmentsGenerated = 3;

    // int randomNumber;
    GameObject[] difficulty;
    GameObject current;
    float currentY;
    GameObject currentEndSegment;

    float playerProgress = 0f;

    System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        easySegments = Resources.LoadAll<GameObject>("EasySegments");
        mediumSegments = Resources.LoadAll<GameObject>("MediumSegments");
        hardSegments = Resources.LoadAll<GameObject>("HardSegments");

        Instantiate(startingSegment, new Vector3(0, 0, 0), Quaternion.identity);

        //The last segment that was generated is assigned to these variables
        current = startingSegment; //Segment
        currentY = current.transform.GetChild(0).transform.position.y; //Segment Y position

        instantiateSegments(startingSegments);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(player.transform.position.y);

        if (player.transform.position.y - playerProgress >= distanceToGenerate)
        {
            playerProgress = player.transform.position.y;
            instantiateSegments(segmentsGenerated);
        }

    }

    public void instantiateSegments(int segments)
    {

        for (int i = 0; i < segments; i++)
        {

            int randomDifficulty = random.Next(1, 101); //Gets a reandom number between 1-100 to choose the difficuly of the segment

            if (randomDifficulty <= segmentDifficultyProb("easy"))
            {
                difficulty = easySegments;
            }
            else if (randomDifficulty <= segmentDifficultyProb("medium"))
            {
                difficulty = mediumSegments;
            }
            else
            {
                difficulty = hardSegments;
            }


            int randomSegment = random.Next(1, difficulty.Length);
            // Debug.Log(difficulty.Length);
            Instantiate(difficulty[randomSegment], new Vector3(0, currentY, 0), Quaternion.identity); //Instantiates the segment to the postion of the endSegment of the current segment
            current = difficulty[randomSegment]; // Makes the segment that was just create, the current segment
            currentY = currentY + current.transform.GetChild(0).transform.position.y; // sets the position for the end of the last segment that was added
        }
    }

    public int segmentDifficultyProb(string dif) // Return the probablity of getting the diffeculty of a segment based on the player Y position
    {

        // switch (dif)
        // {
        //     case "easy":   
        if (dif == "easy")
        {

            if (player.transform.position.y <= 50) //before 50 there is a 100% chance of gettign an easy segment
            {
                return 100;

            }
            else if (player.transform.position.y <= 100) //before 100 there is a 60% chance
            {
                return 60;
            }
            else if (player.transform.position.y <= 150) //before 150 there is a 40% chance
            {
                return 40;
            }
            else if (player.transform.position.y <= 200) //before 200 there is a 20%
            {
                return 20;
            }
            else if (player.transform.position.y <= 300) //before 300 there is a 15%
            {
                return 15;
            }
            else // after 300 there is a 10%
            { return 10; }
        }
        else if (dif == "medium")
        {
            if (player.transform.position.y <= 100) //before 100 there is a 38% chance of gettign a medium segment
            {
                return 98; //38%

            }
            else if (player.transform.position.y <= 150) //before 150 there is a 50%
            {
                return 90; //50%
            }
            else if (player.transform.position.y <= 200) //before 200 there is a 60%
            {
                return 80; //60%
            }
            else if (player.transform.position.y <= 300) //before 300 there is a 45%
            {
                return 60; //45%
            }
            else //after 300 there is a 40%
            { return 50; } //40%

        }
        else
        {
            return 0;
        }

    }
}
