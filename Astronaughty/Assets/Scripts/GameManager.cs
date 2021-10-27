using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
// using GoogleMobileAds.Api;
// using GoogleMobileAds.Placement;

public class GameManager : MonoBehaviour
{
    public GameObject highscoreManager;

    public bool gameEnded = false;
    public float restartDelay = 1f;
    public GameObject gameOverCanvas;
    public GameObject scoreCanvas;
    public GameObject tutorialCanvas;
    public GameObject player;
    public GameObject playButton;
    public GameObject x2;
    public AudioManager soundTrack;
    public GameObject nameInput;
    public GameObject rewardedAdCanvas;//the UI canvas where player can choose to watch a rewarded video ad
    public GameObject lastPlanet; //last planet player was on before dying
    public Vector3 lastPlayerPosition; //the players last position before jumping
    public Animator playerAnimator;
    public CinemachineVirtualCamera virtualCamera;
    public bool rewardVideoEnabled = true;//if true, player has the option to watch a rewarded video ad
    public bool startOver = false;//if true, player chose to not watch rewarded video ad and start the game over
    // public BannerAdGameObject googleBannerAd;
    public GameObject creditCanvas;
    public GameObject menuCanvas;
    bool canvasActive = false;
    // public bool isGoogleAds = false;
    // public GameObject languageCanvas;

    public GameObject invalidName;

