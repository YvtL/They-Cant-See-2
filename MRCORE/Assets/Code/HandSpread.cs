using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.ARFoundation;
//using TMPro;

public class HandSpread : MonoBehaviour
{
    //public TextMeshPro text;
    readonly float spreadThreshold = 0.05f; // Minimum distance between fingers
    //readonly float spreadThreshold = 0.075f; 
    XRHandSubsystem handSubsystem;
    MakeRift makeRift;

    void Start()
    {
        makeRift = GetComponent<MakeRift>();
    }

    void Update()
    {
        if (handSubsystem == null)
        {
            handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
            if (handSubsystem == null) return;
        }

        makeRift.riftActive = Spread(handSubsystem.leftHand) && Spread(handSubsystem.rightHand);
    }
    bool Spread(XRHand hand)
    {
        if (!hand.isTracked)
        {
            return false;
        }
        hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose);
        hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexPose);
        hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out Pose littlePose);

        float dist1 = palmPose.InverseTransformPosition(indexPose.position).x;
        float dist2 = palmPose.InverseTransformPosition(littlePose.position).x;

        if (Mathf.Abs(dist1) + Mathf.Abs(dist2) < spreadThreshold)
        {
            return false;
        }

        return true;
    }
}