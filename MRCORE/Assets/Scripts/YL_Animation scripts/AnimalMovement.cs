using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AnimalMovement : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private AnimalAnimationController animController;

    public float wanderRadius = 10f;
    public float minWanderTime = 3f;
    public float maxWanderTime = 8f;
    public float fleeDistance = 10f;

    private bool isFleeing = false;
    private GameObject monster;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animController = GetComponent<AnimalAnimationController>();

        if (animController == null)
        {
            animController = gameObject.AddComponent<AnimalAnimationController>();
        }

        // Find the monster
        monster = GameObject.FindGameObjectWithTag("Monster");

        StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        if (monster != null)
        {
            float distanceToMonster = Vector3.Distance(transform.position, monster.transform.position);

            // If the monster gets too close, flee!
            if (distanceToMonster < fleeDistance && !isFleeing)
            {
                StopAllCoroutines();
                StartCoroutine(FleeFromMonster());
            }
        }
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            // Wait at current position
            float waitTime = Random.Range(minWanderTime, maxWanderTime);
            yield return new WaitForSeconds(waitTime);

            // Find a new random position to move to
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);

            // Move to the new position
            navAgent.SetDestination(hit.position);
        }
    }

    IEnumerator FleeFromMonster()
    {
        isFleeing = true;

        // Flee in the opposite direction from the monster
        Vector3 fleeDirection = transform.position - monster.transform.position;
        Vector3 fleePosition = transform.position + fleeDirection.normalized * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, 1))
        {
            navAgent.SetDestination(hit.position);
        }

        // Keep fleeing for a short time
        yield return new WaitForSeconds(3f);

        isFleeing = false;
        StartCoroutine(WanderRoutine());
    }
}