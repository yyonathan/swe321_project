// using UnityEngine;
// using Firebase.Database;
// using System.Collections.Generic;

// public class LeaderboardTest : MonoBehaviour
// {
//     void Start()
//     {
//         SaveScore("Yonathan", Random.Range(50,200));
//     }

//     void SaveScore(string username, int score)
//     {
//         var db = FirebaseDatabase.GetInstance("https://upmaxxing-5205d-default-rtdb.firebaseio.com/").RootReference;

//         string key = db.Child("leaderboard").Push().Key;

//         Dictionary<string, object> entry = new Dictionary<string, object>();

//         entry["username"] = username;
//         entry["score"] = score;

//         db.Child("leaderboard").Child(key).SetValueAsync(entry);

//         Debug.Log("score has been SENNNTTTT");
//     }
// }
