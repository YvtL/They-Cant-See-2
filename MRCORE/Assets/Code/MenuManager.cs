using UnityEngine;
using UnityEngine.XR;
public class MenuManager : MonoBehaviour
{
    int rayCastDistance = 35;
    public LayerMask UILayer;
    Transform pointer;
    Transform target;
    void Start()
    {
        pointer = GameObject.FindGameObjectWithTag("Pointer").transform;
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger);
        if (trigger)
        {
            if(Physics.Raycast(pointer.position , pointer.forward, out RaycastHit hit, UILayer))
            {
               target = hit.transform;
               target.SendMessage("ClickAction");
               if (target.name == "Button1")
               {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
               }
               else if (target.name == "Button2")
               {
                Application.Quit();
               }
            }
        }
    }
    void FixedUpdate()
{
    if (Physics.Raycast(pointer.position, pointer.forward, out RaycastHit hit, rayCastDistance, UILayer))
    {
        target = hit.transform;
        target.SendMessage("HoverAction", true);
    }
    else if (target != null)
    {
        target.SendMessage("HoverAction", false);
        target = null;
    }
}
}


