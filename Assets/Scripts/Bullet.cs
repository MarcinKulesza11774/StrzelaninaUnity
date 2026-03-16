using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Decal dziury po kuli")]
    public GameObject bulletHolePrefab;
    [Tooltip("Jak długo zostaje dziura na ścianie (sekundy). 0 = zostaje na zawsze")]
    public float bulletHoleLifetime = 30f;
    [Tooltip("Mały offset żeby decal nie migotał z powierzchnią (Z-fighting)")]
    public float decalOffset = 0.01f;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player")) return;

        // Spawn decala w miejscu trafienia
        if (bulletHolePrefab != null && col.contacts.Length > 0)
        {
            ContactPoint contact = col.contacts[0];

            // Pozycja lekko nad powierzchnią żeby uniknąć Z-fighting
            Vector3 spawnPos = contact.point + contact.normal * decalOffset;
            // Rotacja: decal "leży" na powierzchni – obrót zgodny z normalną ściany
            Quaternion spawnRot = Quaternion.LookRotation(-contact.normal);
            // Losowy obrót wokół normalnej dla różnorodności
            spawnRot *= Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            GameObject hole = Instantiate(bulletHolePrefab, spawnPos, spawnRot);

            // Przypiąć do ściany żeby nie zostawał w powietrzu gdy ściana się rusza
            hole.transform.SetParent(col.transform);

            if (bulletHoleLifetime > 0f)
                Destroy(hole, bulletHoleLifetime);
        }

        // Reakcje z otoczeniem – do uzupełnienia:
        // col.gameObject.GetComponent<EnemyAI>()?.GetShot();
        // col.gameObject.GetComponent<Padlock>()?.Unlock();
        // col.gameObject.GetComponent<TurretTrap>()?.TakeHit();

        Destroy(gameObject);
    }
}