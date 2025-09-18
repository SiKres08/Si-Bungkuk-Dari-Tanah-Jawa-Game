using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 100f;
    public float jumpImpulse = 100f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public AudioClip moveAudioClip;  // Audio di darat
    public AudioClip swimmingAudioClip; // Audio saat berenang
    private AudioSource audioSource;

    [SerializeField] private float fadeDuration = 1f; // Durasi fade in/out
    [SerializeField] private float moveAudioVolume = 0.5f; // Volume untuk moveAudioClip
    [SerializeField] private float swimmingAudioVolume = 0.5f; // Volume untuk swimmingAudioClip
    [SerializeField] private LayerMask wallLayer; // Layer untuk mendeteksi wall
    private Vector2 wallCheckDirection => transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    [SerializeField] private GameObject deathPanel;

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove");
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool("isAlive");
        }
    }

    Rigidbody2D rb;
    Animator animator;

    [SerializeField] private LayerMask waterLayer; // Layer Water

    private bool IsInWater()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Water"))
            {
                Debug.Log($"In Water: {collider.gameObject.name}");
                return true;
            }
        }
        Debug.Log("Not in Water");
        return false;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = true;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if (IsAlive && CanMove && !damageable.LockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * walkSpeed * Time.fixedDeltaTime, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        animator.SetFloat("yVelocity", rb.velocity.y);

        // Cek apakah pemain bergerak atau tidak
        if (IsMoving && touchingDirections.IsGrounded)
        {
            if (IsInWater())
            {
                if (audioSource.clip != swimmingAudioClip || !audioSource.isPlaying)
                {
                    StartCoroutine(FadeOutAndChangeClip(swimmingAudioClip, swimmingAudioVolume));
                }
            }
            else
            {
                if (audioSource.clip != moveAudioClip || !audioSource.isPlaying)
                {
                    StartCoroutine(FadeOutAndChangeClip(moveAudioClip, moveAudioVolume));
                }
            }
        }
        else if (!touchingDirections.IsGrounded) // Pastikan audio berhenti jika pemain tidak di tanah
        {
            if (audioSource.isPlaying)
            {
                StartCoroutine(FadeOut(audioSource));
            }
        }
        else // Saat pemain tidak bergerak
        {
            if (audioSource.isPlaying)
            {
                StartCoroutine(FadeOut(audioSource));
            }
        }
        CheckWallCollision();
    }

    private void Update()
    {
        // Cek apakah pemain sudah mati
        if (!IsAlive)
        {
            // Tampilkan panel kematian
            if (deathPanel != null && !deathPanel.activeSelf)
            {
                deathPanel.SetActive(true);
            }
        }
    }

    private void CheckWallCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, wallCheckDirection, 0.5f, wallLayer);
        if (hit.collider != null)
        {
            Debug.Log("Player hit the wall: " + hit.collider.name);
            rb.velocity = new Vector2(0, Mathf.Min(rb.velocity.y, 0)); // Set kecepatan horizontal ke 0, dan hanya jatuh
        }
    }

    private IEnumerator FadeOutAndChangeClip(AudioClip newClip, float volume)
    {
        yield return FadeOut(audioSource);
        
        // Setel volume sebelum memulai audio baru
        audioSource.clip = newClip;
        audioSource.volume = volume;
        
        // Mulai audio setelah volume diatur
        audioSource.Play();
        
        // Lakukan fade in setelah audio mulai diputar
        StartCoroutine(FadeIn(audioSource));
    }

    private IEnumerator FadeOut(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();
    }

    private IEnumerator FadeIn(AudioSource audioSource)
    {
        float startVolume = 0;
        audioSource.volume = startVolume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 1, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 1;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger("attack");
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        damageable.LockVelocity = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
