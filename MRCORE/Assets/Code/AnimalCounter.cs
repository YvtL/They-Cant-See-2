using UnityEngine;
using TMPro;

public class AnimalCounter : MonoBehaviour
{
    public string animalTag = "Animal";
    public TextMeshProUGUI animalCounterText;

    void Update()
    {
        int animalCount = GameObject.FindGameObjectsWithTag(animalTag).Length;
        animalCounterText.text = $"Animals Left: {animalCount}";
    }
}
