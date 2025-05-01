using UnityEngine;
//using TMPro;

public class MakeRift : MonoBehaviour
{
    public GameObject[] hands;
    int pointCount = 10;
    public bool riftActive = true;
    LineRenderer line;
    //public TextMeshPro text;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = pointCount;
    }

    void Update()
    {
        //text.text = hands[0].transform.position.ToString();
        if (line.enabled != riftActive) line.enabled = riftActive;

        if (!riftActive) return;

        Vector3[] points = new Vector3[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            points[i] = Vector3.Lerp(hands[0].transform.position, hands[1].transform.position, i * .111f);
        }
        line.SetPositions(points);
        line.widthMultiplier = Vector3.Distance(hands[0].transform.position, hands[1].transform.position) / 4;
    }
}
