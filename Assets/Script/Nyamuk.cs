using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nyamuk : MonoBehaviour
{
    public float flightSpeed = 3f;
    public float attackSpeed = 10f;
    public float attackDuration = 1f;
    public float cooldownDuration = 10f;
    public float wayPointReachedDistance = 0.1f;
    public DetectionZone attackZone;
    public List<Transform> waypoints;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextwaypoint;
    int waypointNum = 0;

    private bool _hasTarget = false;
    private bool isAttacking = false;
    private bool isCooldown = false;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            if (!isCooldown) // Jangan aktifkan target jika sedang cooldown
            {
                _hasTarget = value;
                animator.SetBool("hasTarget", value);
            }
        }
    }

    public bool CanMove
    {
        get
        {
            return !isAttacking; // Bisa bergerak selama tidak menyerang
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        nextwaypoint = waypoints[waypointNum];
    }

    private void Update()
    {
        if (!isCooldown)
        {
            HasTarget = attackZone.detectedColliders.Count > 0;

            if (HasTarget && !isAttacking)
            {
                Transform target = attackZone.detectedColliders[0].transform;
                StartCoroutine(AttackTarget(target));
            }
        }
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else if (!isAttacking)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    private void Flight()
    {
        Vector2 directionToWaypoint = (nextwaypoint.position - transform.position).normalized;
        float distance = Vector2.Distance(nextwaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        if (distance <= wayPointReachedDistance)
        {
            waypointNum++;
            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }
        }

        nextwaypoint = waypoints[waypointNum];
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale;
        if (transform.localScale.x > 0)
        {
            if (rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }

    private void LookAtTarget(Vector2 targetPosition)
    {
        Vector3 scale = transform.localScale;

        // Periksa apakah nyamuk perlu membalik arah
        if ((targetPosition.x < transform.position.x && scale.x > 0) || 
            (targetPosition.x > transform.position.x && scale.x < 0))
        {
            scale.x *= -1; // Balik skala X untuk menghadap arah yang benar
            transform.localScale = scale;
        }
    }

    private IEnumerator AttackTarget(Transform target)
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true); // Sinkronisasi animasi

        Vector2 directionToTarget = (target.position - transform.position).normalized;
        float attackEndTime = Time.time + attackDuration;

        while (Time.time < attackEndTime)
        {
            rb.velocity = directionToTarget * attackSpeed;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        isAttacking = false;

        animator.SetBool("isAttacking", false); // Matikan animasi menyerang

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;

        // Reset target saat cooldown
        HasTarget = false;
        animator.SetBool("hasTarget", false);

        yield return new WaitForSeconds(cooldownDuration);
        isCooldown = false;
    }
}
