using System.Collections;
using UnityEngine;

public class Genderuwo : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Damageable damageable;
    private Vector2 knockbackVelocity = Vector2.zero;
    private float knockbackDuration = 0f;
    private float knockbackTime = 0f;
    public float speed;
    public float runSpeed;
    public float idleTime = 2f;
    private bool isIdle = false;
    public DetectionZone attackZone;
    public DetectionZone runAttackZone;

    private float attackCooldownTime = 0f;
    public float attackCooldown = 1f;
    private float runAttackCooldownTime = 0f;
    public float runAttackCooldown = 3f;

    private bool isRunAttacking = false;
    private Vector2 runAttackDirection;

    public float runAttackDuration = 2f;
    private float runAttackEndTime;

    public bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
        set
        {
            isAlive = value;
            animator.SetBool("isAlive", value);
            if (!isAlive)
            {
                rb.velocity = Vector2.zero;
                HasTarget = false;
                StopRunAttack();
                isRunAttacking = false;
                animator.SetBool("isRunning", false);
                HasTarget = false;
                attackZone.detectedColliders.Clear();
                runAttackZone.detectedColliders.Clear();

                // Disable all walls when Genderuwo dies
                DisableWalls();
            }
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    void Update()
    {
        if (!isAlive)
        {
            return;
        }

        if (isIdle)
        {
            rb.velocity = Vector2.zero;
            HasTarget = attackZone.detectedColliders.Count > 0;
            if (HasTarget && !isRunAttacking && Time.time >= attackCooldownTime)
            {
                AttackPlayer();
            }
            return;
        }

        HasTarget = attackZone.detectedColliders.Count > 0 && !isRunAttacking;
        bool hasRunAttackTarget = runAttackZone.detectedColliders.Count > 0;

        if (hasRunAttackTarget && Time.time >= runAttackCooldownTime)
        {
            PrepareRunAttack();
        }
        else if (!isRunAttacking && HasTarget && Time.time >= attackCooldownTime)
        {
            AttackPlayer();
        }

        if (isRunAttacking && Time.time >= runAttackEndTime)
        {
            StopRunAttack();
        }
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            HasTarget = false;
            attackZone.detectedColliders.Clear();
            runAttackZone.detectedColliders.Clear();
            return;
        }

        if (knockbackTime > Time.time)
        {
            rb.velocity = knockbackVelocity;
        }
        else if (isRunAttacking)
        {
            rb.velocity = new Vector2(runAttackDirection.x * runSpeed, 0f);
            if (runAttackDirection.magnitude < 0.1f)
            {
                runAttackDirection = new Vector2(Random.Range(-1f, 1f), 0f).normalized;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void AttackPlayer()
    {
        Vector2 directionToPlayer = (attackZone.detectedColliders[0].transform.position - transform.position).normalized;
        FlipToFaceDirection(directionToPlayer);

        if (Vector2.Distance(transform.position, attackZone.detectedColliders[0].transform.position) < 2f)
        {
            animator.SetTrigger("attack");
            attackCooldownTime = Time.time + attackCooldown;
        }
    }

    private void PrepareRunAttack()
    {
        if (!isAlive) return;
        if (runAttackZone.detectedColliders.Count == 0) return;

        Collider2D target = null;
        foreach (var collider in runAttackZone.detectedColliders)
        {
            if (collider.CompareTag("Player"))
            {
                target = collider;
                break;
            }
        }

        if (target != null)
        {
            runAttackDirection = (target.transform.position - transform.position).normalized;

            if (runAttackDirection == Vector2.zero)
            {
                runAttackDirection = Vector2.right;
            }

            FlipToFaceDirection(runAttackDirection);

            isRunAttacking = true;
            HasTarget = false;
            animator.SetBool("isRunning", true);
            runAttackCooldownTime = Time.time + runAttackCooldown;
            runAttackEndTime = Time.time + runAttackDuration;
        }

        if (runAttackDirection.magnitude < 0.1f)
        {
            runAttackDirection = new Vector2(1f, 0f).normalized;
        }
    }

    private void FlipToFaceDirection(Vector2 direction)
    {
        if (direction.x > 0 && transform.localScale.x < 0)
        {
            flip();
        }
        else if (direction.x < 0 && transform.localScale.x > 0)
        {
            flip();
        }
    }

    private void StopRunAttack()
    {
        if (!isAlive) return;
        isRunAttacking = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive && !isRunAttacking)
        {
            Debug.Log("Genderuwo mati, tidak dapat bergerak atau menyerang.");
            return;
        }

        if (isRunAttacking && collision.collider.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.collider.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.collider.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * 500f);
            }

            StopRunAttack();
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        knockbackVelocity = knockback;
        knockbackDuration = 0.5f;
        knockbackTime = Time.time + knockbackDuration;
    }

    private void DisableWalls()
    {
        // Cari semua GameObject dengan layer "Wall"
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        foreach (GameObject wall in walls)
        {
            wall.SetActive(false); // Nonaktifkan objek Wall
        }
    }
}
