using UnityEngine;
using UnityEngine.XR;

public class PortalForAnimal : MonoBehaviour
{
    public XRNode handRole = XRNode.RightHand;
    public Transform handTrans;
    public GameObject portalPrefab;
    public GameObject meshColliderPrefab; // Assign a prefab with MeshCollider in inspector
    bool lastTrigger = false;

    void Update()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(handRole);
        controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);
        if (trigger && !lastTrigger)
        {
            // Spawn portal
            LinePainter linePainter = Instantiate(portalPrefab, handTrans.position, Quaternion.identity).GetComponent<LinePainter>();
            linePainter.handRole = handRole;
            linePainter.handTrans = handTrans;

            // Hook up event to spawn collider when the portal is completed
            linePainter.onPortalComplete = () =>
            {
                Vector3 center = linePainter.GetPortalCenter();
                Quaternion rotation = Quaternion.LookRotation(linePainter.transform.forward); // align with portal
                Instantiate(meshColliderPrefab, center, rotation);
            };
        }
        lastTrigger = trigger;
    }
}
