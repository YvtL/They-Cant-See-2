using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navAgent;

    // Animation parameter names - match these to your animator controller
    private const string MOVEMENT_SPEED = "Speed";
    private const string KILL_TRIGGER = "Kill";

    private bool isKilling = false;

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
        if (animator == null || navAgent == null || isKilling) return;

        // Update movement animation based on actual movement speed
        float speed = navAgent.velocity.magnitude;
        animator.SetFloat(MOVEMENT_SPEED, speed);
    }

    // Call this when monster catches an animal
    public void PlayKillAnimation(GameObject targetAnimal)
    {
        if (isKilling || animator == null) return;

        StartCoroutine(KillSequence(targetAnimal));
    }

    private IEnumerator KillSequence(GameObject targetAnimal)
    {
        isKilling = true;

        if (navAgent != null)
        {
            navAgent.isStopped = true;
        }

        // Face the animal
        if (targetAnimal != null)
        {
            Vector3 lookDirection = targetAnimal.transform.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // Trigger kill animation
        animator.SetTrigger(KILL_TRIGGER);

        // Wait for animation to play (adjust time as needed)
        yield return new WaitForSeconds(1.5f);

        // Kill the animal
        if (targetAnimal != null)
        {
            AnimalAnimationController animalController = targetAnimal.GetComponent<AnimalAnimationController>();
            if (animalController != null)
            {
                animalController.Die();
            }
            else
            {
                Destroy(targetAnimal);
            }
        }

        // Resume navmesh agent
        if (navAgent != null)
        {
            navAgent.isStopped = false;
        }

        isKilling = false;
    }
}