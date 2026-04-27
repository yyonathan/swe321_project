using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;


public class Leaderboard : MonoBehaviour
{
    // Database URL Constant
    private const string DATABSEURL = "https://upmaxxing-5205d-default-rtdb.firebaseio.com/";
    private const int MAX_ENTRIES = 50;
    private float _pendingScore;

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
        _pendingScore = ScoreManager.Instance.CurrentScore;

        // here's my idea for the leaderboard: (if you want to implement it in a different way feel free)
        // each player will have only one entry in the database, storing their name, highest score, and date of the run.
        // a player's entry will be keyed by player's name
    }

    public void Submit(string playerName)
    {
        StartCoroutine(SubmitScore(playerName, _pendingScore));
    }


    // this should fetch the entries from Firebase, sorted by score descending
    // i can handle passing the results to the UI for displaying purposes
    // you can change the reeturn type of this if you want it to just return some data structure holding the entries
    private IEnumerator SubmitScore(string playerName, float score)
    {
        // first check if player already has an entry
        string url = DATABSEURL + "leaderboard/" + playerName + ".json";
        UnityWebRequest getRequest = UnityWebRequest.Get(url);
        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to check existing score: " + getRequest.error);
            yield break;
        }

        string response = getRequest.downloadHandler.text;

        // if entry exists, check if new score is better
        if (response != "null")
        {
            LeaderboardEntry existing = JsonUtility.FromJson<LeaderboardEntry>(response);
            if (score <= existing.score)
            {
                Debug.Log("Not a new best, not submitting.");
                yield break;
            }
        }

        // submit new/updated entry
        string date = System.DateTime.UtcNow.ToString("M/d/yy");
        LeaderboardEntry entry = new LeaderboardEntry { name = playerName, score = score, date = date };
        string json = JsonUtility.ToJson(entry);

        UnityWebRequest putRequest = UnityWebRequest.Put(url, Encoding.UTF8.GetBytes(json));
        putRequest.SetRequestHeader("Content-Type", "application/json");
        yield return putRequest.SendWebRequest();

        if (putRequest.result == UnityWebRequest.Result.Success)
            Debug.Log("Score submitted: " + playerName + " - " + score);
        else
            Debug.LogError("Failed to submit score: " + putRequest.error);
    }

    public void FetchLeaderboard(System.Action<List<LeaderboardEntry>> onComplete)
    {
        StartCoroutine(FetchLeaderboardCoroutine(onComplete));
    }

    private IEnumerator FetchLeaderboardCoroutine(System.Action<List<LeaderboardEntry>> onComplete)
    {
        string url = DATABSEURL + "leaderboard.json?orderBy=\"score\"&limitToLast=" + MAX_ENTRIES;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch leaderboard: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log("Raw JSOnN: " + json);

        if (json == "null" || string.IsNullOrEmpty(json))
        {
            Debug.Log("No leaderboard entries yett.");
            yield break;
        }

        List<LeaderboardEntry> entries = ParseEntries(json);
        entries.Sort((a, b) => b.score.CompareTo(a.score));
        onComplete?.Invoke(entries);
    }

    private List<LeaderboardEntry> ParseEntries(string json)
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        // split by top level keys: "},\"" pattern
        json = json.Substring(1, json.Length - 2); // strip outer {}

        int depth = 0;

        for (int i = 0; i < json.Length; i++)
        {
            if (json[i] == '{') depth++;
            else if (json[i] == '}')
            {
                depth--;
                if (depth == 0)
                {
                    // extract just the object {}
                    int objStart = json.LastIndexOf('{', i);
                    string entryJson = json.Substring(objStart, i - objStart + 1);
                    try
                    {
                        LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(entryJson);
                        entries.Add(entry);
                    }
                    catch { }
                }
            }
        }

        return entries;
    }
}

[System.Serializable]
public struct LeaderboardEntry
{
    public string name;
    public float score;
    public string date;
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