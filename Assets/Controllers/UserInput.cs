using UnityEngine;

public class UserInput : MonoBehaviour
{
    private string currentInput = "";
    private bool hasEnteredName = false;
    void Update()
    {
        if (hasEnteredName) return;
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // This will be for the backspace :DDDDD
            {
                if (currentInput.Length > 0) currentInput = currentInput.Substring(0, currentInput.Length - 1);
            }
            else if (c == '\n' || c == '\r') // Enter pressed 
            {
                SaveName(currentInput);
                currentInput = "";
                hasEnteredName = true;
            }
            else
            {
                currentInput += c;
            }
        }
    }

    void SaveName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
        PlayerPrefs.Save();

        Debug.Log("Saved name: " + name);
    }
}