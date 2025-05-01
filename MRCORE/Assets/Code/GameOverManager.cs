using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Conditions")]
    public int maxAnimalsLeft = 15;

    [Header("Animal Tracking")]
    public string animalTag = "Animal"; //  make sure all the animals have the "Animal" tag

    [Header("UI")]
    public GameObject gameOverScreen;
    public Button retryButton;

    private bool gameEnded = false;

    void Start()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        if (retryButton != null)
            retryButton.onClick.AddListener(RestartLevel);
    }

    void Update()
    {
        if (gameEnded) return;

        int animalsRemaining = GameObject.FindGameObjectsWithTag(animalTag).Length;

        if (animalsRemaining > maxAnimalsLeft)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        gameEnded = true;
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Time.timeScale = 0f; // Optional: pause game
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
