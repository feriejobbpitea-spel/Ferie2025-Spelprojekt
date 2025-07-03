using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static BossStateController;

public class BossAttackHandler : MonoBehaviour
{
    public Transform throwPoint;
    public Transform earthWavePoint;
    public Collider2D[] colliders;
    private Transform player;
    private Rigidbody2D rb;

    [Header("Prefabs")]
    public GameObject throwablePrefab;
    public GameObject earthWavePrefab;
    public GameObject platformPrefab;

    [Header("Settings")]
    public float jumpChargeTime = 1f;
    public float jumpForce = 10f;
    public float slamDelay = 1.5f;
    public float throwForce = 10f;
    public float attackCooldown = 3f;
    public float hoverDuration = 2f;
    public float hoverSpeed = 2f;

    [Header("EarthWave Settings")]
    public int earthWaveCount = 5;
    public float earthWaveSpacing = 1f;
    public float earthWaveSpeed = 5f;
    public float delayBetweenWaves = 0.2f;
    public float disappearDelay = 3f;
    public float distanceUp = 3f;
    public float startDistanceDown = 3f;

    [Header("Platform Spawn Settings")]
    public int platformCount = 3;
    public float platformSpacing = 2f;
    public float platformYPosition = 3f;
    public float platformFadeDuration = 0.5f;
    public float platformLifeTime = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Audio")]
    public AudioClip flySound;
    public AudioClip slamSound;
    private AudioSource audioSource;
    private AudioSource voiceAudioSource; // separat för tal

    [Header("Voicelines - Välj ett eller flera LocalizedAudioClips per attacktyp")]
    public List<LocalizedAudioClip> slamVoicelines = new List<LocalizedAudioClip>();
    public List<LocalizedAudioClip> jumpVoicelines = new List<LocalizedAudioClip>();
    public List<LocalizedAudioClip> throwVoicelines = new List<LocalizedAudioClip>();
    public List<LocalizedAudioClip> tauntVoicelines = new List<LocalizedAudioClip>();

    public delegate void AttackEvent();
    public event AttackEvent OnSlam;
    public event AttackEvent OnFly;
    public event AttackEvent OnThrow;

    private BossStateController stateController;

    // ➕ NY FLAGGA FÖR ATTACK LOOP
    private bool isAttackLoopRunning = false;

    private void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        stateController = GetComponent<BossStateController>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        voiceAudioSource = gameObject.AddComponent<AudioSource>();

