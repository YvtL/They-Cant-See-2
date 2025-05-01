using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AnimalAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navAgent;
    private bool isDead = false;
    private bool isDying = false; // Added this variable that was missing

    [SerializeField]
    private float deathAnimationTime = 1f;

    // Animation parameter names - match these to your animator controller
    private const string MOVEMENT_SPEED = "Speed";
    private const string IS_DEAD = "IsDead";

    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            Debug.LogError("Animator component missing on " + gameObject.name);
        }

        if (navAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (isDead || animator == null || navAgent == null) return;

        // Update movement animation based on actual movement speed
        float speed = navAgent.velocity.magnitude;
        animator.SetFloat(MOVEMENT_SPEED, speed);
    }

    // Called when the animal is caught by the monster
    public void Die()
    {
        if (isDying) return;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        isDying = true;
        isDead = true;

        // Stop movement
        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.enabled = false; // Prevents sliding during death animation
        }

        // Trigger death animation
        if (animator != null)
        {
            animator.SetBool(IS_DEAD, true);
        }

        // Wait for animation to finish
        yield return new WaitForSeconds(deathAnimationTime);

        // We don't have GameManager.Instance implemented yet, so commenting this out
        // GameManager.Instance?.AnimalDied();

        // Destroy the object
        Destroy(gameObject);
    }
}