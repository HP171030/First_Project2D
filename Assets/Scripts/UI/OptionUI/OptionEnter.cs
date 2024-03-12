using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionEnter : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler


{
    float fontSize;
    [SerializeField] TMP_Text pointText;
    Color baseFontColor;


    private void Start()
    {
        baseFontColor = pointText.color;
        fontSize = pointText.fontSize;
    }
    public void OnPointerEnter( PointerEventData eventData )
    {
        Debug.Log("Enter");
        pointText.color = Color.red;
        pointText.fontSize = 65f;
    }

    public void OnPointerExit( PointerEventData eventData )
    {

        pointText.color = baseFontColor;
        pointText.fontSize = fontSize;
    }
}
