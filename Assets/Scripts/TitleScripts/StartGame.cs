using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    [SerializeField] AudioClip selectButton;
    [SerializeField] GameObject menu;
    [SerializeField] Image setUpMenu;
    [SerializeField] OptionUI optionUI;
    [SerializeField] GameObject title;
    private void Start()
    {
        Manager.Game.titleOption += OptionClose;
    }
    public void StartGameFunc()
    {
        Manager.Sound.PlaySFX(selectButton);
        Manager.Scene.LoadScene("GameScene");
        Manager.Sound.StopBGM();
    }

    public void Option()
    {
        Manager.Sound.PlaySFX(selectButton);
        menu.SetActive(false);
        setUpMenu.gameObject.SetActive(false);
        title.SetActive(false) ;
        Manager.Game.title = true;
       
        Manager.UI.ShowPopUpUI(optionUI);
        }
        public void Credit()
    {
        Manager.Sound.PlaySFX(selectButton);
    }
    public void Quit()
    {
        Manager.Sound.PlaySFX(selectButton);
        Application.Quit();
        Debug.Log("Quit");
    }

    public void OptionClose()
    {
        menu.SetActive(true);
        setUpMenu.gameObject.SetActive(true);
        title.SetActive(true);
    }
}