    string[] blackList = { "4r5e", "5h1t", "5hit", "a55", "anal", "anus", "ar5e", "arrse", "arse", "ass", "ass-fucker", "asses", "assfucker", "assfukka", "asshole", "assholes", "asswhole", "a_s_s", "b!tch", "b00bs", "b17ch", "b1tch", "ballbag", "balls", "ballsack", "bastard", "beastial", "beastiality", "bellend", "bestial", "bestiality", "bi+ch", "biatch", "bitch", "bitcher", "bitchers", "bitches", "bitchin", "bitching", "bloody", "blow job", "blowjob", "blowjobs", "boiolas", "bollock", "bollok", "boner", "boob", "boobs", "booobs", "boooobs", "booooobs", "booooooobs", "breasts", "buceta", "bugger", "bum", "bunny fucker", "butt", "butthole", "buttmuch", "buttplug", "c0ck", "c0cksucker", "carpet muncher", "cawk", "chink", "cipa", "cl1t", "clit", "clitoris", "clits", "cnut", "cock", "cock-sucker", "cockface", "cockhead", "cockmunch", "cockmuncher", "cocks", "cocksuck", "cocksucked", "cocksucker", "cocksucking", "cocksucks", "cocksuka", "cocksukka", "cok", "cokmuncher", "coksucka", "coon", "cox", "crap", "cum", "cummer", "cumming", "cums", "cumshot", "cunilingus", "cunillingus", "cunnilingus", "cunt", "cuntlick", "cuntlicker", "cuntlicking", "cunts", "cyalis", "cyberfuc", "cyberfuck", "cyberfucked", "cyberfucker", "cyberfuckers", "cyberfucking", "d1ck", "damn", "dick", "dickhead", "dildo", "dildos", "dink", "dinks", "dirsa", "dlck", "dog-fucker", "doggin", "dogging", "donkeyribber", "doosh", "duche", "dyke", "ejaculate", "ejaculated", "ejaculates", "ejaculating", "ejaculatings", "ejaculation", "ejakulate", "f u c k", "f u c k e r", "f4nny", "fag", "fagging", "faggitt", "faggot", "faggs", "fagot", "fagots", "fags", "fanny", "fannyflaps", "fannyfucker", "fanyy", "fatass", "fcuk", "fcuker", "fcuking", "feck", "fecker", "felching", "fellate", "fellatio", "fingerfuck", "fingerfucked", "fingerfucker", "fingerfuckers", "fingerfucking", "fingerfucks", "fistfuck", "fistfucked", "fistfucker", "fistfuckers", "fistfucking", "fistfuckings", "fistfucks", "flange", "fook", "fooker", "fuck", "fucka", "fucked", "fucker", "fuckers", "fuckhead", "fuckheads", "fuckin", "fucking", "fuckings", "fuckingshitmotherfucker", "fuckme", "fucks", "fuckwhit", "fuckwit", "fudge packer", "fudgepacker", "fuk", "fuker", "fukker", "fukkin", "fuks", "fukwhit", "fukwit", "fux", "fux0r", "f_u_c_k", "gangbang", "gangbanged", "gangbangs", "gaylord", "gaysex", "goatse", "God", "god-dam", "god-damned", "goddamn", "goddamned", "hardcoresex", "hell", "heshe", "hoar", "hoare", "hoer", "homo", "hore", "horniest", "horny", "hotsex", "jack-off", "jackoff", "jap", "jerk-off", "jism", "jiz", "jizm", "jizz", "kawk", "knob", "knobead", "knobed", "knobend", "knobhead", "knobjocky", "knobjokey", "kock", "kondum", "kondums", "kum", "kummer", "kumming", "kums", "kunilingus", "l3i+ch", "l3itch", "labia", "lust", "lusting", "m0f0", "m0fo", "m45terbate", "ma5terb8", "ma5terbate", "masochist", "master-bate", "masterb8", "masterbat*", "masterbat3", "masterbate", "masterbation", "masterbations", "masturbate", "mo-fo", "mof0", "mofo", "mothafuck", "mothafucka", "mothafuckas", "mothafuckaz", "mothafucked", "mothafucker", "mothafuckers", "mothafuckin", "mothafucking", "mothafuckings", "mothafucks", "mother fucker", "motherfuck", "motherfucked", "motherfucker", "motherfuckers", "motherfuckin", "motherfucking", "motherfuckings", "motherfuckka", "motherfucks", "muff", "mutha", "muthafecker", "muthafuckker", "muther", "mutherfucker", "n1gga", "n1gger", "nazi", "nigg3r", "nigg4h", "nigga", "niggah", "niggas", "niggaz", "nigger", "niggers", "nob", "nob jokey", "nobhead", "nobjocky", "nobjokey", "numbnuts", "nutsack", "orgasim", "orgasims", "orgasm", "orgasms", "p0rn", "pawn", "pecker", "penis", "penisfucker", "phonesex", "phuck", "phuk", "phuked", "phuking", "phukked", "phukking", "phuks", "phuq", "pigfucker", "pimpis", "piss", "pissed", "pisser", "pissers", "pisses", "pissflaps", "pissin", "pissing", "pissoff", "poop", "porn", "porno", "pornography", "pornos", "prick", "pricks", "pron", "pube", "pusse", "pussi", "pussies", "pussy", "pussys", "rectum", "retard", "rimjaw", "rimming", "s hit", "s.o.b.", "sadist", "schlong", "screwing", "scroat", "scrote", "scrotum", "semen", "sex", "sh!+", "sh!t", "sh1t", "shag", "shagger", "shaggin", "shagging", "shemale", "shi+", "shit", "shitdick", "shite", "shited", "shitey", "shitfuck", "shitfull", "shithead", "shiting", "shitings", "shits", "shitted", "shitter", "shitters", "shitting", "shittings", "shitty", "skank", "slut", "sluts", "smegma", "smut", "snatch", "son-of-a-bitch", "spac", "spunk", "s_h_i_t", "t1tt1e5", "t1tties", "teets", "teez", "testical", "testicle", "tit", "titfuck", "tits", "titt", "tittie5", "tittiefucker", "titties", "tittyfuck", "tittywank", "titwank", "tosser", "turd", "tw4t", "twat", "twathead", "twatty", "twunt", "twunter", "v14gra", "v1gra", "vagina", "viagra", "vulva", "w00se", "wang", "wank", "wanker", "wanky", "whoar", "whore", "willies", "willy", "xrated", "xxx", "aborto",
                            "anal", "ano", "culo", "follador de culo", "culos", "estúpido", "bolsa de pelota", "bolas", "bastardo", "campana", "bestial", "bestialidad", "perra", "perras", "quejas", "sangriento", "mamada", "bollok", "teta", "tetas", "los pechos", "buceta", "extremo", "muncher alfombra", "grieta", "cipa", "clítoris", "polla", "chupar la polla", "gallos", "mapache", "mierda", "semen", "corrida", "cunillingus", "coño", "maldita sea", "consolador", "consoladores", "tonto", "perro follador", "duche", "dique", "eyacular", "eyaculado", "eyacula", "eyaculación", "maricón", "fagging", "maricones", "felching", "felación", "brida", "follada", "cabron", "folladores", "maldito", "carajo", "folla", "fudge packer", "maldito sea", "infierno", "hore", "córneo", "tirón", "kock", "labios vaginales", "lujuria", "masoquista", "masturbarse", "madre folladora", "nazi", "negro", "niggers", "orgasimo", "orgasmo", "orgasmos", "pájaro carpintero", "pene", "mear", "molesto", "pisser", "orinando", "enojado", "pornografía", "porno", "pinchazo", "pinchazos", "pube", "coños", "violación", "violador", "recto", "retardar", "rimming", "sádico", "atornillar", "escroto", "sexo", "pelusa", "follar", "transexual", "cagadas", "cagado", "cagando", "de mierda", "skank", "puta", "putas", "smegma", "tizón", "arrebatar", "espacio", "agallas", "testículo", "zurullo", "vagina", "viagra", "vulva", "wang", "paja",
                            "aborto","anale", "ano", "culo", "ass-stronzo", "asini", "stronzo", "stronzi", "ballbag", "palle", "bastardo", "bellend", "bestiale", "brutalità", "cagna", "bitches", "bitching", "sanguinoso", "pompino", "bollok", "tetta", "tette", "seni", "buceta", "muncher di tappeti", "spiraglio", "cipa", "clitoride", "cazzo", "pompinara", "cazzi", "procione lavatore", "una schifezza", "cum", "eiaculazione", "cunillingus", "fica", "dannazione", "dildo", "dink", "dog-stronzo", "duche", "diga", "eiaculare", "eiaculato", "eiacula", "sigaretta", "fagging", "fascina", "fascine", "figa", "felching", "fellatio", "flangia", "fanculo", "scopata", "coglione", "fuckers", "fuckings", "scopa", "fudge packer", "god-dannato", "inferno", "hore", "corneo", "kock", "labbra", "lussuria", "lusting", "masochista", "masturbarsi", "madre stronza", "nazista", "negro", "negri", "orgasim", "orgasmo", "orgasmi", "pene", "pisciare", "incazzata", "pisser", "piscia", "pissing", "pissoff", "cacca", "porno", "pornografia", "puntura", "pube", "fighe", "micio", "stupro", "stupratore", "retto", "ritardare", "rimming", "sadico", "avvitamento", "scroto", "sperma", "sesso", "scopare", "shagging", "transessuali", "merda", "shite", "merde", "shitted", "cacare", "merdoso", "skank", "slut", "troie", "smegma", "oscenità", "strappare", "figlio di puttana", "spac", "audacia", "testicolo", "titt", "escremento", "vagina", "viagra", "vulva", "wang", "wank", "puttana", "x valutato",};

