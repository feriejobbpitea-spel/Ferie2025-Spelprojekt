using System;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject stonePrefab;
    public Transform weaponTip;
    public float minForce = 5f;
    public float maxForce = 20f;
    public float chargeRate = 10f;

    public LineRenderer trajectoryLine; // Dra in i Inspector
    public int linePoints = 30;
    public float timeStep = 0.05f;

    public Animator animator; // Dra in Animator i Inspector

    [Header("Audio")]
    public AudioClip chargeSound;
    public AudioClip shootSound;
    private AudioSource audioSource;

    private float currentForce = 0f;
    private bool isCharging = false;
    private bool isChargeSoundPlaying = false;

    private float fireCooldown = 0.5f; // Max 2 skott per sekund
    private float nextFireTime = 0f;

    void Start()
    {
        if (trajectoryLine != null)
        {
            trajectoryLine.textureMode = LineTextureMode.Tile;
            trajectoryLine.startColor = new Color(0f, 0f, 0f, 0.5f);
            trajectoryLine.endColor = new Color(0f, 0f, 0f, 0.5f);
        }

        if (animator != null)
        {
            animator.SetBool("isCharging", false);
            animator.speed = 1f;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentForce = minForce;

            if (animator != null)
            {
                animator.SetBool("isCharging", true);
                animator.speed = chargeRate / (maxForce - minForce);
            }

            if (chargeSound != null && !isChargeSoundPlaying)
            {
                audioSource.clip = chargeSound;
                audioSource.loop = true;  // Loopa laddningsljudet medan knappen hålls nere
                audioSource.Play();
                isChargeSoundPlaying = true;
            }
        }

        if (Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Shoot", KeyCode.Mouse0.ToString()))) && isCharging)
        {
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, minForce, maxForce);
            UpdateTrajectoryLine(currentForce);
        }

        if (Input.GetKeyUp((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Shoot", KeyCode.Mouse0.ToString()))) && isCharging)
        {
            if (audioSource.isPlaying && audioSource.clip == chargeSound)
            {
                audioSource.Stop();  // Stoppa laddningsljudet direkt när vi släpper knappen
                audioSource.loop = false;
                isChargeSoundPlaying = false;
            }

            if (Time.time >= nextFireTime)
            {
                Shoot(currentForce);
                nextFireTime = Time.time + fireCooldown;
            }

            if (animator != null)
            {
                animator.SetBool("isCharging", false);
                animator.speed = 1f;
            }

            isCharging = false;
            trajectoryLine.positionCount = 0;
        }
    }

    void Shoot(float force)
    {
        GameObject newStone = Instantiate(stonePrefab, weaponTip.position, Quaternion.identity);
        Rigidbody2D rb = newStone.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 shootDirection = weaponTip.right;
            rb.linearVelocity = shootDirection * force;
        }

        if (shootSound != null)
            audioSource.PlayOneShot(shootSound);
    }

    void UpdateTrajectoryLine(float force)
    {
        Camera cam = Camera.main;
        Vector3 screenBottomLeft = cam.ScreenToWorldPoint(Vector3.zero);
        Vector3 screenTopRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        Vector2 startPos = weaponTip.position;
        Vector2 startVel = weaponTip.right * force;

        trajectoryLine.positionCount = linePoints;

        for (int i = 0; i < linePoints; i++)
        {
            float t = i * timeStep;
            Vector2 point = startPos + startVel * t + 0.5f * Physics2D.gravity * t * t;

            point.x = Mathf.Clamp(point.x, screenBottomLeft.x, screenTopRight.x);
            point.y = Mathf.Clamp(point.y, screenBottomLeft.y, screenTopRight.y);

            trajectoryLine.SetPosition(i, point);
        }

        if (trajectoryLine.material != null)
            trajectoryLine.material.mainTextureScale = new Vector2(force, 1);
    }
}
