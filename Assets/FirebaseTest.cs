// using UnityEngine;
// using Firebase;
// using Firebase.Extensions;
// public class FirebaseTest : MonoBehaviour
// {
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         Debug.Log("firebase test has BEGUN");

//         FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task=>
//         {
//             Debug.Log("Firebase dependency status: " + task.Result);

//             if(task.Result == DependencyStatus.Available)
//             {
//                 Debug.Log("Firebase is working good");
//             } else
//             {
//                 Debug.Log("it ain't working properly" + task.Result);
//             }
//         });
//     }
// }
