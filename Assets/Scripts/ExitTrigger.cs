using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public AudioClip winSound;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        UIManager.Instance?.ShowMessage("Udało ci się uciec", 1f);
        StopGame();
    }

    void StopGame()
    {
        Time.timeScale = 0f;
    }
}
