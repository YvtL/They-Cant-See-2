using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRPaint : MonoBehaviour
{
    public float lineWidth = 0.02f; // Exposed to Inspector for adjusting thickness
    public Material lineMaterial;
    public Transform rightHandController;

    private LineRenderer currentLineRenderer;
    private List<Vector3> linePoints = new List<Vector3>();
    private bool isDrawing = false;

    private Color currentColor = Color.blue;
    private ColorPicker currentColorTarget;

    void Update()
    {
        if (rightHandController == null) return;

        // Move this GameObject (with collider) to follow the controller
        transform.position = rightHandController.position;
        transform.rotation = rightHandController.rotation;

        var inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // === Draw line with Trigger ===
        if (inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
        {
            if (triggerValue)
            {
                if (!isDrawing)
                {
                    StartNewLine();
                }

                AddPointToLine(rightHandController.position);
            }
            else if (isDrawing)
            {
                EndLine();
            }
        }

        // === Pick color with A button ===
        if (inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
        {
            if (currentColorTarget != null)
            {
                currentColor = currentColorTarget.GetColor();
                Debug.Log("Picked color: " + currentColor);
            }
        }

        // Update Emission Color and Texture Offset for glow effect
        if (currentLineRenderer != null)
        {
            // Apply glowing effect
            currentLineRenderer.material.SetColor("_EmissionColor", currentColor * Mathf.PingPong(Time.time, 1)); // Glow effect

            // Apply moving texture offset
            float offset = Time.time * 2f;
            currentLineRenderer.material.mainTextureOffset = new Vector2(offset, 0);
        }
    }

    void StartNewLine()
    {
        GameObject lineObject = new GameObject("VR Paint Line");
        currentLineRenderer = lineObject.AddComponent<LineRenderer>();

        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.material = new Material(lineMaterial); // Create new material instance
        currentLineRenderer.material.color = currentColor;

        // Enable Emission
        currentLineRenderer.material.EnableKeyword("_EMISSION");
        currentLineRenderer.material.SetColor("_EmissionColor", currentColor); // Start with emission

        linePoints.Clear();
        isDrawing = true;
    }

    void AddPointToLine(Vector3 newPoint)
    {
        linePoints.Add(newPoint);
        currentLineRenderer.positionCount = linePoints.Count;
        currentLineRenderer.SetPositions(linePoints.ToArray());
    }

    void EndLine()
    {
        isDrawing = false;
        currentLineRenderer = null;
    }

    // === Detect when controller touches a color cube ===
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ColorPicker colorObj))
        {
            currentColorTarget = colorObj;
            Debug.Log("Touching color: " + colorObj.GetColor());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ColorPicker colorObj) && currentColorTarget == colorObj)
        {
            Debug.Log("Stopped touching color: " + colorObj.GetColor());
            currentColorTarget = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.03f);
    }
}
