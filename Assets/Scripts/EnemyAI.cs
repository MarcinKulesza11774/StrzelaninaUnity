using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { GeekedOut, LockedIn, Dead }

    [Header("Referencje")]
    public Transform player;

    [Header("Geeked Out – szukanie gracza")]
    [Tooltip("Co ile sekund przeciwnik wybiera nowy punkt wędrówki")]
    [Range(1f, 10f)] public float wanderInterval = 3f;
    [Tooltip("Jak daleko od gracza może wybrać punkt wędrówki – im wyżej tym bardziej losowo")]
    [Range(2f, 20f)] public float wanderRadius = 8f;
    [Tooltip("Jak bardzo punkt wędrówki jest przyciągany w stronę gracza (0=czysto losowo, 1=prosto do gracza)")]
    [Range(0f, 1f)] public float playerBias = 0.4f;
    public float geekedOutSpeed = 2.5f;

    [Header("Złapanie gracza")]
    [Tooltip("Odległość w której przeciwnik łapie gracza")]
    public float catchRange = 1.2f;

    [Header("Jumpscare")]
    public GameObject jumpscareCanvas;
    public float jumpscareTime = 2f;

    [Header("Respawn")]
    public float respawnDelay = 30f;
    private Vector3 spawnPosition;

    // ── Stan wewnętrzny ────────────────────────────────────────────────────
    private NavMeshAgent agent;
    private EnemyState   state = EnemyState.GeekedOut;
    private float        wanderTimer;

    void Start()
    {
        agent         = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;
        wanderTimer   = wanderInterval;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(false);
    }

    void Update()
    {
        if (state == EnemyState.Dead || player == null) return;

        // Sprawdź czy złapał gracza
        if (Vector3.Distance(transform.position, player.position) <= catchRange)
        {
            StartCoroutine(DoJumpscare());
            return;
        }

        if (state == EnemyState.GeekedOut)
            UpdateGeekedOut();
    }

    void UpdateGeekedOut()
    {
        agent.speed = geekedOutSpeed;

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f || IsAgentIdle())
        {
            Vector3 target = PickWanderTarget();
            agent.SetDestination(target);
            wanderTimer = wanderInterval + Random.Range(-1f, 1f);
        }
    }

    Vector3 PickWanderTarget()
    {
        // Losowy punkt w kółku wokół przeciwnika
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset  = new Vector3(randomCircle.x, 0f, randomCircle.y);

        // Przyciągnij punkt w stronę gracza (playerBias)
        Vector3 toPlayer    = (player.position - transform.position).normalized * wanderRadius;
        Vector3 biasedPoint = transform.position
                            + Vector3.Lerp(randomOffset, toPlayer, playerBias);

        // Znajdź najbliższy punkt na NavMesh
        if (NavMesh.SamplePosition(biasedPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    bool IsAgentIdle()
    {
        return !agent.pathPending
            && agent.remainingDistance < 0.5f
            && agent.velocity.sqrMagnitude < 0.1f;
    }

    IEnumerator DoJumpscare()
    {
        state           = EnemyState.Dead;
        agent.isStopped = true;

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(true);

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(jumpscareTime);
        Time.timeScale = 1f;

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(false);

        // Reset sceny
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    // Wywołane przez Bullet.cs gdy pocisk trafi w wroga
    public void GetShot()
    {
        if (state == EnemyState.Dead) return;
        StartCoroutine(RespawnSequence());
    }

    IEnumerator RespawnSequence()
    {
        state           = EnemyState.Dead;
        agent.isStopped = true;
        GetComponent<Collider>().enabled = false;

        // Ukryj mesh (zamiast animacji śmierci na razie)
        GetComponent<Renderer>()?.gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        // Teleport na pozycję startową
        agent.Warp(spawnPosition);
        GetComponent<Collider>().enabled = true;
        GetComponent<Renderer>()?.gameObject.SetActive(true);
        agent.isStopped = false;
        state           = EnemyState.GeekedOut;
        wanderTimer     = 0f;
    }

    // Podepnij tu później tryb LockedIn
    public void SetLockedIn()
    {
        state = EnemyState.LockedIn;
        // TODO: biegnie prosto do gracza z większą prędkością
    }
}
