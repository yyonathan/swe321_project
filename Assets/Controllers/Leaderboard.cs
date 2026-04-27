using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;


public class Leaderboard : MonoBehaviour
{
    // Database URL Constant
    private const string DATABSEURL = "https://upmaxxing-5205d-default-rtdb.firebaseio.com/";

    // "subscribing" to the death event
    void Start()
    {
        FetchLeaderboard();
    }
    private void OnEnable()
    {
        PlayerDeath.OnPlayerDeath += HandleDeath;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDeath -= HandleDeath;
    }

    // this function will get called when the player dies, so feel free to add whatever you want in here
    // hopefully those 3 variables are enough to work with
    private void HandleDeath()
    {
        // player's locally saved name, should never be empty by this point
        string playerName = PlayerPrefs.GetString("PlayerName", "Unknown");

        // time survived in seconds
        float score = ScoreManager.Instance.CurrentScore;

        // only submit to Firebase if this is a new personal best
        float highScore = ScoreManager.Instance.PersonalBest;

        Debug.Log("Name: " + playerName);
        Debug.Log("Score: " + score);
        Debug.Log("Personal Best: " + highScore);

        SubmitScore(playerName, score);
        // if(score >= highScore)
        // {
        //     Debug.Log("New best made");
        //     SubmitScore(playerName, score);
        // } else
        // {
        //     Debug.Log("No new best showing");
        // }


        // here's my idea for the leaderboard: (if you want to implement it in a different way feel free)
        // each player will have only one entry in the database, storing their name, highest score, and date of the run.
        // a player's entry will be keyed by player's name
    }

    // this should fetch the entries from Firebase, sorted by score descending
    // i can handle passing the results to the UI for displaying purposes
    // you can change the reeturn type of this if you want it to just return some data structure holding the entries

    private void SubmitScore(string playerName, float score)
    {
        // Get firebase Instance
        var db = FirebaseDatabase.GetInstance(DATABSEURL).RootReference;
        DatabaseReference playerRef = db.Child("leaderboard").Child(playerName);


        playerRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            // if it got the info correctly it wil
            if (!task.IsCompletedSuccessfully)
            {
                Debug.LogError("Failed to check existing score: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;

            if (!snapshot.Exists)
            {
                Debug.Log("No Firebase score yet, submitting... ");
                SaveScore(playerRef, playerName, score);
                return;
            }

            float oldScore = float.Parse(snapshot.Child("score").Value.ToString());

            if (score > oldScore)
            {
                Debug.Log("New Best, submitting to firebase :DDDDDD ");
                SaveScore(playerRef, playerName, score);
            }
            else
            {
                Debug.Log("Current score is better, not going to submit, you suck");
            }
        });
    }

    private void SaveScore(DatabaseReference playerRef, string playerName, float score)
    {
        Dictionary<string, object> entry = new Dictionary<string, object>();
        entry["name"] = playerName;
        entry["score"] = score;

        playerRef.SetValueAsync(entry);

        Debug.Log("Submitted the score of: " + playerName + "score is: " + score);
    }

    public void FetchLeaderboard()
    {
        var db = FirebaseDatabase.GetInstance(DATABSEURL).GetReference("leaderboard");

        // Gets the order by score ---- gets whatever amount you want
        db.OrderByChild("score").LimitToLast(10).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompletedSuccessfully)
            {
                Debug.LogError("Failed to fetch the leaderboard whomp whomp" + task.Exception);
                return;
            }

            List<LeaderboardInfo> entries = new List<LeaderboardInfo>();

            foreach (DataSnapshot child in task.Result.Children)
            {
                string username = child.Child("name").Value.ToString();
                float score = float.Parse(child.Child("score").Value.ToString());

                entries.Add(new LeaderboardInfo(username, score));
            }

            entries.Reverse();

            //This will give you access.
            foreach (LeaderboardInfo entry in entries)
            {
                Debug.Log(entry.username + "has a score of: " + entry.score);
            }
        });
    }

}
public struct LeaderboardInfo
{
    public string username;
    public float score;

    public LeaderboardInfo(string username, float score)
    {
        this.username = username;
        this.score = score;
    }
}