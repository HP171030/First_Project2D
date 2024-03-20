using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasManager : Singleton<UICanvasManager>
{
    [SerializeField] public Image dash;
    [SerializeField] public Image slash;
    [SerializeField]public Image skill1;
    [SerializeField]public Image skill2;

    [SerializeField] public Image Skill1Cool;
    [SerializeField] public Image Skill2Cool;

    [SerializeField] public Image dialogue;
    [SerializeField] public TMP_Text text;

    private void Start()
    {
        this.gameObject.SetActive(false);
        dialogue.enabled = false;
        text .enabled = false;

    }
}
