using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CatchingAnimals : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Animator animator;
    private MonsterAnimationController animController;

    public GameObject losePanel; // Assign in Inspector
    private GameObject currentTarget;

    public Material outlineMaterial;
    private Material originalMaterial;
    private Renderer animalRenderer;

    private int animalsEaten = 0;
    public int maxAnimalsAllowedToDie = 5;

    private bool gameOver = false;

    // The distance at which the monster will catch an animal
    public float catchDistance = 1.5f;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Check if the animation controller already exists before adding
        animController = GetComponent<MonsterAnimationController>();
        if (animController == null)
        {
            animController = gameObject.AddComponent<MonsterAnimationController>();
        }
    }

    void Update()
    {
        if (gameOver) return;

        currentTarget = FindNearestAnimal();

        if (currentTarget != null)
        {
            navAgent.SetDestination(currentTarget.transform.position);

            // Check if it's close enough to catch animal
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToTarget <= catchDistance)
            {
                CatchAnimal(currentTarget);
            }
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

void CatchAnimal(GameObject animal)
{
    if (gameOver || animal == null) return;

    Debug.Log("Monster caught an animal: " + animal.name);

    // Immediately "lock" catching
    navAgent.isStopped = true;

    if (animController != null)
    {
        animController.PlayKillAnimation(animal);
    }
    else
    {
        AnimalAnimationController animalController = animal.GetComponent<AnimalAnimationController>();
        if (animalController != null)
        {
            animalController.Die();
        }
        else
        {
            Destroy(animal);
        }
    }

    animalsEaten++;

    // Very important: Only check for game over AFTER animal was processed
    if (animalsEaten >= maxAnimalsAllowedToDie)
    {
        gameOver = true;
        StartCoroutine(ShowLoseUI());
    }
    else
    {
        // Resume movement AFTER killing animation (optional small delay here if you want realism)
        StartCoroutine(ResumeChasing());
    }
}

IEnumerator ResumeChasing()
{
    yield return new WaitForSeconds(1f); // Wait a moment so monster doesn't immediately eat another
    if (!gameOver && navAgent != null)
    {
        navAgent.isStopped = false;
    }
}


    // void CatchAnimal(GameObject animal)
    // {
    //     if (gameOver || animal == null) return;

    //     Debug.Log("Monster caught an animal: " + animal.name);

    //     // Play the killing animation
    //     if (animController != null)
    //     {
    //         animController.PlayKillAnimation(animal);
    //     }
    //     else
    //     {
    //         // Fallback if animation controller is missing
    //         // Get the animal controller to play death animation
    //         AnimalAnimationController animalController = animal.GetComponent<AnimalAnimationController>();
    //         if (animalController != null)
    //         {
    //             animalController.Die();
    //         }
    //         else
    //         {
    //             // Direct destroy if no controller exists
    //             Destroy(animal);
    //         }
    //     }

    //     animalsEaten++;

    //     if (animalsEaten >= maxAnimalsAllowedToDie)
    //     {
    //         gameOver = true;
    //         if (navAgent != null)
    //         {
    //             navAgent.isStopped = true;
    //         }
    //         StartCoroutine(ShowLoseUI());
    //     }
    // }

    IEnumerator ShowLoseUI()
    {
        yield return new WaitForSeconds(1.5f); // Optional delay for animation
        SceneManager.LoadScene("Losing Scene");
    }

    void TargetAnimal(GameObject animal)
    {
        animalRenderer = animal.GetComponent<Renderer>();
        if (animalRenderer != null)
        {
            originalMaterial = animalRenderer.material;
            animalRenderer.material = outlineMaterial;
        }
    }

    void ClearTarget()
    {
        if (animalRenderer != null && originalMaterial != null)
            animalRenderer.material = originalMaterial;
    }
}