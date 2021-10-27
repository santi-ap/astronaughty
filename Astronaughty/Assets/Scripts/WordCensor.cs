using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordCensor : MonoBehaviour
{

    string[] blackList = { "fuck", "shit" };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool isWordProfanity(string word)
    {
        foreach (string i in blackList)
        {
            if (word.Contains(i))
            {
                Debug.Log("No no word");
                return true;
            }
        }
        Debug.Log("Good word");
        return false;
    }
}