        // Vänta på att lokalisation ska vara redo
        if (!LocalizationSettings.InitializationOperation.IsDone)
        {
            StartCoroutine(WaitForLocalization());
        }
        else
        {
            Debug.Log("Aktuellt språk vid start: " + LocalizationSettings.SelectedLocale.LocaleName);
        }
    }

    private IEnumerator WaitForLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;
        if (LocalizationSettings.AvailableLocales.Locales.Count > 0)
        {
            Debug.Log("Lokalisering klar. Aktuellt språk: " + LocalizationSettings.SelectedLocale.LocaleName);
        }
    }

    // ➕ NY METOD: Starta en evig attack-loop
    public void StartAttackLoop()
    {
        if (!isAttackLoopRunning)
        {
            StartCoroutine(AttackLoopCoroutine());
        }
    }

    // ➕ NY METOD: Stoppa attack-loopen
    public void StopAttackLoop()
    {
        isAttackLoopRunning = false;
    }

    // ➕ NY KORUTIN: Loopar PerformRandomAttack()
    private IEnumerator AttackLoopCoroutine()
    {
        isAttackLoopRunning = true;

        while (isAttackLoopRunning)
        {
            yield return PerformRandomAttack();
        }
    }

    public IEnumerator PerformRandomAttack()
    {
        // Slumpa om bossen säger något innan attack
        if (Random.value < 0.4f && tauntVoicelines.Count > 0)
        {
            PlayRandomTauntVoiceline();
        }

        int attackType = Random.Range(0, 2);
        if (attackType == 0)
            yield return SlamAttack();
        else
            yield return ThrowAttack();

        yield return new WaitForSeconds(attackCooldown);
    }

    private void SetCollisionStatus(bool enabled)
    {
        foreach (var item in colliders)
        {
            item.enabled = enabled;
        }
    }
    [SerializeField] private GameObject shadow;  // Dra in skuggan i inspector
    [SerializeField] private float shadowYPosition = 0f; // Höjden på marken/skuggan ska ligga på

    private IEnumerator SlamAttack()
    {
        SpawnPlatforms();
        LookAtPlayer();
        OnFly?.Invoke();
        PlayRandomJumpVoiceline();

        if (flySound != null)
            audioSource.PlayOneShot(flySound);

        

        yield return new WaitForSeconds(jumpChargeTime);

        float offScreenY = transform.position.y + 15;
        rb.linearVelocity = new Vector2(0, jumpForce);
        rb.gravityScale = 0f;
        SetCollisionStatus(false);

        // Kör en loop för att uppdatera skuggans position under flygningen
        while (transform.position.y < offScreenY)
        {
            shadow.transform.position = new Vector3(transform.position.x, shadowYPosition, shadow.transform.position.z);
            yield return null;
        }

        Vector2 targetPosition = player.position;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        var hit = Physics2D.Raycast(player.position, Vector2.up, 100, groundLayer);
        rb.position = new Vector3(targetPosition.x, hit.point.y - 1, transform.position.z);

        // Se till att skuggan följer bossen även vid positionuppdateringen
        shadow.transform.position = new Vector3(rb.position.x, shadowYPosition, shadow.transform.position.z);
        shadow.SetActive(true);  // Visa skuggan när bossen hoppar
        yield return new WaitForSeconds(0.3f);

        rb.linearVelocity = new Vector2(0, -jumpForce);
        rb.gravityScale = 1f;

        // Skuggan kan vara kvar medan bossen faller ner
        while (!IsGrounded())
        {
            shadow.transform.position = new Vector3(transform.position.x, shadowYPosition, shadow.transform.position.z);
            yield return null;
        }

        shadow.SetActive(false);  // Dölj skuggan när bossen landat

        if (slamSound != null)
            audioSource.PlayOneShot(slamSound);

        PlayRandomSlamVoiceline();

        SetCollisionStatus(true);
        OnSlam?.Invoke();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(SpawnEarthWaves());

        yield return new WaitForSeconds(1f);

        stateController.SetState(BossState.Vulnerable);
    }


    private IEnumerator ThrowAttack()
    {
        OnThrow?.Invoke();
        LookAtPlayer();
        PlayRandomThrowVoiceline(); // Spela voiceline vid kast

        yield return new WaitForSeconds(1f);

        Vector2 direction = (player.position - throwPoint.position).normalized;
        GameObject thrownObj = Instantiate(throwablePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rbObj = thrownObj.GetComponent<Rigidbody2D>();

        if (rbObj != null)
        {
            rbObj.gravityScale = 0f;
            rbObj.linearVelocity = direction * throwForce;
        }

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator SpawnEarthWaves()
    {
        for (int i = 0; i < earthWaveCount; i++)
        {
            int direction = (i % 2 == 0) ? 1 : -1;
            int distanceIndex = (i + 1) / 2;
            float xOffset = distanceIndex * earthWaveSpacing * direction;

            Vector3 spawnPosition = earthWavePoint.position + new Vector3(xOffset, -startDistanceDown, 0);
            GameObject earthWave = Instantiate(earthWavePrefab, spawnPosition, Quaternion.identity);

            earthWave.transform.DOMoveY(earthWave.transform.position.y + distanceUp, earthWaveSpeed).SetEase(Ease.InOutSine);
            StartCoroutine(DisappearEarthWave(earthWave));

            yield return new WaitForSeconds(delayBetweenWaves);
        }
    }

    private IEnumerator DisappearEarthWave(GameObject earthWave)
    {
        yield return new WaitForSeconds(disappearDelay);

        earthWave.transform.DOMoveY(earthWave.transform.position.y - startDistanceDown, 1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(earthWave);
        });
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 5f, groundLayer);
        return hit.collider != null;
    }

    private void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        scale.x = player.position.x > transform.position.x
            ? -Mathf.Abs(scale.x)
            : Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    private void SpawnPlatforms()
    {/*
        float centerX = transform.position.x;
        float y = transform.position.y - 1f; // Lite under bossens mitt

        for (int i = 0; i < platformCount; i++)
        {
            float offset = (i - (platformCount - 1) / 2f) * platformSpacing;

            if (Mathf.Abs(offset) < 0.1f) continue;

            Vector3 spawnPosition = new Vector3(centerX + offset, y, 0);
            GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            SpriteRenderer sr = platform.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                Color startColor = sr.color;
                startColor.a = 0;
                sr.color = startColor;

                sr.DOFade(1f, platformFadeDuration);
                StartCoroutine(FadeOutAndDestroy(platform, sr));
            }
            else
            {
                Debug.LogWarning("Spawned platform has no SpriteRenderer!");
                Destroy(platform, platformLifeTime);
            }
        }*/
    }

    private IEnumerator FadeOutAndDestroy(GameObject obj, SpriteRenderer sr)
    {
        yield return new WaitForSeconds(platformLifeTime - platformFadeDuration);
        sr.DOFade(0f, platformFadeDuration).OnComplete(() =>
        {
            Destroy(obj);
        });
    }

    // VOICELINE-METODER
    private void PlayRandomSlamVoiceline()
    {
        PlayRandomVoicelineFromList(slamVoicelines);
    }

    private void PlayRandomJumpVoiceline()
    {
        PlayRandomVoicelineFromList(jumpVoicelines);
    }

    private void PlayRandomThrowVoiceline()
    {
        PlayRandomVoicelineFromList(throwVoicelines);
    }

    private void PlayRandomTauntVoiceline()
    {
        PlayRandomVoicelineFromList(tauntVoicelines);
    }

    private void PlayRandomVoicelineFromList(List<LocalizedAudioClip> voicelines)
    {
        if (voicelines == null || voicelines.Count == 0)
        {
            return;
        }

        LocalizedAudioClip clipToPlay = voicelines[UnityEngine.Random.Range(0, voicelines.Count)];
        var handle = clipToPlay.LoadAssetAsync();
        handle.Completed += (AsyncOperationHandle<AudioClip> op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded && op.Result != null)
            {
                voiceAudioSource.PlayOneShot(op.Result);
            }
            else
            {
                Debug.LogError($"Failed to load voiceline from {clipToPlay}");
            }
        };
    }
}