using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class xTwoAnimation : MonoBehaviour
{
    public GameObject scoreAnimation;
    public bool scaling = false;
    public bool fadeOut = false;
    public bool fadeIn = false;
    Vector3 oldScale;
    Color oldColor;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (scaling && !(fadeIn || fadeOut))
        {
            // if (deb) {
            // Debug.Log("Scaling animation begun");
            // deb = false;
            // }
            //increase size of duplicate text
            scoreAnimation.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);

            Color oldColor = scoreAnimation.GetComponent<Text>().color;
            scoreAnimation.GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a - 0.1f);
            // Debug.Log(oldColor.a);
            //check if object has become transparent
            if (scoreAnimation.GetComponent<Text>().color.a < 0.1f)
            {
                //if so, destroy it and set flag to false
                Destroy(scoreAnimation);
                scaling = false;
            // Debug.Log("Scaling animation finished");
            // deb = true;
            }
        }

        if (fadeOut)
        {

            //increase size of object
            this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            Color color = this.gameObject.GetComponent<Text>().color;
            this.gameObject.GetComponent<Text>().color = new Color(color.r, color.g, color.b, color.a - 0.05f);
            //check if object has become transparent
            if (this.gameObject.GetComponent<Text>().color.a < 0.2f)
            {
                //set object inactive, reset it to normal size and color
                this.gameObject.SetActive(false);
                this.transform.localScale = oldScale;
                this.gameObject.GetComponent<Text>().color = oldColor;
                fadeOut = false;
            }
        }

        if (fadeIn)
        {
            //Debug.Log("Fading in");
            //decrease size of duplicate text
            this.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            Color color = this.gameObject.GetComponent<Text>().color;
            this.gameObject.GetComponent<Text>().color = new Color(color.r, color.g, color.b, color.a + 0.1f);
            //check if object has become visible
            if (this.gameObject.GetComponent<Text>().color.a > 0.8f)
            {
                //Debug.Log("Finished Fading in");

                //set object inactive, reset it to normal size and color
                this.transform.localScale = oldScale;
                this.gameObject.GetComponent<Text>().color = oldColor;
                fadeIn = false;
            }
        }
    }

    public void AnimateScore()
    {
        //check if there is an animation ongoing
        if (this.transform.childCount == 0)
        {
            //duplicate the score object     
            float multiplier = 3;   
            scoreAnimation = GameObject.Instantiate(this.gameObject);
            scoreAnimation.transform.position = this.transform.position;
            scoreAnimation.transform.localScale = new Vector3 (scoreAnimation.transform.localScale.x*multiplier,scoreAnimation.transform.localScale.y*multiplier,scoreAnimation.transform.localScale.z*multiplier);
            //setting it as the correct child of scoreCanvas
            //scoreAnimation.transform.parent = this.gameObject.transform;
            scoreAnimation.transform.SetParent(this.gameObject.transform);
            //increase size and increase transparency
            scaling = true;
        }
    }

    public void FadeOut()
    {
        //getting the old values to reset object
        oldScale = this.transform.localScale;
        oldColor = this.gameObject.GetComponent<Text>().color;
        fadeOut = true;

    }

    public void FadeIn()
    {
        //getting the target scale and color
        oldScale = this.transform.localScale;
        oldColor = this.gameObject.GetComponent<Text>().color;
        //making the object big and transparent to fade it in
        this.transform.localScale = new Vector3(this.transform.localScale.x + 1.6f, this.transform.localScale.y + 1.6f, this.transform.localScale.z + 1.6f);
        this.gameObject.GetComponent<Text>().color = new Color(oldColor.r, oldColor.g, oldColor.b, 0);
        //make it active and start the animation
        this.gameObject.SetActive(true);
        fadeIn = true;
    }
}
