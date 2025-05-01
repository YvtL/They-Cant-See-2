using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BotMove : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Animator animator;

    public GameObject losePanel; // Assign in Inspector
    private bool hasLost = false;
    private GameObject currentTarget;

    public Material outlineMaterial;
    private Material originalMaterial;
    private Renderer animalRenderer;    

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (hasLost) return;

        currentTarget = FindNearestAnimal();

        if (currentTarget != null)
        {
            navAgent.SetDestination(currentTarget.transform.position);
        }
    }

    GameObject FindNearestAnimal()
    {
        GameObject[] animals = GameObject.FindGameObjectsWithTag("Animal");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject animal in animals)
        {
            float dist = Vector3.Distance(transform.position, animal.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = animal;
            }
        }

        return closest;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasLost && other.CompareTag("Animal"))
        {
            hasLost = true;
            navAgent.isStopped = true;

            if (animator != null)
            {
                animator.SetTrigger("Eat"); // Ensure this trigger exists
            }

            Destroy(other.gameObject); // Animal disappears
            StartCoroutine(ShowLoseUI());
        }
    }

    IEnumerator ShowLoseUI()
    {
        yield return new WaitForSeconds(1.5f); // Animation delay
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }

    void TargetAnimal(GameObject animal)
{
    animalRenderer = animal.GetComponent<Renderer>();
    originalMaterial = animalRenderer.material;
    animalRenderer.material = outlineMaterial;
}

void ClearTarget()
{
    if (animalRenderer != null)
        animalRenderer.material = originalMaterial;
}


}
