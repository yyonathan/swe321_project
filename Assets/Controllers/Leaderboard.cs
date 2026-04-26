using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Leaderboard : MonoBehaviour
{
    // "subscribing" to the death event
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
        bool isNewBest = ScoreManager.Instance.IsNewBest;

        // here's my idea for the leaderboard: (if you want to implement it in a different way feel free)
        // each player will have only one entry in the database, storing their name, highest score, and date of the run.
        // a player's entry will be keyed by player's name
    }

    // this should fetch the entries from Firebase, sorted by score descending
    // i can handle passing the results to the UI for displaying purposes
    // you can change the reeturn type of this if you want it to just return some data structure holding the entries
    public void FetchLeaderboard()
    {
        
    }
}
