using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public GameObject winUI;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        Win();
    }

    void Win()
    {
        Time.timeScale = 0f;
        winUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
