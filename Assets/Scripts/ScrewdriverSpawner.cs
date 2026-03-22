using UnityEngine;

/// <summary>
/// Losowo aktywuje jeden śrubokręt spośród dzieci tego obiektu.
/// Wrzuć wszystkie obiekty śrubokrętów jako dzieci tego obiektu.
/// </summary>
public class ScrewdriverSpawner : MonoBehaviour
{
    void Start()
    {
        int count = transform.childCount;
        if (count == 0) return;

        // Wyłącz wszystkie
        for (int i = 0; i < count; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        // Włącz losowy jeden
        int chosen = Random.Range(0, count);
        transform.GetChild(chosen).gameObject.SetActive(true);
    }
}
