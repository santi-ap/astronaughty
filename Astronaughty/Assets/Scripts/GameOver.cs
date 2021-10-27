using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public Animator playerAnimator;
    // Update is called once per frame
    public AudioSource sizzle;
    public bool isDed = false;
    public GameManager gameManager;
    void Update()
    {
        //Debug.log("Game Over 14");
        if (!GetComponent<Renderer>().IsVisibleFrom(Camera.main) && !isDed){
            //Debug.log("Out of view");
            isDed = true;
            gameManager.GameOver(); //When player leaves the camera view, it will call the game over event 
            //Debug.log("Game over 18");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            //Debug.log("Hit an asteroid");
            isDed = true;
            sizzle.Play();
            playerAnimator.SetBool("isDead", true);//starts the burning animation
            gameManager.GameOver();
        }
    }
    
}
