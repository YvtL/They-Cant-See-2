using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class LinePainter : MonoBehaviour
{
    public System.Action onPortalComplete;

    public Transform handTrans;
    public XRNode handRole = XRNode.RightHand;
    readonly float minDistance = 0.01f;

    List<Vector3> points = new();
    LineRenderer line;
    MakeMesh makeMesh;
    bool lastTrigger = true;
    bool active = true;

    void Start()
    {
        makeMesh = GetComponent<MakeMesh>();
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    void Update()
    {
        if (!active) return;

        InputDevice controller = InputDevices.GetDeviceAtXRNode(handRole);
        controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);

        if (trigger)
        {
            MakeLine();
        }
        else if (!trigger && lastTrigger)
        {
            makeMesh.MakeFill(points.ToArray());
            line.loop = true;
            active = false;

            // Call the event when portal drawing is complete
            CompletePortal();
        }
        lastTrigger = trigger;
    }

    void MakeLine()
    {
        Vector3 currentPos = transform.InverseTransformPoint(handTrans.position);
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], currentPos) > minDistance)
        {
            points.Add(currentPos);
            line.positionCount = points.Count;
            line.SetPosition(points.Count - 1, currentPos);
        }
    }

    public void CompletePortal()
    {
        onPortalComplete?.Invoke();
        SpawnColliderInPortal(); // ← Call the spawn logic here
    }

    public Vector3 GetPortalCenter()
    {
        if (points.Count == 0) return transform.position;

        Vector3 sum = Vector3.zero;
        foreach (var point in points)
        {
            sum += transform.TransformPoint(point); // convert from local to world space
        }
        return sum / points.Count;
    }

    void SpawnColliderInPortal()
    {
        if (points.Count < 3) return; // can't form a surface

        Bounds bounds = GetLineBounds();

        GameObject portalCollider = GameObject.CreatePrimitive(PrimitiveType.Quad);
        portalCollider.transform.position = bounds.center;
        portalCollider.transform.rotation = transform.rotation;
        portalCollider.transform.localScale = bounds.size;

        Destroy(portalCollider.GetComponent<MeshRenderer>()); // only keep collider
        portalCollider.name = "PortalCollider";
    }

    Bounds GetLineBounds()
    {
        Vector3 first = transform.TransformPoint(points[0]);
        Bounds bounds = new Bounds(first, Vector3.zero);
        foreach (Vector3 p in points)
        {
            bounds.Encapsulate(transform.TransformPoint(p));
        }
        return bounds;
    }
}
