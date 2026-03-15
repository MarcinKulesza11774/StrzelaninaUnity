using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("Naboje")]
    public int maxBullets = 6;
    private int bulletsLeft;

    [Header("Referencje")]
    public Transform muzzlePoint;
    public Camera playerCamera;
    public GameObject bulletPrefab;

    [Header("Pocisk")]
    public float bulletSpeed = 25f;
    public float bulletLifetime = 3f;
    public float maxRange = 100f;

    [Header("Celownik")]
    [Tooltip("Offset Y raycastu dopasowany do przesunięcia celownika w UI.\n" +
             "100px na 1080p = 0.09, na 720p = 0.14")]
    public float crosshairOffsetY = 0.09f;

    [Header("Efekty")]
    public ParticleSystem muzzleFlash;
    public AudioClip shootSound;
    public AudioClip emptySound;

    [Header("Cooldown")]
    public float shootCooldown = 0.4f;

    private AudioSource audioSource;
    private bool canShoot = true;

    public int BulletsLeft => bulletsLeft;

    void Start()
    {
        bulletsLeft = maxBullets;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
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

        if (shootSound != null) audioSource.PlayOneShot(shootSound);

        if (muzzleFlash != null)
        {
            muzzleFlash.Clear();
            muzzleFlash.Play();
        }

        // Raycast startuje przed postacią, kierunek dopasowany do celownika
        Vector3 rayOrigin = playerCamera.transform.position
                          + playerCamera.transform.forward * 2f;
        Ray aimRay = new Ray(rayOrigin,
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f + crosshairOffsetY, 0f)).direction);

        Vector3 targetPoint;
        if (Physics.Raycast(aimRay, out RaycastHit hit, maxRange))
            targetPoint = hit.point;
        else
            targetPoint = aimRay.origin + aimRay.direction * maxRange;

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

    public void AddBullets(int count)
    {
        bulletsLeft = Mathf.Min(bulletsLeft + count, maxBullets);
    }
}