    float score;


    public void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            IsWordProfanity();
        }
    }

    //Checks if tutorial needs to be shown
    void Start()
    {
        // // Initialize the Mobile Ads SDK.
        // MobileAds.Initialize((initStatus) =>
        // {
        //     // SDK initialization is complete
        // });

        // if (isGoogleAds)
        // {
        //     googleBannerAd = MobileAds.Instance
        //     .GetAd<BannerAdGameObject>("GoogleBannerAd");

        //     googleBannerAd.LoadAd();
        //     googleBannerAd.Hide();
        // }
        // else
        // {
            AdManager.HideBanner(); // hides the ad banner
        // }


        //Debug.log("This is the start method");

        this.gameObject.GetComponent<HighScore>().DownloadHighScores();
        //PlayerPrefs.DeleteAll();
        //checking if the username has been set
        if (!PlayerPrefs.HasKey("UserName"))
        {
            PlayerPrefs.SetString("UserName", "guest" + Random.Range(1, 100000));
        }
        if (nameInput != null)
        {
            //Debug.Log(PlayerPrefs.GetString("UserName"));
            if (!PlayerPrefs.GetString("UserName").Contains("guest"))
            {
                nameInput.transform.parent.GetComponent<InputField>().text = PlayerPrefs.GetString("UserName");
            }
        }
        //looking for the goddamn audio
        if (GameObject.Find("Audio Source") != null)
        {
            soundTrack = GameObject.Find("Audio Source").GetComponent<AudioManager>();
        }

        // Debug.Log("Deleting key");
        // PlayerPrefs.DeleteKey("TutorialKey");
        //checking for the tutorial preferences
        if (!(PlayerPrefs.HasKey("TutorialKey")) || PlayerPrefs.GetInt("TutorialKey") == 0)
        {
            //Debug.log("This is my tutorial key " + PlayerPrefs.GetInt("TutorialKey"));
            PlayerPrefs.SetInt("TutorialKey", 0);
            tutorialCanvas.SetActive(true);
        }

    }

    public void GameOver()
    {
        //Debug.log("This is the GameOver method in Game Manager");
        //Debug.log("Game manager82");
        // player = GameObject.Find("Player");
        //Debug.log("Player: "+player.name);
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        //killing the x2
        if (x2.activeInHierarchy)
        {
            //Debug.log("Game manager 87");

            Destroy(x2.GetComponent<xTwoAnimation>().scoreAnimation);

            x2.GetComponent<xTwoAnimation>().FadeOut();

            x2.GetComponent<xTwoAnimation>().scaling = false;

        }


        if (rewardVideoEnabled && !startOver)
        {
            //Debug.log("Game manager 93");
            Invoke("RewardedAdCanvas", restartDelay);
        }
        else
        {
            //Debug.log("Game manager 98");
            if (gameEnded == false)
            {
                //Debug.log("Game manager 101");
                bool isHighScore = false;
                gameEnded = true;

                rewardVideoEnabled = true; //let's player watch a rewarded video since player has already died


                //HIGHSCORE
                //if the highscore key does not exist, we set it to the current gameScore.
                //PlayerPrefs.DeleteKey("HighScore"); //delete your highscore!
                if (!PlayerPrefs.HasKey("HighScore"))
                {
                    //Debug.log("Game manager 113");
                    PlayerPrefs.SetFloat("HighScore", scoreCanvas.GetComponent<Score>().score);
                }
                else if (PlayerPrefs.GetFloat("HighScore") < scoreCanvas.GetComponent<Score>().score)
                {
                    //Debug.log("Game manager 118");
                    PlayerPrefs.SetFloat("HighScore", scoreCanvas.GetComponent<Score>().score);
                    isHighScore = true;
                }
                //rendering current score
                //Debug.log("Game manager 123");

                gameOverCanvas.transform.GetChild(1).gameObject.GetComponent<Text>().text += scoreCanvas.GetComponent<Score>().score.ToString("0");
                score = scoreCanvas.GetComponent<Score>().score;
                //rendering highscore
                gameOverCanvas.transform.GetChild(2).gameObject.GetComponent<Text>().text += PlayerPrefs.GetFloat("HighScore").ToString("0");
                //render highscore notification
                gameOverCanvas.transform.GetChild(3).gameObject.SetActive(isHighScore);
                //render distance traveled
                gameOverCanvas.transform.GetChild(4).gameObject.GetComponent<Text>().text += scoreCanvas.GetComponent<Score>().currentYScore.ToString("0");
                //render airtime bonus
                gameOverCanvas.transform.GetChild(5).gameObject.GetComponent<Text>().text += scoreCanvas.GetComponent<Score>().airTimeTotalScore.ToString("0");
                //Debug.log("Game manager 135");

                //Finds the player object
                GameObject varGameObject = GameObject.Find("Player");
                // //Disables player jump 
                varGameObject.GetComponent<PlayerJump>().enabled = false;
                //Debug.log("Game manager 141");
                player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                this.gameObject.GetComponent<HighScore>().AddNewHighScore(PlayerPrefs.GetString("UserName"), Mathf.RoundToInt(score));

                Invoke("GameOverCanvas", restartDelay); // Calls game over menu with a delay
                //Debug.log("Game manager ended process");
            }
        }

    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restarts the scene
        Time.timeScale = 1;
    }

    public void GameOverCanvas()
    {

        //Debug.log("Game manager 160");
        // if (isGoogleAds)
        // {
        //     googleBannerAd.Show();
        // }
        // else
        // {
            AdManager.ShowBanner();//shows the banner add on the game over menu
        // }

        // Makes game over menu visible and hides the scoreCanvas
        gameOverCanvas.SetActive(true);
        scoreCanvas.GetComponent<Score>().ScoreText.GetComponent<Text>().text = score.ToString("0");
        this.gameObject.GetComponent<HighScore>().DownloadHighScores();
    }

    public void RewardedAdCanvas()
    {
        Destroy(x2.GetComponent<xTwoAnimation>().scoreAnimation);
        x2.GetComponent<xTwoAnimation>().scaling = false;
        x2.GetComponent<xTwoAnimation>().fadeIn = false;
        x2.GetComponent<xTwoAnimation>().fadeOut = false;

        // if (isGoogleAds)
        // {
        //     googleBannerAd.Show();
        // }
        // else
        // {
            AdManager.ShowBanner();//shows the banner add on the rewarded ad canvas
        //}

        // Makes game over menu visible and hides the scoreCanvas
        rewardedAdCanvas.SetActive(true);
        // scoreCanvas.GetComponent<Score>().ScoreText.GetComponent<Text>().text = score.ToString("0");
        // FindObjectOfType<HighScore>().DownloadHighScores();
    }



    public void returnToMenu()
    { //Returns to main menu.
        Debug.Log("Game Manager 184");
        // if (isGoogleAds)
        // {
        //     googleBannerAd.Hide();
        // }
        // else
        // {
            AdManager.HideBanner(); // hides the ad banner
        // }

        soundTrack.ChangingScene("MainMenu");
        SceneManager.LoadScene(0);
        Debug.Log("Game Manager 188");
    }

    public void play()
    {
        // Debug.Log("Game Manager 193");
        bool badWord = IsWordProfanity();
        Debug.Log("Dad word is: :" + badWord);

        if (badWord == false)
        {
            PlayerPrefs.SetString("UserName", nameInput.GetComponent<Text>().text.Equals("") && !PlayerPrefs.GetString("UserName").Equals("") ? "guest" + Random.Range(1, 100000) : nameInput.GetComponent<Text>().text);
            soundTrack.ChangingScene("Prototype Level 1");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        // Debug.Log("Game Manager 197");

    }

    public void exitGame()
    {
        Application.Quit();
        ////Debug.Log("Quit");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        GameObject varGameObject = GameObject.Find("Player");
        //Disables player jump while paused
        varGameObject.GetComponent<PlayerJump>().enabled = false;
        playButton.SetActive(true);

    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        GameObject varGameObject = GameObject.Find("Player");
        //Enables player jump when unpaused
        varGameObject.GetComponent<PlayerJump>().enabled = true;
        playButton.SetActive(false);
    }

    public void RewardedVideoAction()
    {
        // if (isGoogleAds)
        // {
        //     googleBannerAd.Hide();
        // }
        // else
        // {
            AdManager.HideBanner(); // hides banner ad
        // }

        AdManager.ShowRewardedAd(SecondChance, Skip, Failed);
    }

    //this method will be called to show a rewarded video and give the player a second a chance
    public void SecondChance()
    {
        virtualCamera.ForceCameraPosition(new Vector3(virtualCamera.transform.position.x, lastPlayerPosition.y, virtualCamera.transform.position.z), virtualCamera.transform.rotation);
        //Debug.log("Show rewarded video");
        rewardVideoEnabled = false; //doesn't allow player to watch more than one rewarded video per session (must die once before being able to watch another rewarded video)
                                    //continue game from last planet
                                    // virtualCamera.transform.position = new Vector3(virtualCamera.transform.position.x,lastPlayerPosition.y,virtualCamera.transform.position.z);

        player.transform.position = lastPlayerPosition;//brings the player back to the last position before it died
        player.transform.parent = lastPlanet.transform;//makes the player a child of the last planet it was on before it died
        player.GetComponent<PlayerLandingBehaviour>().asteroidTransform = lastPlanet.transform;
        rewardedAdCanvas.SetActive(false);
        // FindObjectOfType<GameOver>().isDed = false;
        playerAnimator.SetBool("isDead", false);//stops the burning animation
    }
    void Skip()
    {
        //Debug.log("Ad skipped");
    }

    void Failed()
    {
        //Debug.log("Ad failed to load");
    }

    public void StartOver()
    {
        rewardVideoEnabled = true;
        //Debug.log("Start Over");
        startOver = true;
        rewardedAdCanvas.SetActive(false);
        GameOver();
    }

    public void CreditsCanvas()
    {
        Debug.Log("Game Manager 267");
        menuCanvas.SetActive(canvasActive);
        canvasActive = !canvasActive;
        creditCanvas.SetActive(canvasActive);
    }

    public bool IsWordProfanity()
    {
        foreach (string i in blackList)
        {
            if (nameInput.transform.parent.GetComponent<InputField>().text.ToLower().Contains(i))
            {

                invalidName.SetActive(true);
                return true;
            }
        }
        invalidName.SetActive(false);
        return false;
        // play();
    }

    //     public IEnumerator setLanguage(int i)
    //     {
    //         // Wait for the localization system to initialize, loading Locales, preloading, etc.
    //         yield return LocalizationSettings.InitializationOperation;

    //         // This variable selects the language. For example, if in the table your first language is English then 0 = English. If the second language in the table is Russian then 1 = Russian etc.


    // // This part changes the language
    //         LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
    //     }

    // public void enableLanguageCanvas(bool languageActive, bool menuActive)
    // {
    //     Debug.Log("Changing canvas");
    //     languageCanvas.SetActive(languageActive);
    //     menuCanvas.SetActive(menuActive);

    // }

    // public void setEnglish()
    // {
    //     // setLanguage(2);
    //     LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
    //     enableLanguageCanvas(false, true);
    // }
    // public void setItalian()
    // {
    //     // setLanguage(2);
    //     Debug.Log("Changing to italian");
    //     LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    //     enableLanguageCanvas(false, true);
    // }
    // public void setSpanish()
    // {
    //     // setLanguage(2);
    //     LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[2];
    //     enableLanguageCanvas(false, true);
    // }

    // public void languageCanvasEnabled()
    // {
    //     enableLanguageCanvas(true, false);
    // }

    // public void returnFromLanguage()
    // {
    //     languageCanvas.SetActive(false);
    //     menuCanvas.SetActive(true);
    // }
}
