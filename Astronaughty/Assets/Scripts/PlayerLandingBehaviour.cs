using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * This script takes care of the behaviour of the player during landing on an asteroid.
 */
public class PlayerLandingBehaviour : MonoBehaviour
{
    
    public GameObject myGameObject;
    public Transform asteroidTransform;

    public xTwoAnimation x2;

    //this is being called when the player touches something
    private void OnCollisionEnter2D(Collision2D other)
    {
        ////Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag.Equals("Asteroid"))
        {
            //when the player collides with the planet, it turns isLanded true
            other.gameObject.GetComponent<GravityBehaviour>().isLanded = true;
            ////Debug.Log("Touched the asteroid for the first time");
            //setting the force to 0
            myGameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            
            //making the gameObject kinematic, so that it doesnt bounce around
            myGameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            // Debug.Log("Player Landing 29");
            //these three lines take care of the asteroid, calling him setting it kinematic and not simulated (if simulated the asteroids get pushed by the player)
            asteroidTransform = other.gameObject.transform;
            //setting the player as the child of the asteroid, so that it rotates
            myGameObject.transform.parent = asteroidTransform;
            x2.fadeOut = false;
            x2.fadeIn = false;
            x2.scaling = false;
            Destroy(x2.scoreAnimation);
        }
    }
}
