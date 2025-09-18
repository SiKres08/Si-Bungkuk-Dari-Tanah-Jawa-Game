using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damageableDeath;
    public UnityEvent <int, int> healthChanged;
    Animator animator;

    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth
    {
        get{
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            // _health = value;
            // healthChanged?.Invoke(_health,MaxHealth);
            _health = Mathf.Max(0, value); // Pastikan nilai health tidak kurang dari 0
            healthChanged?.Invoke(_health, MaxHealth);
            
            //jika health turun hingga 0, mati
            if(_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool isInvicible = false;
    // public bool IsHit {get
    //     {
    //         return animator.GetBool("isHit");
    //     }
    //     private set
    //     {
    //         animator.SetBool("isHit", value);
    //     }
    //     }
    private float timeSinceHit = 0;
    public float invicibilityTime = 0.25f;

    public bool IsAlive 
    {
        get
        {
            return _isAlive;
        } 
        set
        {
            _isAlive = value;
            animator.SetBool("isAlive", value);
            Debug.Log("IsAlive set " + value);
        }
    }

    public bool LockVelocity { get
        {
           return animator.GetBool("lockVelocity");
        }
        set{
            animator.SetBool("lockVelocity", value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Health = MaxHealth;
    }

    private void Update()
    {
        if(isInvicible)
        {
            if(timeSinceHit > invicibilityTime)
            {
                //remove invicibility
                isInvicible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if(IsAlive && !isInvicible)
        {
            Health -= damage;
            isInvicible = true;

            animator.SetTrigger("hit");
            LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);

            return true;
        }

        return false;
    }

    public void Heal (int healthRestore) 
    {
        if(IsAlive)
        {
            Health += healthRestore;
            
            Health = Mathf.Min(Health, MaxHealth);

            CharacterEvent.characterHealed?.Invoke(gameObject, healthRestore);

        }    
    }
}
