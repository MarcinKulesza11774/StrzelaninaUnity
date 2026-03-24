using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletHolePrefab;
    public float bulletHoleLifetime = 30f;
    public float decalOffset = 0.01f;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player")) return;

        col.gameObject.GetComponentInParent<EnemyAI>()?.GetShot();
        col.gameObject.GetComponentInParent<WindowExit>()?.OnGlassHit();
        var windowExit = col.gameObject.GetComponentInParent<WindowExit>();
        Debug.Log("Trafiono: " + col.gameObject.name + " | WindowExit: " + windowExit);

        if (bulletHolePrefab != null && col.contacts.Length > 0)
        {
            ContactPoint contact = col.contacts[0];
            Vector3 spawnPos = contact.point + contact.normal * decalOffset;
            Quaternion spawnRot = Quaternion.LookRotation(-contact.normal);
            spawnRot *= Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            GameObject hole = Instantiate(bulletHolePrefab, spawnPos, spawnRot);
            hole.transform.SetParent(col.transform);

            if (bulletHoleLifetime > 0f)
                Destroy(hole, bulletHoleLifetime);
        }

        Destroy(gameObject);
    }
}