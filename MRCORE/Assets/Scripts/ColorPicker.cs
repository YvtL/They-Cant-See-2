using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] private Color color;

    public Color GetColor()
    {
        return color;
    }
}
