using System.Collections;
using UnityEngine;

public class Ratu : MonoBehaviour
{
    private Animator animator;

    public DetectionZone attackZone;

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

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
        if (HasTarget)
        {
            animator.SetTrigger("QueenPointing");
        }
    }
}
