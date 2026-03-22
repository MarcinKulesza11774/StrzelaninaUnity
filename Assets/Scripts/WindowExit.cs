using UnityEngine;
using System.Collections;

public class WindowExit : MonoBehaviour
{
    [Header("Modele okna")]
    [Tooltip("Niezbite okno – domyślnie aktywne")]
    public GameObject intactWindow;
    [Tooltip("Zbite okno – domyślnie nieaktywne")]
    public GameObject brokenWindow;
    [Tooltip("Kraty – znikają po użyciu śrubokrętu")]
    public GameObject bars;
    [Tooltip("Trigger wyjścia – aktywowany po rozbiciu szyby")]
    public GameObject exitTrigger;

    [Header("Efekty")]
    public GameObject glassPrefab;
    public AudioClip screwdriverSound;
    public AudioClip glassBreakSound;

    [Header("Zasięg interakcji z kratami")]
    public float interactRange = 2f;

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

        if (screwdriverSound != null)
            AudioSource.PlayClipAtPoint(screwdriverSound, transform.position);

        // Animacja znikania krat
        if (bars != null)
        {
            float t = 0f;
            Renderer r = bars.GetComponentInChildren<Renderer>();
            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                if (r != null)
                {
                    Color c = r.material.color;
                    c.a = 1f - t;
                    r.material.color = c;
                }
                yield return null;
            }
            bars.SetActive(false);
        }
    }

    public void OnGlassHit()
    {
        if (!barsRemoved || glassBroken) return;
        glassBroken = true;
        StartCoroutine(BreakGlass());
    }

    IEnumerator BreakGlass()
    {
        if (glassBreakSound != null)
            AudioSource.PlayClipAtPoint(glassBreakSound, transform.position);

        // Spawn kawałków szkła
        if (glassPrefab != null)
        {
            for (int i = 0; i < 8; i++)
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

        // Zamień model niezbity na zbity
        if (intactWindow != null) intactWindow.SetActive(false);
        if (brokenWindow != null) brokenWindow.SetActive(true);

        // Włącz wiatr we włosach
        HairPhysics[] hairComponents = Object.FindObjectsByType<HairPhysics>(FindObjectsSortMode.None);
        foreach (var hair in hairComponents)
            hair.SetWind(true, -transform.forward, 1.5f);

        yield return new WaitForSeconds(0.5f);

        if (exitTrigger != null) exitTrigger.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}