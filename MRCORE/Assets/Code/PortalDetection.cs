using UnityEngine;

public class Portal : MonoBehaviour
{
    private GameManager gameManager;
    public ParticleSystem portalEffect;
    
    void Start()
    {
        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("Portal: GameManager not found in scene!");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            // Animal hit the portal
            if (portalEffect != null)
            {
                portalEffect.Play(); // Visual feedback
            }
            
            // Notify GameManager that an animal was saved
            if (gameManager != null)
            {
                gameManager.AnimalSaved();
            }
            
            // Destroy the animal
            Destroy(other.gameObject);
        }
    }
}