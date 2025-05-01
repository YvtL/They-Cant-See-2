using UnityEngine;
using UnityEngine.UI;   // required for UI elements
using System.Collections;

public class PortalTrigger : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Drag the Panel (with CanvasGroup) here")]
    public GameObject winPanel;

    [Header("Fade Settings")]
    [Tooltip("How long (in seconds) the fade‑in should take")]
    public float fadeDuration = 1f;

    private CanvasGroup _canvasGroup;
    private bool _hasTriggered = false;

    void Awake()
    {
        if (winPanel == null)
        {
            Debug.LogError("PortalTrigger: winPanel is not assigned!", this);
            return;
        }

        // Try to get (or add) a CanvasGroup on your panel
        _canvasGroup = winPanel.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = winPanel.AddComponent<CanvasGroup>();

        // Start invisible and disabled
        _canvasGroup.alpha = 0f;
        winPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered)
            return;

        // Only react to Animals
        if (other.CompareTag("Animal"))
        {
            _hasTriggered = true;

            // Stop physics so it doesn't keep colliding
            var rb = other.attachedRigidbody;
            if (rb != null) rb.isKinematic = true;

            // Destroy the animal shortly after impact
            Destroy(other.gameObject, 0.1f);

            // Kick off the fade‑in coroutine
            StartCoroutine(FadeInWinPanel());
        }
    }

    private IEnumerator FadeInWinPanel()
    {
        // Enable the panel (it now has alpha = 0)
        winPanel.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        // Ensure fully opaque
        _canvasGroup.alpha = 1f;
    }
}
