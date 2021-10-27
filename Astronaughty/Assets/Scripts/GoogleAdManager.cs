// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GoogleMobileAds.Api;

// public class GoogleAdManager : MonoBehaviour
// {
//     public static GoogleAdManager instance;

//     private void Awake()
//     {
//         if(instance == null)
//         {
//             instance = this;
//             DontDestroyOnLoad(gameObject);
//             MobileAds.Initialize((initStatus) =>
//             {
//                 // SDK initialization is complete
//             });
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
//     // Start is called before the first frame update
//     void Start()
//     {
//         // // Initialize the Mobile Ads SDK.
//         // MobileAds.Initialize((initStatus) =>
//         // {
//         //     // SDK initialization is complete
//         // });
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }
