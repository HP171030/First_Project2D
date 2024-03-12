using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionUI : PopUpUI
{
    
    [SerializeField] Toggle Toggle;

    protected override void Awake()
    {
        base.Awake();
        GetUI<Button>("Close").onClick.AddListener(Close);
        GetUI<Toggle>("Toggle").onValueChanged.AddListener(ToggleOn);
        
    }
    public void ToggleOn(bool check)
    {
       
            Screen.fullScreen = check;
        Debug.Log(Screen.fullScreen);
        Debug.Log(check);
        
    }

}



