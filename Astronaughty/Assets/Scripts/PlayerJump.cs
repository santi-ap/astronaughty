using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerJump : MonoBehaviour
{
    public Rigidbody2D playerRigidBody; //The rigidBody2D component of the player
    public Vector2 forceDirection; // The x & y components of the force that will make the player jump
    public GameObject myGameObject;
    public GameManager gameManager;


    public bool isTouchScreen = false;

    public float jumpX = 10f; // x component of the force that will make the player jump
    public float jumpY = 10f; // y component of the force that will make the player jump
    public float div = 1f;
    // Start is called before the first frame update

    Vector3 worldPosition;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isTouchScreen && Input.touchCount > 0) // Input.touchCount checks if the screen is bieng touched
        {
            Touch touch = Input.GetTouch(0);//instances a touch input
            int touchId = touch.fingerId;
           //Debug.Log("Player Jump 35");

            if (touch.phase == TouchPhase.Began)
            {
               //Debug.Log("Player Jump 39");
                if (EventSystem.current.IsPointerOverGameObject(touchId))
                {
                   //Debug.Log("Player Jump 42");
                    return;
                }
                else
                {
                    Jump();
                   //Debug.Log("Player Jump 48");
                }
            }
            // case TouchPhase.Ended:
            //     //the finger has been lifted from the screen
            //     Jump();

        }
        else
        {
            ////Debug.Log("Player Jump 58");
            if (Input.GetMouseButtonDown(0))
            { // when the left button is clicked
              // Touch touch = Input.GetTouch(0);//instances a touch input
              // int touchId = touch.fingerId;
             //Debug.Log("Player Jump 63");
                if (EventSystem.current.IsPointerOverGameObject())
                {
                   //Debug.Log("Player Jump 66");
                    return;
                }
                else
                {
                    Jump();
                   //Debug.Log("Player Jump 72");
                }
            }
        }
    }

    public void Jump()
    {
       //Debug.Log("Player Jump 80");
        if (myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform != null)
        {

            //////Debug.Log("Player, Planet, Direction");
            Vector3 playerPosition = myGameObject.transform.position;
            //////Debug.Log(myGameObject.transform.position);
           //Debug.Log("Player Jump 86");

            Vector3 planetPosition = myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform.position;
            //////Debug.Log(myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform.position);
           //Debug.Log("Player Jump 90");

            Vector3 direction = playerPosition - planetPosition;
            //////Debug.Log(playerPosition - planetPosition);
           //Debug.Log("Player Jump 94");

            myGameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
           //Debug.Log("Player Jump 97");
            gameManager.lastPlanet = myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform.gameObject;//sets the last planet the player was on to the asteroid game object 
            gameManager.lastPlayerPosition = playerPosition;//sets the player's last position before he jumps
            FindObjectOfType<GameOver>().isDed = false;//let player die when off screen
            //////Debug.Log("Jumping");
            forceDirection = new Vector2((direction.x / myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform.localScale.x) * div, (direction.y / myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform.localScale.y) * div);
            //////Debug.Log("ForceDirectio: " + forceDirection);
           //Debug.DrawRay(playerPosition, playerPosition - planetPosition, Color.white, 10);
            playerRigidBody.AddForce(forceDirection, ForceMode2D.Impulse);
            //if there is an asteroid, no more
            if (myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform != null)
            {
                myGameObject.GetComponent<PlayerLandingBehaviour>().asteroidTransform = null;
            }
            //telling the landing script we're not doing the landing thing anymore
            myGameObject.transform.parent = null;
        }

    }

    void OnTriggerExit2D(Collider2D other){
        //when the player exits the the gravity field, it turns isLanded false
        other.gameObject.GetComponent<GravityBehaviour>().isLanded = false;
    }
}
