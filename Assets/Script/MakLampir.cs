using System.Collections;
using UnityEngine;

public class MakLampir : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone attack2Zone;
    public DetectionZone attack3Zone;
    public DetectionZone attack4Zone;
    public DetectionZone summonZone; // Zona untuk summon Pocong
    public GameObject pocongPrefab; // Prefab enemy Pocong
    public Transform[] summonPoints; // Array untuk posisi summon Pocong
    public Dialogue2 dialogue2;

    private float attack1CooldownTime = 0f;
    public float attack1Cooldown = 1f;

    private float attack2CooldownTime = 0f;
    public float attack2Cooldown = 3f;

    private float attack3CooldownTime = 0f;
    public float attack3Cooldown = 3f;

    private float attack4CooldownTime = 0f;
    public float attack4Cooldown = 3f;

    private bool isAlive = true;
    private bool isAttacking = false;

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
                attack2Zone.detectedColliders.Clear();
                attack3Zone.detectedColliders.Clear();
                attack4Zone.detectedColliders.Clear(); // Bersihkan target di zona serang kedua
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

    private bool _hasAttack3Target = false;
    public bool HasAttack3Target
    {
        get { return _hasAttack3Target; }
        private set
        {
            _hasAttack3Target = value;
            animator.SetBool("hasAttack3Target", value);
        }
    }

    private bool _hasAttack4Target = false;
    public bool HasAttack4Target
    {
        get { return _hasAttack4Target; }
        private set
        {
            _hasAttack4Target = value;
            animator.SetBool("hasAttack4Target", value);
        }
    }

    public class Dialogue : MonoBehaviour
    {
        public bool IsActive { get; private set; }

        public void ShowDialogue()
        {
            IsActive = true;
            // Logika untuk menampilkan dialog
        }

        public void CloseDialogue()
        {
            IsActive = false;
            // Logika untuk menutup dialog
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    // void Update()
    // {
    //     if (!isAlive || isAttacking)
    //     {
    //         return;
    //     }

    //     HasTarget = attackZone.detectedColliders.Count > 0;
    //     HasAttack2Target = attack2Zone.detectedColliders.Count > 0;
    //     HasAttack3Target = attack3Zone.detectedColliders.Count > 0;
    //     HasAttack4Target = attack4Zone.detectedColliders.Count > 0;

    //     // Pastikan HasTarget = false selama cooldown
    //     if (Time.time < attack1CooldownTime)
    //     {
    //         HasTarget = false; // Menonaktifkan target selama cooldown
    //     }

    //     if (Time.time < attack2CooldownTime)
    //     {
    //         HasAttack2Target = false; // Menonaktifkan target serangan kedua selama cooldown
    //     }

    //     if (Time.time < attack3CooldownTime)
    //     {
    //         HasAttack3Target = false; // Menonaktifkan target serangan kedua selama cooldown
    //     }

    //     if (Time.time < attack4CooldownTime)
    //     {
    //         HasAttack4Target = false; // Menonaktifkan target serangan kedua selama cooldown
    //     }

    //     // Prioritaskan serangan kedua jika ada target untuk serangan kedua dan cooldown selesai
    //     if (HasAttack4Target && Time.time >= attack4CooldownTime)
    //     {
    //         StartCoroutine(PerformAttack("attack4", attack4Cooldown, Attack4CooldownSet));
    //     }
    //     // Lanjutkan ke serangan pertama jika serangan kedua tidak dipilih dan cooldown selesai
    //     else if (HasTarget && Time.time >= attack1CooldownTime)
    //     {
    //         StartCoroutine(PerformAttack("attack1", attack1Cooldown, Attack1CooldownSet));
    //     }
    //     else if (HasAttack2Target && Time.time >= attack2CooldownTime)
    //     {
    //         StartCoroutine(PerformAttack("attack2", attack2Cooldown, Attack2CooldownSet));
    //     }
    //     else if (HasAttack3Target && Time.time >= attack3CooldownTime)
    //     {
    //         StartCoroutine(PerformAttack("attack3", attack3Cooldown, Attack3CooldownSet));
    //     }
    // }

    void Update()
    {
        if (!isAlive || isAttacking)
        {
            return;
        }

        // Cek apakah player sudah memasuki trigger dialog
        if (dialogue2 != null && !dialogue2.playerHasTriggeredDialogue)
        {
            Debug.Log("Player belum memasuki trigger dialog, Mak Lampir tidak akan menyerang.");
            return; // Jangan lanjutkan logika serangan
        }

        // Cek apakah dialog aktif
        if (dialogue2 != null && dialogue2.dialoguePanel.activeInHierarchy)
        {
            Debug.Log("Dialog aktif, menonaktifkan serangan Mak Lampir.");
            HasTarget = false;
            HasAttack2Target = false;
            HasAttack3Target = false;
            HasAttack4Target = false;
            return; // Jangan lanjutkan logika lainnya
        }

        Debug.Log("Dialog tidak aktif dan player sudah memasuki trigger, Mak Lampir dapat menyerang.");

        // Logika normal jika dialog tidak aktif
        HasTarget = attackZone.detectedColliders.Count > 0;
        HasAttack2Target = attack2Zone.detectedColliders.Count > 0;
        HasAttack3Target = attack3Zone.detectedColliders.Count > 0;
        HasAttack4Target = attack4Zone.detectedColliders.Count > 0;

        if (Time.time < attack1CooldownTime)
            HasTarget = false;

        if (Time.time < attack2CooldownTime)
            HasAttack2Target = false;

        if (Time.time < attack3CooldownTime)
            HasAttack3Target = false;

        if (Time.time < attack4CooldownTime)
            HasAttack4Target = false;

        if (HasAttack4Target && Time.time >= attack4CooldownTime)
            StartCoroutine(PerformAttack("attack4", attack4Cooldown, Attack4CooldownSet));
        else if (HasTarget && Time.time >= attack1CooldownTime)
            StartCoroutine(PerformAttack("attack1", attack1Cooldown, Attack1CooldownSet));
        else if (HasAttack2Target && Time.time >= attack2CooldownTime)
            StartCoroutine(PerformAttack("attack2", attack2Cooldown, Attack2CooldownSet));
        else if (HasAttack3Target && Time.time >= attack3CooldownTime)
            StartCoroutine(PerformAttack("attack3", attack3Cooldown, Attack3CooldownSet));
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

    private void Attack3CooldownSet()
    {
        attack3CooldownTime = Time.time + attack3Cooldown; // Set cooldown untuk attack2
    }

    private void Attack4CooldownSet()
    {
        attack4CooldownTime = Time.time + attack4Cooldown; // Set cooldown untuk attack2
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

    private IEnumerator SummonPocong()
    {
        foreach (var point in summonPoints)
        {
            Instantiate(pocongPrefab, point.position, Quaternion.identity); // Spawn Pocong
            yield return new WaitForSeconds(0.2f); // Delay antar summon
        }
    }

    

}
