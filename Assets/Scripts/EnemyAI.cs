using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Video;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { GeekedOut, LockedIn, Dead }

    [Header("Referencje")]
    public Transform player;
    public GameObject visualObject;

    [Header("Geeked Out")]
    [Range(1f, 10f)] public float wanderInterval = 3f;
    [Range(2f, 20f)] public float wanderRadius = 8f;
    [Range(0f, 1f)] public float playerBias = 0.4f;
    public float geekedOutSpeed = 2.5f;

    [Header("Locked In")]
    public float lockedInSpeed = 5f;
    [Range(10f, 180f)] public float fovAngle = 90f;
    [Range(2f, 30f)] public float fovRange = 10f;
    [Range(2f, 20f)] public float sprintDetectionRange = 6f;
    public PlayerMovement playerMovement;
    public LayerMask wallLayers;

    [Header("Złapanie gracza")]
    public float catchRange = 1.2f;

    [Header("Jumpscare")]
    public GameObject jumpscareCanvas;
    public VideoPlayer jumpscareVideo;
    public float jumpscareTime = 2f;

    [Header("Respawn")]
    public float respawnDelay = 30f;
    private Vector3 spawnPosition;

    [Header("Śmierć")]
    public ParticleSystem bloodSplatter;
    public AudioClip deathScream;

    [Header("Dźwięki")]
    public AudioClip stepSound;
    public AudioClip randomSound;
    [Range(1f, 30f)] public float randomSoundMinInterval = 5f;
    [Range(1f, 60f)] public float randomSoundMaxInterval = 15f;

    [Header("3D Audio")]
    public float audioMinDistance = 1f;
    public float audioMaxDistance = 20f;

    private NavMeshAgent agent;
    private EnemyState state = EnemyState.GeekedOut;
    private float wanderTimer;
    private AudioSource stepAudioSource;
    private AudioSource randomAudioSource;
    private AudioSource screamAudioSource;
    private float randomSoundTimer;
    private Collider enemyCollider;
    private bool jumpscareTriggered = false;
    private Vector3 lastKnownPlayerPos;
    private float lockedInSearchTimer = 0f;
    private const float lockedInSearchTime = 3f;
    private bool playerVisible = false;
    private bool playerSprinting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponentInChildren<Collider>();
        spawnPosition = transform.position;
        wanderTimer = wanderInterval;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerMovement == null && player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        if (visualObject == null && transform.childCount > 0)
            visualObject = transform.GetChild(0).gameObject;

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(false);

        SetupAudio();
        ResetRandomSoundTimer();
        StartCoroutine(PerceptionLoop());
    }

    void SetupAudio()
    {
        stepAudioSource = gameObject.AddComponent<AudioSource>();
        stepAudioSource.clip = stepSound;
        stepAudioSource.loop = true;
        stepAudioSource.playOnAwake = false;
        Setup3D(stepAudioSource);

        randomAudioSource = gameObject.AddComponent<AudioSource>();
        randomAudioSource.loop = false;
        randomAudioSource.playOnAwake = false;
        Setup3D(randomAudioSource);

        screamAudioSource = gameObject.AddComponent<AudioSource>();
        screamAudioSource.loop = false;
        screamAudioSource.playOnAwake = false;
        Setup3D(screamAudioSource);
    }

    void Setup3D(AudioSource src)
    {
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = audioMinDistance;
        src.maxDistance = audioMaxDistance;
        src.dopplerLevel = 0f;
    }

    IEnumerator PerceptionLoop()
    {
        while (true)
        {
            if (state != EnemyState.Dead && player != null)
            {
                playerVisible = CanSeePlayer();
                playerSprinting = PlayerIsSprinting();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Update()
    {
        if (state == EnemyState.Dead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!jumpscareTriggered && dist <= catchRange)
        {
            jumpscareTriggered = true;
            StartCoroutine(DoJumpscare());
            return;
        }

        switch (state)
        {
            case EnemyState.GeekedOut: UpdateGeekedOut(); break;
            case EnemyState.LockedIn: UpdateLockedIn(); break;
        }

        UpdateSounds();
    }

    bool CanSeePlayer()
    {
        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        if (dist > fovRange) return false;

        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > fovAngle * 0.5f) return false;

        Vector3 eyePos = transform.position + Vector3.up * 1.5f;
        Vector3 playerEyePos = player.position + Vector3.up * 1.5f;
        if (Physics.Raycast(eyePos, (playerEyePos - eyePos).normalized, dist, wallLayers))
            return false;

        return true;
    }

    bool PlayerIsSprinting()
    {
        if (playerMovement == null) return false;
        float dist = Vector3.Distance(transform.position, player.position);
        return dist <= sprintDetectionRange && playerMovement.IsSprinting;
    }

    void UpdateGeekedOut()
    {
        agent.speed = geekedOutSpeed;

        if (playerVisible || playerSprinting)
        {
            lastKnownPlayerPos = player.position;
            state = EnemyState.LockedIn;
            return;
        }

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f || IsAgentIdle())
        {
            agent.SetDestination(PickWanderTarget());
            wanderTimer = wanderInterval + Random.Range(-1f, 1f);
        }
    }

    void UpdateLockedIn()
    {
        agent.speed = lockedInSpeed;

        if (playerVisible || playerSprinting)
        {
            lastKnownPlayerPos = player.position;
            lockedInSearchTimer = 0f;
            agent.SetDestination(lastKnownPlayerPos);
        }
        else
        {
            agent.SetDestination(lastKnownPlayerPos);
            lockedInSearchTimer += Time.deltaTime;

            if (lockedInSearchTimer >= lockedInSearchTime)
            {
                lockedInSearchTimer = 0f;
                state = EnemyState.GeekedOut;
                wanderTimer = 0f;
            }
        }
    }

    void UpdateSounds()
    {
        if (stepSound != null && !stepAudioSource.isPlaying)
            stepAudioSource.Play();

        if (randomSound != null)
        {
            randomSoundTimer -= Time.deltaTime;
            if (randomSoundTimer <= 0f)
            {
                randomAudioSource.PlayOneShot(randomSound);
                ResetRandomSoundTimer();
            }
        }
    }

    void ResetRandomSoundTimer()
    {
        randomSoundTimer = Random.Range(randomSoundMinInterval, randomSoundMaxInterval);
    }

    bool IsAgentIdle()
    {
        return !agent.pathPending
            && agent.remainingDistance < 0.5f
            && agent.velocity.sqrMagnitude < 0.1f;
    }

    Vector3 PickWanderTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 toPlayer = (player.position - transform.position).normalized * wanderRadius;
        Vector3 biasedPoint = transform.position
                             + Vector3.Lerp(randomOffset, toPlayer, playerBias);

        if (NavMesh.SamplePosition(biasedPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    IEnumerator DoJumpscare()
    {
        state = EnemyState.Dead;
        agent.isStopped = true;
        stepAudioSource.Stop();

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(true);

        yield return null;

        if (jumpscareVideo != null)
        {
            jumpscareVideo.Prepare();
            yield return new WaitUntil(() => jumpscareVideo.isPrepared);
            jumpscareVideo.Play();
            yield return new WaitUntil(() => jumpscareVideo.isPlaying);
            yield return new WaitUntil(() =>
                !jumpscareVideo.isPlaying ||
                jumpscareVideo.time >= jumpscareVideo.length - 0.1);
            jumpscareVideo.Stop();
        }
        else
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(jumpscareTime);
            Time.timeScale = 1f;
        }

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(false);

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void GetShot()
    {
        if (state == EnemyState.Dead) return;
        if (deathScream != null) screamAudioSource.PlayOneShot(deathScream);

        if (bloodSplatter != null)
        {
            bloodSplatter.Clear();
            bloodSplatter.Play();
        }

        StartCoroutine(RespawnSequence());
    }

    IEnumerator RespawnSequence()
    {
        state = EnemyState.Dead;
        agent.isStopped = true;
        stepAudioSource.Stop();

        yield return new WaitForSeconds(5);

        if (enemyCollider != null) enemyCollider.enabled = false;
        if (visualObject != null) visualObject.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        agent.Warp(spawnPosition);
        if (enemyCollider != null) enemyCollider.enabled = true;
        if (visualObject != null) visualObject.SetActive(true);
        agent.isStopped = false;
        state = EnemyState.GeekedOut;
        wanderTimer = 0f;
        jumpscareTriggered = false;
        ResetRandomSoundTimer();
    }
}