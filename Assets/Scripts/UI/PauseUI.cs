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
        GetUI<Button>("Save").onClick.AddListener(Save);
        GetUI<Button>("Title").onClick.AddListener(Title);
        GetUI<Button>("Close").onClick.AddListener(Close);
    }
    public void Option()
    {
        Manager.UI.ShowPopUpUI(optionUI);

    }
    public void Title()
    {
        Time.timeScale = 1f;
        Manager.UI.ClearPopUpUI();
        Manager.Scene.LoadScene("TitleScene");
    }
    public void Save()
    {

    }
 
}
