using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Tuyul : MonoBehaviour
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
            return animator.GetBool("canMove");
            // return attackZone.detectedColliders.Count == 0;
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
            rb.velocity = UnityEngine.Vector2.zero; // Berhenti saat idle
            HasTarget = attackZone.detectedColliders.Count > 0; 
            return;
        }

        UnityEngine.Vector2 point = currentPoint.position - transform.position;
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            rb.velocity = Vector2.zero; // Hentikan kecepatan segera
            StartCoroutine(SetIdle(currentPoint == pointB.transform ? pointA.transform : pointB.transform));
            return;
        }
        rb.velocity = new Vector2((currentPoint == pointB.transform ? 1 : -1) * speed, 0);

        HasTarget = attackZone.detectedColliders.Count > 0; 
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
        // Selesaikan knockback dan lanjutkan pergerakan patroli
        knockbackVelocity = Vector2.zero;
        if (isIdle || !CanMove)
        {
            rb.velocity = Vector2.zero; // Pastikan berhenti total di FixedUpdate
            rb.angularVelocity = 0f; // Hentikan juga rotasi
            return;
        }

        // Pergerakan patroli
        Vector2 point = currentPoint.position - transform.position;
        if (currentPoint == pointB.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        }
    }
    }

    private IEnumerator SetIdle(Transform nextPoint)
    {
        isIdle = true;
        rb.velocity = Vector2.zero; // Pastikan musuh berhenti
        animator.SetBool("isRunning", false); // Set animasi idle
        
        yield return new WaitForSeconds(idleTime); // Tunggu selama idleTime

        flip();
        animator.SetBool("isRunning", true); // Kembali ke animasi berjalan
         rb.bodyType = RigidbodyType2D.Dynamic;
        currentPoint = nextPoint; // Set tujuan berikutnya
        isIdle = false;
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        // rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        knockbackVelocity = knockback;
        knockbackDuration = 0.5f; // Durasi knockback dalam detik
        knockbackTime = Time.time + knockbackDuration;
    }
}
