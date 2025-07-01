using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float playerSpeed;
    public float jumpForce;
    public float jumpCutMultiplier;
    public Transform groundCheck;
    public float groundCheckRadius;
    public Transform wallCheckL;
    public Transform wallCheckR;
    public SpriteRenderer gfx;
    private Rigidbody2D rb;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public bool isGrounded;
    private bool wasGrounded = false; // 👈 För att detektera landning
    private float isRunning = 1;
    private bool wasGrabbingWall = false;
    private float wallJumpLockTime = 0.2f;
    private float wallJumpTimer = 0f;
    private float wallJumpXMomentum = 0.5f;
    public bool isGrabingwall = false;

    public event Action OnJump;
    private Tween fallStretchTween;

    public bool IsGrounded => isGrounded;
    public bool IsMoving => Input.GetAxisRaw("Horizontal") != 0;
    public float GetMoveSpeed => playerSpeed * isRunning * superSpeed;
    public bool facingRight = true;

    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    #region powerups
    public bool doubleJump = true;
    public bool doubleJumpUsed = false;
    public bool bigJump = false;
    public float bigJumpForce;
    public float superSpeed = 1;
    public bool timeSlow = false;
    #endregion

    private int slowCounter = 0;
    private float normalSpeed;

    [Header("Audio")]
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioClip landSound; // 👈 Nytt ljud för landning
    public AudioSource audioSource;

    private float walkSoundCooldown = 0.5f;
    private float walkTimer = 0f;

    private BoxCollider2D boxCollider;
    public float currentCharge = 1f;
    public float maxCharge = 1f;
    public float rechargeRate = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        normalSpeed = playerSpeed;

        keybinds["Jump"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Jump", KeyCode.Space.ToString()));
        keybinds["Sprint"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Sprint", KeyCode.LeftShift.ToString()));
        keybinds["Shoot"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Shoot", KeyCode.Mouse0.ToString()));
        platformEffector = GameObject.FindAnyObjectByType<PlatformEffector2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private PlatformEffector2D platformEffector;
    public float dropDuration = 0.5f;  // Hur länge man kan gå igenom plattformen

    private bool isDropping = false;
    private IEnumerator Drop()
    {
        isDropping = true;
        platformEffector.rotationalOffset = 180f; // Tillåt att gå igenom nerifrån
        yield return new WaitForSeconds(dropDuration);
        platformEffector.rotationalOffset = 0f; // Återställ effectorn till normal
        isDropping = false;
    }
    void Update()
    {
        if (!isDropping && Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Drop());
        }

        if (!IsGrounded)
            boxCollider.size = new Vector2(0.6f, 1f);

        if (currentCharge < maxCharge)
            if (!IsGrounded)
            {
                boxCollider.size = new Vector2(0.6f, 1f);
            }

        if (currentCharge < maxCharge)
        {
            // Isak tog bort för detta orsakade problem
            //currentCharge = Mathf.Min(currentCharge + rechargeRate * Time.deltaTime, maxCharge);

            ApplyFallStretch();

            if (Input.GetKey(keybinds["Sprint"]) && isGrounded)
                isRunning = 2;
            else if (isGrounded)
                isRunning = 1;
            if (Input.GetKey(keybinds["Sprint"]))
            {
                if (isGrounded) { isRunning = 2; }
            }
            else
            {
                if (isGrounded) { isRunning = 1; }
            }

            keybinds["Left"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Left", KeyCode.A.ToString()));
            keybinds["Right"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("bind_Right", KeyCode.D.ToString()));

            float moveX = 0f;
            if (Input.GetKey(keybinds["Left"])) moveX = -1f;
            if (Input.GetKey(keybinds["Right"])) moveX = 1f;

            Vector2 movement;

            if (wallJumpTimer > 0)
            {
                wallJumpTimer -= Time.deltaTime;
                movement = new Vector2(wallJumpXMomentum, rb.linearVelocity.y);
            }
            else
            {
                wallJumpXMomentum = 0;

                if (Input.GetKey(keybinds["Left"])) moveX = -1f;
                if (Input.GetKey(keybinds["Right"])) moveX = 1f;

                float targetX = moveX * playerSpeed * isRunning * superSpeed;
                float smoothedX = Mathf.Lerp(rb.linearVelocity.x, targetX, 0.1f);
                movement = new Vector2(smoothedX, rb.linearVelocity.y);
            }

            // Här är ändringen för att inte kunna hoppa om S hålls ned
            if (Input.GetKeyDown(keybinds["Jump"]) && !Input.GetKey(KeyCode.S))
            {
                if (isGrounded)
                {
                    OnJump?.Invoke();
                    PlayJumpTween();
                    if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
                    movement.y = bigJump ? bigJumpForce : jumpForce;
                }
                else if (doubleJump && !doubleJumpUsed)
                {
                    OnJump?.Invoke();
                    PlayJumpTween();
                    if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
                    movement.y = bigJump ? bigJumpForce : jumpForce;
                    doubleJumpUsed = true;
                }
                else if (isGrabingwall)
                {
                    OnJump?.Invoke();
                    PlayJumpTween();
                    if (jumpSound != null) audioSource.PlayOneShot(jumpSound);
                    wallJumpTimer = wallJumpLockTime;
                    float direction = (!facingRight) ? 1f : -1f;
                    float xForce = direction * playerSpeed * 1.5f;
                    float yForce = jumpForce * 0.8f;
                    rb.linearVelocity = new Vector2(xForce, yForce);
                    wallJumpXMomentum = xForce;
                    return;
                }
            }

            rb.linearVelocity = movement;

            if (Input.GetKeyUp(keybinds["Jump"]) && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }

            // Kolla om spelaren är på marken
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // 👇 Landningsljud (om vi just landat)
            if (isGrounded && !wasGrounded)
            {
                if (landSound != null)
                    audioSource.PlayOneShot(landSound);
            }
            wasGrounded = isGrounded;

            if (moveX < 0 && facingRight) Flip();
            else if (moveX > 0 && !facingRight) Flip();

            bool huggingLeftWall = Physics2D.OverlapCircle(wallCheckL.position, groundCheckRadius, wallLayer);
            bool huggingRightWall = Physics2D.OverlapCircle(wallCheckR.position, groundCheckRadius, wallLayer);
            isGrabingwall = ((huggingLeftWall && !facingRight) || (huggingRightWall && facingRight));
            if (isGrabingwall && !wasGrabbingWall) PlayWallImpactTween();
            wasGrabbingWall = isGrabingwall;

            if (isGrabingwall && rb.linearVelocity.y < 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
            }

            if (isGrounded)
            {
                doubleJumpUsed = false;
                ResetScale();
            }

            // 🎵 Spela gång-/springljud
            if (isGrounded && IsMoving)
            {
                walkTimer -= Time.deltaTime;
                if (walkTimer <= 0f)
                {
                    AudioClip clipToPlay = (isRunning > 1f) ? runSound : walkSound;
                    if (clipToPlay != null)
                        audioSource.PlayOneShot(clipToPlay);
                    walkTimer = walkSoundCooldown;
                }
            }

            if (Input.GetKeyDown(KeyCode.T) && timeSlow)
            {
                Time.timeScale = (Time.timeScale == 1f) ? 0.3f : 1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }

            if (isGrabingwall && Input.GetKeyDown(KeyCode.S))
            {
                isGrabingwall = false;
                gfx.flipX = false;
                facingRight = !facingRight;
            }
        }
    }


    // 🕸 Dessa två metoder behövs för spindelnätet
    public void ApplySlow()
    {
        slowCounter++;
        playerSpeed = normalSpeed * 0.5f;
    }

    public void RemoveSlow()
    {
        slowCounter--;
        if (slowCounter <= 0)
        {
            slowCounter = 0;
            playerSpeed = normalSpeed;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        gfx.flipX = !facingRight;
        PlayTurnTween();
    }

    void PlayTurnTween()
    {
        gfx.transform.DOKill();
        Sequence turnSquash = DOTween.Sequence();
        turnSquash.Append(gfx.transform.DOScaleX(1.4f, 0.05f));
        turnSquash.Join(gfx.transform.DOScaleY(0.8f, 0.05f));
        turnSquash.Append(gfx.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }

    void PlayWallImpactTween()
    {
        gfx.transform.DOKill();
        Sequence wallSquash = DOTween.Sequence();
        wallSquash.Append(gfx.transform.DOScaleX(0.7f, 0.05f));
        wallSquash.Join(gfx.transform.DOScaleY(1.3f, 0.05f));
        wallSquash.Append(gfx.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }

    void PlayJumpTween()
    {
        gfx.transform.DOKill();
        Sequence jumpSquash = DOTween.Sequence();
        jumpSquash.Append(gfx.transform.DOScaleY(0.7f, 0.05f));
        jumpSquash.Join(gfx.transform.DOScaleX(1.3f, 0.05f));
        jumpSquash.Append(gfx.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
    }

    void ResetScale()
    {
        gfx.transform.DOKill();
        gfx.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
    }

    void ApplyFallStretch()
    {
        if (rb.linearVelocity.y < -0.1f && !isGrabingwall && !isGrounded)
        {
            if (fallStretchTween == null || !fallStretchTween.IsPlaying())
            {
                fallStretchTween?.Kill();
                fallStretchTween = gfx.transform.DOScale(new Vector3(0.6f, 1.4f, 1f), 5f).SetEase(Ease.OutQuad);
            }
        }
        else if (fallStretchTween != null && fallStretchTween.IsActive() && !fallStretchTween.IsComplete())
        {
            fallStretchTween.Kill();
            fallStretchTween = gfx.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        }
    }
}