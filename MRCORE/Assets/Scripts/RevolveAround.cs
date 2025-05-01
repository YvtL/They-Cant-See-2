using UnityEngine;

public class RevolveAround : MonoBehaviour
{
    public Transform centerObject; // The object to revolve around
    public float rotationSpeed = 20f; // Speed of revolution
    public Vector3 rotationAxis = Vector3.up; // Axis of revolution (Y by default)

    void Update()
    {
        if (centerObject != null)
        {
            transform.RotateAround(centerObject.position, rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }
}
