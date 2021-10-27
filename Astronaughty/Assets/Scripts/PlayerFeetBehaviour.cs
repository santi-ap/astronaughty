using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeetBehaviour : MonoBehaviour
{

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        ////Debug.Log("Somebody once told me...");

        if (other.gameObject.tag.Equals("Asteroid"))
        {
            ////Debug.Log("Somebody once told me...");
        }
    }

}
