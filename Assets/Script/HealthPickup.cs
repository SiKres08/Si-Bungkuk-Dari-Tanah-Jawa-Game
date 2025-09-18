using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthRestore = 50;
    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (isPickedUp) return;

        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            // Periksa apakah Health kurang dari MaxHealth
            if (damageable.Health < damageable.MaxHealth)
            {
                isPickedUp = true;

                damageable.Heal(healthRestore);

                GetComponent<Collider2D>().enabled = false;

                Destroy(gameObject);
            }
            else
            {
                // Opsional: Tambahkan logika untuk memberi tahu player bahwa Health sudah penuh
                Debug.Log("Health is already full!");
            }
        }
    }
}
