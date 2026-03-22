using UnityEngine;

/// <summary>
/// Trigger wyjścia – gdy gracz wejdzie, gra się kończy.
/// Dodaj do pustego obiektu z Collider (Is Trigger = true) w otworze okna.
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    public AudioClip winSound;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (winSound != null)
            AudioSource.PlayClipAtPoint(winSound, transform.position);

        // Pokaż ekran wygranej lub załaduj scenę końcową
        UIManager.Instance?.ShowMessage("Udało ci się uciec", 1f);

        // Opcjonalnie: załaduj scenę wygranej
        // UnityEngine.SceneManagement.SceneManager.LoadScene("WinScene");

        // Na razie zatrzymaj grę po chwili
        //Invoke(nameof(StopGame), 3f);
        StopGame();
    }

    void StopGame()
    {
        Time.timeScale = 0f;
    }
}
