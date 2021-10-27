using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSettingsBehaviour : MonoBehaviour
{
    public GameObject activateSound;
    public GameObject deactivateSound;

    public List<GameObject> listOfSounds;


    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("SoundSettings"))
        {
            PlayerPrefs.SetInt("SoundSettings", 1);
        }
        int soundSettings = PlayerPrefs.GetInt("SoundSettings");
        if (deactivateSound != null)
        {
            deactivateSound.SetActive(soundSettings == 1);
            activateSound.SetActive(soundSettings == 0);
        }
        if (soundSettings == 0)
        {
            foreach (GameObject g in listOfSounds)
            {
                if (g.TryGetComponent( out AudioManager audio)) {
                g.GetComponent<AudioManager>().fadingIn = false;
                }
                g.GetComponent<AudioSource>().volume = 0;
            }
        }

    }

    public void DeactivateSound()
    {
        PlayerPrefs.SetInt("SoundSettings", 0);
        if (deactivateSound != null)
        {
            deactivateSound.SetActive(false);
            activateSound.SetActive(true);
        }

        foreach (GameObject g in listOfSounds)
        {
            g.GetComponent<AudioManager>().fadingIn = false;
            g.GetComponent<AudioSource>().volume = 0;
        }
    }

    public void ActivateSound()
    {
        if (deactivateSound != null)
        {
            deactivateSound.SetActive(true);
            activateSound.SetActive(false);
        }
        PlayerPrefs.SetInt("SoundSettings", 1);

        foreach (GameObject g in listOfSounds)
        {
            g.GetComponent<AudioManager>().FadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
