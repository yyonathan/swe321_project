using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    public float delay = 2f;
    private bool isTriggered = false;

private void OnCollisionEnter2D(Collision2D collision)
{
    Debug.Log("TOUCH DETECTED: " + collision.gameObject.name);

    Debug.Log("Tag is: " + collision.gameObject.tag);

    if (collision.gameObject.CompareTag("Player") && !isTriggered)
    {
        Debug.Log("PLAYER CONFIRMED");
        isTriggered = true;
        StartCoroutine(Disappear());
    }
}

IEnumerator Disappear()
{
    Debug.Log("Coroutine started");

    yield return new WaitForSeconds(2f);

    Debug.Log("AFTER WAIT - about to disappear");

    gameObject.SetActive(false);
}
}