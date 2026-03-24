using UnityEngine;
using System.Collections;

public class WindowExit : MonoBehaviour
{
    public GameObject intactWindow;
    public GameObject brokenWindow;
    public GameObject bars;
    public GameObject exitTrigger;

    [Header("Efekty")]
    public GameObject glassPrefab;
    public AudioClip glassBreakSound;

    [Header("Zasięg interakcji z kratami")]
    public float interactRange = 2f;

    [Header("Wiatr")]
    public float windStrength = -20f;

    private bool barsRemoved = false;
    private bool glassBroken = false;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (intactWindow != null) intactWindow.SetActive(true);
        if (brokenWindow != null) brokenWindow.SetActive(false);
        if (exitTrigger != null) exitTrigger.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool playerNear = dist <= interactRange;

        if (playerNear && !barsRemoved && Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerInventory.Instance != null && PlayerInventory.Instance.HasScrewdriver)
                StartCoroutine(RemoveBars());
        }

        if (playerNear && !barsRemoved)
        {
            if (PlayerInventory.Instance != null && PlayerInventory.Instance.HasScrewdriver)
                UIManager.Instance?.ShowMessage("[E] Odkręć kraty");
            else
                UIManager.Instance?.ShowMessage("kraty w oknach. może dałoby się je zdjąć śrubokrętem");
        }
        else if (!playerNear)
        {
            UIManager.Instance?.HideMessage();
        }
    }

    IEnumerator RemoveBars()
    {
        barsRemoved = true;

        if (bars != null)
        {
            Renderer r = bars.GetComponentInChildren<Renderer>();
            yield return null;
            bars.SetActive(false);
        }
    }

    public void OnGlassHit()
    {
        if (!barsRemoved || glassBroken) return;
        glassBroken = true;
        BreakGlass();
    }

    void BreakGlass()
    {
        if (glassBreakSound != null)
            AudioSource.PlayClipAtPoint(glassBreakSound, transform.position);

        if (glassPrefab != null)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * 0.3f;
                GameObject shard = Instantiate(glassPrefab, pos, Random.rotation);
                Rigidbody rb = shard.GetComponent<Rigidbody>();
                if (rb != null)
                    rb.AddForce(
                        (Random.insideUnitSphere + transform.forward) * Random.Range(2f, 6f),
                        ForceMode.Impulse);
                Destroy(shard, 4f);
            }
        }

        if (intactWindow != null) intactWindow.SetActive(false);
        if (brokenWindow != null) brokenWindow.SetActive(true);

        HairPhysics[] hairComponents = Object.FindObjectsByType<HairPhysics>(FindObjectsSortMode.None);
        foreach (var hair in hairComponents)
            hair.SetWind(true, -transform.forward, windStrength);

        if (exitTrigger != null) exitTrigger.SetActive(true);
    }
}