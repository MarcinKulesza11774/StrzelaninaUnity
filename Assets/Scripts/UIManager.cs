using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TextMeshProUGUI bulletsText;
    public TextMeshProUGUI messageText;

    [Header("Ekran wygranej")]
    public GameObject winPanel;

    private Coroutine msgCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateBullets(int current, int max)
    {
        if (bulletsText != null)
            bulletsText.text = $"Naboje: {current} / {max}";
    }

    public void ShowMessage(string msg, float duration = 3f)
    {
        if (msgCoroutine != null) StopCoroutine(msgCoroutine);
        msgCoroutine = StartCoroutine(ShowMsgCoroutine(msg, duration));
    }

    public void HideMessage()
    {
        if (msgCoroutine != null) StopCoroutine(msgCoroutine);
        if (messageText != null) messageText.text = "";
    }

    IEnumerator ShowMsgCoroutine(string msg, float duration)
    {
        if (messageText != null) messageText.text = msg;
        yield return new WaitForSeconds(duration);
        if (messageText != null) messageText.text = "";
    }

    public void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }
}
