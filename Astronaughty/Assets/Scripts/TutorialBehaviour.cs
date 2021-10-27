using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBehaviour : MonoBehaviour
{
    
    

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
            this.gameObject.SetActive(false);
            PlayerPrefs.SetInt("TutorialKey", 1);
        }
    }
}
