using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // or UnityEngine.UI for regular Text

public class WinningPortal : MonoBehaviour
{
    public int animalsNeededToWin = 5;
    private int animalsSaved = 0;

    public TextMeshProUGUI animalsSavedText; // Assign in Inspector

    private void Start()
    {
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            animalsSaved++;
            Debug.Log("Animal saved! Total: " + animalsSaved);

            Destroy(other.gameObject); // Simulate sending animal through portal

            UpdateUI();

            if (animalsSaved >= animalsNeededToWin)
            {
                SceneManager.LoadScene("Winning Scene");
            }
        }
    }

    void UpdateUI()
    {
        if (animalsSavedText != null)
        {
            animalsSavedText.text = $"Animals Saved: {animalsSaved}";
        }
    }

    void LoadWinningScene()
    {
        SceneManager.LoadScene("Winning Scene");
    }
}
