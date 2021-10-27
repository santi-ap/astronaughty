using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GravityBehaviour : MonoBehaviour
{
    public Vector2 forceDirection; //The x & y components of the gravity force that will be applied to the player
    public GameObject myGameObject; //the planet's game object
    public float div = 40f; //The amount to devide the forceDirection by to make it less powerful
    public GameObject playerGameObject; //The game object fo the player
    public bool isLanded = true; //Used to determine if the player is on a planet or not

    //when the player enters and stays withing the planet's gravity field (triggerCollider)
    void OnTriggerStay2D(Collider2D other)
    {
        //if it's the player that entered the gravity field and it's not already on a planet
        if (other.gameObject.tag == "Player" && !isLanded)
        {
            Vector3 playerPosition = other.gameObject.transform.position; //the position of the player
            Vector3 planetPosition = myGameObject.transform.position; // the position fo the planet
            Vector3 directionToPlanetCenter = planetPosition - playerPosition; //the direction from the player to the center of the planet

            playerGameObject = other.gameObject; //links the variable to the player's game object

            Debug.DrawRay(playerPosition, directionToPlanetCenter, Color.white, 10);
            forceDirection = directionToPlanetCenter;//makes the magnitud of the force of gravity equal to the magnitud of the direction
            Gravitate();//calls the method that applies the gravity force
        }
    }

    void Gravitate()
    {
        //rounds the force direction to two decimal points
        forceDirection.x = (float)Math.Round(forceDirection.x, 2);
        forceDirection.y = (float)Math.Round(forceDirection.y, 2);

        //if the x component of the force direction is between -0.1 and 0.1
        if (Math.Abs(forceDirection.x) < 0.1f)
        {
            forceDirection.x = 0f; // make it equal 0
        }else{// if not, then make it equal the inverse
            forceDirection.x = 1 / forceDirection.x;
        }

        //if the x component of the force direction is between -0.1 and 0.1
        if (Math.Abs(forceDirection.y) < 0.1f)
        {
            forceDirection.y = 0f;// make it equal 0
        }else{
            forceDirection.y = 1 / forceDirection.y;// if not, then make it equal the inverse
        }

        //apply the gravity force to the player
        playerGameObject.GetComponent<Rigidbody2D>().AddForce(forceDirection / div, ForceMode2D.Impulse);
    }


}
