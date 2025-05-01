using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ButtonFeedback : MonoBehaviour
{
  Image buttonImg;
  public Color clickColor = Color.red;
  public Color textHoverColor = Color.cyan;
  Color textBaseColor; 
  public Sprite hoverSprite;
  Sprite baseSprite;
  TextMeshProUGUI buttonText;

    void Start()
    {
        buttonImg = GetComponent<Image>();
        buttonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textBaseColor = buttonText.color;
        baseSprite = buttonImg.sprite;
    }

    public void HoverAction(bool hovering)
    {
        if (hovering)
        {
          buttonImg.sprite = hoverSprite;
          buttonText.color = textHoverColor;
        }
        else
        {
          buttonImg.sprite = baseSprite;
          buttonText.color = textBaseColor; 
        }
    }
    public void ClickAction()
    {
        StartCoroutine(ClickFeedback());
    }

    IEnumerator ClickFeedback()
    {
        buttonImg.color = clickColor;
        yield return null;
        yield return null;
        buttonImg.color = Color.white;
    }
}
