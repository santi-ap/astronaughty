using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    static GameObject instance = null;
    public string myScene;

    public bool fadingIn = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            this.FadeIn();
            instance = this.gameObject;
            GameObject.DontDestroyOnLoad(gameObject);
        }

    }


    public void ChangingScene(string newScene)
    {
        if (!newScene.Equals(myScene))
        {
            Destroy(gameObject);
        }
    }

    public void FadeIn() {
        fadingIn = true;
    }

    void FixedUpdate() {
        if (fadingIn) {
            this.gameObject.GetComponent<AudioSource>().volume += 0.001f; 
        }
    }
}