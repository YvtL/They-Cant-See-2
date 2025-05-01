using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GodBehavior : MonoBehaviour
{
    public float chargeSpeed = 5f;
    public float timeBeforeCharge = 1.5f;
    public string loseSceneName = "Losing Scene";

    private Transform playerTransform;
    private Animator animator;
    private bool isCharging = false;

    void Start()
    {
        // Find the player's camera/head
        playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isCharging && playerTransform != null)
        {
            // Move towards the player
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                chargeSpeed * Time.deltaTime
            );

            // Always face the player
            transform.LookAt(playerTransform);
        }
    }

    public void KillPlayer()
    {
        StartCoroutine(KillSequence());
    }

    private IEnumerator KillSequence()
    {
        // Wait dramatically before charging
        yield return new WaitForSeconds(timeBeforeCharge);

        // Play animation if available
        if (animator != null)
        {
            animator.SetTrigger("Charge");
        }

        // Start charging
        isCharging = true;

        // Wait until we're very close to the player
        while (playerTransform != null &&
               Vector3.Distance(transform.position, playerTransform.position) > 0.5f)
        {
            yield return null;
        }

        // Play kill animation if available
        if (animator != null)
        {
            animator.SetTrigger("Kill");
            yield return new WaitForSeconds(1.5f); // Wait for animation
        }

        // Load the lose scene
        SceneManager.LoadScene(loseSceneName);
    }
}