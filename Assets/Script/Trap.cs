using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Animator animator;
    public DetectionZone attackZone; // Zona deteksi target
    private bool _hasTarget = false;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool("hasTarget", value); // Mengubah parameter animator
        }
    }

    private void Awake()
    {
        // Mendapatkan komponen Animator dari GameObject
        animator = GetComponent<Animator>();

        // Validasi jika animator tidak ditemukan
        if (animator == null)
        {
            Debug.LogError("Animator not found on this GameObject.");
        }
    }

    private void Update()
    {
        // Cek apakah ada target di zona deteksi
        if (attackZone != null)
        {
            HasTarget = attackZone.detectedColliders.Count > 0;

            // Jika ada target, lakukan serangan
            if (HasTarget)
            {
                AttackPlayer();
            }
        }
        else
        {
            Debug.LogWarning("Attack Zone is not assigned.");
        }
    }

    private void AttackPlayer()
    {
        // Trigger animasi serangan
        animator.SetTrigger("attack");
    }
}
