using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int totalAnimalsToSave = 5;
    public int maxAnimalsThatCanDie = 5;

    [Header("References")]
    public GameObject godPrefab; // The "god" entity that appears when you lose
    public Transform playerCamera; // Reference to the VR camera/head

    private int animalsSaved = 0;
    private int animalsDead = 0;
    private bool gameOver = false;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AnimalSaved()
    {
        if (gameOver) return;

        animalsSaved++;
        Debug.Log($"Animal saved! {animalsSaved}/{totalAnimalsToSave}");

        if (animalsSaved >= totalAnimalsToSave)
        {
            WinGame();
        }
    }

    public void AnimalDied()
    {
        if (gameOver) return;

        animalsDead++;
        Debug.Log($"Animal died! {animalsDead}/{maxAnimalsThatCanDie}");

        if (animalsDead >= maxAnimalsThatCanDie)
        {
            LoseGame();
        }
    }

    private void WinGame()
    {
        gameOver = true;
        Debug.Log("You won the game!");

        // Load win scene
        SceneManager.LoadScene("Winning Scene");
    }

    private void LoseGame()
    {
        gameOver = true;
        Debug.Log("You lost the game!");

        // Spawn the "god" in front of the player
        if (godPrefab != null && playerCamera != null)
        {
            Vector3 spawnPosition = playerCamera.position + playerCamera.forward * 3f;
            GameObject god = Instantiate(godPrefab, spawnPosition, Quaternion.identity);

            // Make the god face the player
            god.transform.LookAt(playerCamera);

            // Start the god's killing animation/sequence
            GodBehavior godBehavior = god.GetComponent<GodBehavior>();
            if (godBehavior != null)
            {
                godBehavior.KillPlayer();
            }
        }
        else
        {
            // If god or camera reference is missing, just load the lose scene
            SceneManager.LoadScene("Losing Scene");
        }
    }
}