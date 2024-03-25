using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadUi : PopUpUI
{
    protected override void Awake()
    {
        base.Awake();
        GetUI<Button>("Dead").onClick.AddListener(Dead);
    }
    public void Dead()
    {
        Time.timeScale = 1f;
        Manager.UI.ClearPopUpUI();
        Manager.Scene.LoadScene("TitleScene");

    }
}
