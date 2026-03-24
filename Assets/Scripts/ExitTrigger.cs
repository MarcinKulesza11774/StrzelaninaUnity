using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitTrigger : MonoBehaviour
{
    public AudioClip winSound;
    public string winMessage = "Udało ci się uciec";

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || triggered) return;
        triggered = true;
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        if (winSound != null)
            AudioSource.PlayClipAtPoint(winSound, transform.position);

        UIManager.Instance?.ShowMessage(winMessage, 999f);
        Time.timeScale = 0f;

        yield return new WaitUntil(() => Input.anyKeyDown);

        Time.timeScale = 1f;
        UIManager.Instance?.HideMessage();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}