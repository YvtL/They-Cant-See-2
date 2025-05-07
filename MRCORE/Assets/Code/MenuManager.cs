using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement; // Added for easier scene management

public class MenuManager : MonoBehaviour
{
    public int rayCastDistance = 35; // Changed to public for inspector editing
    public LayerMask UILayer;
    private Transform pointer;
    private Transform target;
    private bool isTransitioning = false; // Prevent multiple scene loads
    private float cooldownTimer = 0f;
    private float cooldownDuration = 0.5f; // Prevents multiple clicks

    void Start()
    {
        pointer = GameObject.FindGameObjectWithTag("Pointer").transform;
        if (pointer == null)
        {
            Debug.LogError("No GameObject with tag 'Pointer' found!");
        }
    }

    void Update()
    {
        // If already transitioning or in cooldown, skip
        if (isTransitioning || cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        InputDevice controller = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);

        if (trigger)
        {
            // Fixed raycast by passing distance parameter correctly
            if (Physics.Raycast(pointer.position, pointer.forward, out RaycastHit hit, rayCastDistance, UILayer))
            {
                target = hit.transform;
                Debug.Log("Hit object: " + target.name); // Debug log

                // Trigger the button feedback
                target.SendMessage("ClickAction", SendMessageOptions.DontRequireReceiver);

                // Cooldown to prevent multiple triggers
                cooldownTimer = cooldownDuration;

                // Check for scene transition based on actual button names in your scene
                if (target.name == "START") // Match your actual button name
                {
                    Debug.Log("Loading next scene");
                    LoadNextScene();
                }
                else if (target.name == "EXIT") // Match your actual button name
                {
                    Debug.Log("Quitting application");
                    QuitGame();
                }
                else if (target.name == "CREDIT") // Match your actual button name
                {
                    Debug.Log("Loading credit scene");
                    LoadScene("Vidhi CreditPage");
                }
                else if (target.name == "Button4") // Keep this if you have a Button4
                {
                    Debug.Log("Loading main menu");
                    LoadScene("Vidhi_MainMenu");
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(pointer.position, pointer.forward, out RaycastHit hit, rayCastDistance, UILayer))
        {
            Transform newTarget = hit.transform;

            // Only send hover message if target changes
            if (target != newTarget)
            {
                // Stop hovering on old target
                if (target != null)
                {
                    target.SendMessage("HoverAction", false, SendMessageOptions.DontRequireReceiver);
                }

                // Start hovering on new target
                target = newTarget;
                target.SendMessage("HoverAction", true, SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (target != null)
        {
            // Stop hovering when no longer pointing at a target
            target.SendMessage("HoverAction", false, SendMessageOptions.DontRequireReceiver);
            target = null;
        }
    }

    // Helper methods for scene management with error handling
    void LoadNextScene()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check if the next scene exists in build settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Loading scene index: " + nextSceneIndex);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogError("Scene index " + nextSceneIndex + " does not exist in build settings!");
            isTransitioning = false;
        }
    }

    void LoadScene(string sceneName)
    {
        if (isTransitioning) return;

        isTransitioning = true;
        Debug.Log("Loading scene: " + sceneName);

        // Try-catch to handle scene loading errors
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load scene: " + sceneName + " - " + e.Message);
            isTransitioning = false;
        }
    }

    void QuitGame()
    {
        Debug.Log("Quitting application");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}