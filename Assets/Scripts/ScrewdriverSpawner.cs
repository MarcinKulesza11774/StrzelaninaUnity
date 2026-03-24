using UnityEngine;

public class ScrewdriverSpawner : MonoBehaviour
{
    void Start()
    {
        int count = transform.childCount;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        int chosen = Random.Range(0, count);
        transform.GetChild(chosen).gameObject.SetActive(true);
    }
}
