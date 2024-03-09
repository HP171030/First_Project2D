using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TitleUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] AudioClip enterSound;
    [SerializeField] TMP_Text titleSelect;
    Color baseFontColor;
    float fontSize;


    
    private void Start()
    {
        baseFontColor = titleSelect.color;
        fontSize = titleSelect.fontSize;
    }
    public void OnPointerEnter( PointerEventData eventData )
    {
        Manager.Sound.PlaySFX( enterSound );
        titleSelect.color = Color.white;
        titleSelect.fontSize = 65f;
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        
        titleSelect.color = baseFontColor;
        titleSelect.fontSize= fontSize;
    }
}
