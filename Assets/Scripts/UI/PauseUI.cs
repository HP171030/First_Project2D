using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class PauseUI : PopUpUI
{

   [SerializeField] OptionUI optionUI;
    

    protected override void Awake()
    {
        base.Awake();
        GetUI<Button>("Options").onClick.AddListener(Option);
        GetUI<Button>("Close").onClick.AddListener(Close);
    }
    public void Option()
    {
        Manager.UI.ShowPopUpUI(optionUI);

    }
 
}
