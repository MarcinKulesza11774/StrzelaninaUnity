using UnityEngine;
public class Screwdriver : MonoBehaviour
{
    [Tooltip("Zasięg interakcji")]
    public float interactRange = 2f;
    public AudioClip pickupSound;

    private Transform player;
    private bool playerNearby = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        playerNearby = dist <= interactRange;

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
            Pickup();
    }

    void Pickup()
    {
        PlayerInventory.Instance.HasScrewdriver = true;
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        UIManager.Instance?.ShowMessage("Zebrano śrubokręt!", 2f);
        Destroy(gameObject);
    }
}
