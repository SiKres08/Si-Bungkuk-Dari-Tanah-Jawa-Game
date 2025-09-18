using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private Rigidbody2D rb;
    private Damageable damageable;
    private int groundLayer;
    private int treeLayer;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask whatIsGround;

    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private bool isGrounded;
    private bool hasPlayedDeathSound = false;
    // private bool hasPlayedHitSound = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        audioSource = GetComponent<AudioSource>();

        // Cache layer indices for performance
        groundLayer = LayerMask.NameToLayer("Ground");
        treeLayer = LayerMask.NameToLayer("Tree");

        // Ensure the tree starts in the correct state
        UpdateTreeState();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the tree is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        // Continuously check and update tree state based on isAlive and isGrounded
        UpdateTreeState();
    }

    private void UpdateTreeState()
    {
        if (damageable.IsAlive)
        {
            // Tree is alive: gravity scale is 0 and layer is "Tree"
            rb.gravityScale = 0;
            gameObject.layer = treeLayer;

            // Reset death sound flag
            hasPlayedDeathSound = false;
        }
        else
        {
            // Tree is dead: check if it is grounded
            rb.gravityScale = 2;

            if (isGrounded)
            {
                // Set layer to "Ground" only when grounded
                gameObject.layer = groundLayer;
            }

            // Play death sound once
            if (!hasPlayedDeathSound && deathSound != null)
            {
                Debug.Log("Playing death sound");
                audioSource.clip = deathSound; // Set the clip to death sound
                audioSource.Play();           // Play the clip
                hasPlayedDeathSound = true;
            }
        }
    }

    public void OnHit()
    {
        Debug.Log("OnHit called");
        // Play hit sound
        if (hitSound != null)
        {
            audioSource.clip = hitSound; // Set the clip to hit sound
            audioSource.Play();         // Play the clip
        }
        else
        {
            Debug.Log("Hit sound is null!");
        }
    }


    private void OnDrawGizmos()
    {
        // Draw ground check radius in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
