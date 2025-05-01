using UnityEngine;

public class GateTriggerDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            Destroy(other.gameObject); // Poof! Fish gone.

            // Destroy the portal line renderer object (assumes this is part of it)
            LinePainter painter = GetComponentInParent<LinePainter>();
            if (painter != null)
            {
                Destroy(painter.gameObject);
            }

            // Destroy this trigger object (if it's separate)
            Destroy(gameObject);
        }
    }
}
