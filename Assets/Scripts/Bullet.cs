using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Reakcje z otoczeniem będą dodawane tu później:
    //   GetComponent<EnemyAI>()?.GetShot();
    //   GetComponent<Padlock>()?.Unlock();
    //   GetComponent<TurretTrap>()?.TakeHit();

    void OnCollisionEnter(Collision col)
    {
        // Ignoruj kolizję z graczem
        if (col.gameObject.CompareTag("Player")) return;

        // Efekt trafienia w miejsce kontaktu (opcjonalnie)
        // Instantiate(hitEffectPrefab, col.contacts[0].point, Quaternion.identity);

        Destroy(gameObject);
    }
}
