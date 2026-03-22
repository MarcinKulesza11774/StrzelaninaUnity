using UnityEngine;
using System.Collections;
using TMPro;

public class GunController : MonoBehaviour
{
    [Header("Naboje")]
    public int maxBullets = 5;
    private int bulletsLeft;

    [Header("Referencje")]
    public PlayerMovement playerMovement;
    public Transform muzzlePoint;
    public Camera playerCamera;
    public GameObject bulletPrefab;

    [Header("Pocisk")]
    public float bulletSpeed = 25f;
    public float bulletLifetime = 3f;
    public float maxRange = 100f;

    [Header("Celownik")]
    [Tooltip("wysokość celownika \n" +
             "100px na 1080p = 0.09, na 720p = 0.14")]
    public float crosshairOffsetY = 0.09f;

    [Header("Efekty")]
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;
    public AudioClip emptySound;

    [Header("Cooldown")]
    public float shootCooldown = 0.4f;

    [Header("UI")]
    public TextMeshProUGUI bulletsCounter;

    private AudioSource audioSource;
    private bool canShoot = true;

    public int BulletsLeft => bulletsLeft;

    void Start()
    {
        bulletsLeft = maxBullets;
        UpdateBullets(bulletsLeft, maxBullets);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot && playerMovement.IsAiming)
            StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        if (bulletsLeft <= 0)
        {
            if (emptySound != null) audioSource.PlayOneShot(emptySound);
            yield return new WaitForSeconds(shootCooldown);
            canShoot = true;
            yield break;
        }

        bulletsLeft--;
        UpdateBullets(bulletsLeft, maxBullets);

        if (shootSound != null) audioSource.PlayOneShot(shootSound, 2f);

        if (muzzleFlash != null)
        {
            muzzleFlash.Clear();
            muzzleFlash.Play();
        }

        // Punkt startowy raycast                             nieco przed kamerą, żeby nie wchodził w kolizję z postacią
        Vector3 rayOrigin = playerCamera.transform.position + playerCamera.transform.forward * 2f;
        // stworzenie promienia
        Ray aimRay = new Ray(rayOrigin, playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f + crosshairOffsetY, 0f)).direction);

        // ustalenie punktu trafienia
        Vector3 targetPoint;
        if (Physics.Raycast(aimRay, out RaycastHit hit, maxRange))
            targetPoint = hit.point;
        else
            targetPoint = aimRay.origin + aimRay.direction * maxRange;

        // stworzenie wektora strzału
        Vector3 shootDir = (targetPoint - muzzlePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzlePoint.position,
            Quaternion.LookRotation(shootDir)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = shootDir * bulletSpeed;
        }

        Destroy(bullet, bulletLifetime);

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    public void UpdateBullets(int bulletsLeft, int bulletsMax)
    {
        if (bulletsCounter != null)
            bulletsCounter.text = $"{bulletsLeft} / {bulletsMax}"
        ;
    }
}