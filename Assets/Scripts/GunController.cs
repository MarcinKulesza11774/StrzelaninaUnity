using UnityEngine;
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
    [Tooltip("Wysokość celownika.\n100px = 0.09")]
    public float crosshairOffsetY = 0.09f;

    [Header("Efekty")]
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;
    public AudioClip emptySound;
    public float shotVolume = 6f;

    [Header("Cooldown")]
    public float shootCooldown = 0.4f;
    private float shootTimer = 0f;

    [Header("UI")]
    public TextMeshProUGUI bulletsCounter;

    private AudioSource audioSource;

    public int BulletsLeft => bulletsLeft;

    void Start()
    {
        bulletsLeft = maxBullets;
        UpdateBullets();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (shootTimer > 0f)
            shootTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && shootTimer <= 0f && playerMovement.IsAiming)
            Shoot();
    }

    void Shoot()
    {
        shootTimer = shootCooldown;

        if (bulletsLeft <= 0)
        {
            if (emptySound != null) audioSource.PlayOneShot(emptySound);
            return;
        }

        bulletsLeft--;
        UpdateBullets();

        if (shootSound != null) audioSource.PlayOneShot(shootSound, shotVolume);

        if (muzzleFlash != null)
        {
            muzzleFlash.Clear();
            muzzleFlash.Play();
        }

        Vector3 rayOrigin = playerCamera.transform.position + playerCamera.transform.forward * 2f;

        Ray aimRay = new Ray(rayOrigin, playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f + crosshairOffsetY, 0f)).direction);

        Vector3 targetPoint = Physics.Raycast(aimRay, out RaycastHit hit, maxRange) ? hit.point : aimRay.origin + aimRay.direction * maxRange;

        Vector3 shootDir = (targetPoint - muzzlePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(shootDir));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = shootDir * bulletSpeed;
        }

        Destroy(bullet, bulletLifetime);
    }

    void UpdateBullets()
    {
        if (bulletsCounter != null)
            bulletsCounter.text = $"{bulletsLeft} / {maxBullets}";
    }
}