using System.Collections;
using UnityEngine;

public class Ular : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone attack2Zone;

    private float attack1CooldownTime = 0f;
    public float attack1Cooldown = 1f; // Cooldown untuk serangan pertama

    private float attack2CooldownTime = 0f;
    public float attack2Cooldown = 3f; // Cooldown untuk serangan kedua

    private bool isAlive = true;
    private bool isAttacking = false; // Menandai apakah sedang menyerang

    public bool IsAlive
    {
        get { return isAlive; }
        set
        {
            isAlive = value;
            animator.SetBool("isAlive", value);

            if (!isAlive)
            {
                rb.velocity = Vector2.zero; // Hentikan semua gerakan
                attackZone.detectedColliders.Clear(); // Bersihkan target di zona serang
                attack2Zone.detectedColliders.Clear(); // Bersihkan target di zona serang kedua
            }
        }
    }

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value);
        }
    }

    private bool _hasAttack2Target = false;
    public bool HasAttack2Target
    {
        get { return _hasAttack2Target; }
        private set
        {
            _hasAttack2Target = value;
            animator.SetBool("hasAttack2Target", value);
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
        if (!isAlive || isAttacking)
        {
            return;
        }

        HasTarget = attackZone.detectedColliders.Count > 0;
        HasAttack2Target = attack2Zone.detectedColliders.Count > 0;

        // Pastikan HasTarget = false selama cooldown
        if (Time.time < attack1CooldownTime)
        {
            HasTarget = false; // Menonaktifkan target selama cooldown
        }

        if (Time.time < attack2CooldownTime)
        {
            HasAttack2Target = false; // Menonaktifkan target serangan kedua selama cooldown
        }

        // Prioritaskan serangan kedua jika ada target untuk serangan kedua dan cooldown selesai
        if (HasAttack2Target && Time.time >= attack2CooldownTime)
        {
            StartCoroutine(PerformAttack("attack2", attack2Cooldown, Attack2CooldownSet));
        }
        // Lanjutkan ke serangan pertama jika serangan kedua tidak dipilih dan cooldown selesai
        else if (HasTarget && Time.time >= attack1CooldownTime)
        {
            StartCoroutine(PerformAttack("attack1", attack1Cooldown, Attack1CooldownSet));
        }
    }

    private IEnumerator PerformAttack(string attackTrigger, float cooldown, System.Action cooldownSetter)
    {
        isAttacking = true; // Menandai sedang menyerang
        animator.SetTrigger(attackTrigger);

        // Tunggu hingga animasi selesai (pastikan waktu animasi cukup)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        cooldownSetter(); // Set waktu cooldown setelah animasi selesai
        isAttacking = false; // Reset status menyerang
    }

    private void Attack1CooldownSet()
    {
        attack1CooldownTime = Time.time + attack1Cooldown; // Set cooldown untuk attack1
    }

    private void Attack2CooldownSet()
    {
        attack2CooldownTime = Time.time + attack2Cooldown; // Set cooldown untuk attack2
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive)
        {
            Debug.Log("Ular mati, tidak dapat bergerak atau menyerang.");
            return;
        }

        if (collision.collider.CompareTag("Player"))
        {
            // Tambahkan logika tabrakan dengan pemain jika diperlukan
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = knockback;
        // Implementasikan logika knockback jika diperlukan
    }
}
