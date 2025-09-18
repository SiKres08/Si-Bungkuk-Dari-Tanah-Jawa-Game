using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class enemyPatrol : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform currentPoint;
    Damageable damageable;
    private Vector2 knockbackVelocity = Vector2.zero;
    private float knockbackDuration = 0f;
    private float knockbackTime = 0f;
    public float speed;
    public float idleTime = 2f; // Waktu idle di setiap titik
    private bool isIdle = false; // Apakah musuh sedang idle
    public DetectionZone attackZone;

    public bool _hasTarget = false;
    public bool HasTarget { get { return _hasTarget; } private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value);
        }
    }

    public bool CanMove
    {
        get
        {
            return attackZone.detectedColliders.Count == 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPoint = pointB.transform;
        animator.SetBool("isRunning", true);
        damageable = GetComponent<Damageable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isIdle)
        {
            rb.velocity = Vector2.zero; // Berhenti saat idle
            HasTarget = attackZone.detectedColliders.Count > 0;
            if (HasTarget)
            {
                AttackPlayer();
            }
            return;
        }

        Vector2 point = currentPoint.position - transform.position;
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            rb.velocity = Vector2.zero; // Hentikan kecepatan segera
            StartCoroutine(SetIdle(currentPoint == pointB.transform ? pointA.transform : pointB.transform));
            return;
        }

        // Mengatur arah pergerakan sesuai titik tujuan
        if (currentPoint == pointB.transform)
        {
            if (transform.localScale.x < 0) flip(); // Pastikan menghadap kanan saat menuju pointB
            rb.velocity = new Vector2(speed, 0); 
        }
        else
        {
            if (transform.localScale.x > 0) flip(); // Pastikan menghadap kiri saat menuju pointA
            rb.velocity = new Vector2(-speed, 0); 
        }

        HasTarget = attackZone.detectedColliders.Count > 0; 
        if (HasTarget)
        {
            AttackPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (knockbackTime > Time.time)
        {
            // Terapkan knockback
            rb.velocity = knockbackVelocity;
        }
        else
        {
            knockbackVelocity = Vector2.zero;
            if (isIdle || !CanMove)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                return;
            }

            // Pergerakan patroli sudah diatur di Update()
        }
    }

    private IEnumerator SetIdle(Transform nextPoint)
    {
        isIdle = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        
        yield return new WaitForSeconds(idleTime); // Tunggu selama idleTime

        flip();
        animator.SetBool("isRunning", true);
        rb.bodyType = RigidbodyType2D.Dynamic;
        currentPoint = nextPoint;
        isIdle = false;
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Membalikkan sumbu X
        transform.localScale = localScale;
    }

    private void AttackPlayer()
    {
        // Menghadap ke pemain
        Vector2 directionToPlayer = (attackZone.detectedColliders[0].transform.position - transform.position).normalized;
        if (directionToPlayer.x > 0 && transform.localScale.x < 0)
        {
            flip();  // Membalikkan musuh jika perlu
        }
        else if (directionToPlayer.x < 0 && transform.localScale.x > 0)
        {
            flip();  // Membalikkan musuh jika perlu
        }

        // Menyerang jika dalam jarak tertentu (misalnya, dalam zona serangan)
        if (Vector2.Distance(transform.position, attackZone.detectedColliders[0].transform.position) < 2f) 
        {
            // Panggil fungsi animasi serang
            animator.SetTrigger("attack");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        knockbackVelocity = knockback;
        knockbackDuration = 0.5f; // Durasi knockback dalam detik
        knockbackTime = Time.time + knockbackDuration;
    }
}
