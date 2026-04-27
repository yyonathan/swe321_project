using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private GameObject _leaderboardCanvas;
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _rowPrefab;
    [SerializeField] private Leaderboard _leaderboard;

    // called by the leaderboard button on TitleCanvas
    public void OpenLeaderboard()
    {
        _leaderboardCanvas.SetActive(true);

        Debug.Log("About to call DisplayEntries");

        _leaderboard.FetchLeaderboard(DisplayEntries);
    }

    // called by the close button on LeaderboardCanvas
    public void CloseLeaderboard()
    {
        _leaderboardCanvas.SetActive(false);
    }

    private void DisplayEntries(List<LeaderboardEntry> entries)
    {
        Debug.Log("DisplayEntries called with " + entries.Count + " entries. Content: " + _content);

        // clear existing rows
        foreach (Transform child in _content)
            Destroy(child.gameObject);

        for (int i = 0; i < entries.Count; i++)
        {
            Debug.Log("Row prefabb: " + _rowPrefab);
            Debug.Log("Instantiating row " + i);
            GameObject row = Instantiate(_rowPrefab, _content);
            Debug.Log("Row instantiated: " + row.name + " parernt: " + row.transform.parent.name);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();

            // order matches: RankText, NameText, ScoreText, DateText
            texts[0].text = "#" + (i + 1);
            texts[1].text = entries[i].name;
            texts[2].text = entries[i].score.ToString("F2");
            string rawDate = entries[i].date;
            string displayDate = "N/A";

            if (!string.IsNullOrEmpty(rawDate))
            {
                System.DateTime parsed;
                if (System.DateTime.TryParse(rawDate, out parsed))
                    displayDate = parsed.ToString("M/d/yy");
            }

            texts[3].text = displayDate;
        }
    }
}