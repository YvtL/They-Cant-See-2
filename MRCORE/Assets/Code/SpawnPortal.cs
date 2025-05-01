using UnityEngine;
using UnityEngine.XR;

public class SpawnPortal : MonoBehaviour
{
    public XRNode handRole = XRNode.RightHand;
    public Transform handTrans;
    public GameObject portalPrefab;
    bool lastTrigger = false;

    void Update()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(handRole);
        controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);
        if (trigger && !lastTrigger)
        {
            LinePainter linePainter = Instantiate(portalPrefab, handTrans.position, Quaternion.identity).GetComponent<LinePainter>();
            linePainter.handRole = handRole;
            linePainter.handTrans = handTrans;
        }
        lastTrigger = trigger;
    }
}
