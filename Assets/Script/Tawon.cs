using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tawon : MonoBehaviour
{
    public float flightSpeed = 3f;
    public float wayPointReachedDistance = 0.1f;
    public DetectionZone attackZone;
    public List<Transform> waypoints;
    public AudioClip moveAudioClip;  // Audio yang ingin diputar saat bergerak
    private AudioSource audioSource;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextwaypoint;
    int waypointNum = 0;

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
            // return animator.GetBool("canMove");
            return attackZone.detectedColliders.Count == 0;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // nextwaypoint = waypoints[waypointNum];
        // // Atur clip audio yang diputar
        // audioSource.clip = moveAudioClip;
        // audioSource.loop = true;  // Loop audio agar tetap berputar
        nextwaypoint = waypoints[waypointNum];

        if (moveAudioClip != null && audioSource != null)
        {
            audioSource.clip = moveAudioClip;
            audioSource.loop = true;  // Loop audio agar tetap berputar
        }
        else
        {
            Debug.LogWarning("AudioSource atau MoveAudioClip belum diatur!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
                if (audioSource != null && moveAudioClip != null && !audioSource.isPlaying)
                {
                    StartCoroutine(FadeInAudio());
                    audioSource.Play();
                }
            }
            else
            {
                rb.velocity = Vector3.zero;

                if (audioSource != null && audioSource.isPlaying)
                {
                    StartCoroutine(FadeOutAudio());
                    audioSource.Stop();
                }
            }

        }
    }

    private void Flight() 
    {
        //fly to the next direction
        Vector2 directionToWaypoint = (nextwaypoint.position - transform.position).normalized;

        //check if we have reached the waypoint already
        float distance = Vector2.Distance(nextwaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        if(distance <= wayPointReachedDistance)
        {
            waypointNum++;
            if(waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }
        }

        nextwaypoint = waypoints[waypointNum];
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale;
        if(transform.localScale.x > 0)
        {
            //fcing right
            if(rb.velocity.x > 0)
            {
                //flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            //facing left
            if(rb.velocity.x < 0)
            {
                //flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }

        }
    }

    IEnumerator FadeOutAudio()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / 0.5f; // Fade dalam 0.5 detik
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    IEnumerator FadeInAudio()
    {
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += Time.deltaTime / 0.5f; // Fade dalam 0.5 detik
            yield return null;
        }
    }

}
