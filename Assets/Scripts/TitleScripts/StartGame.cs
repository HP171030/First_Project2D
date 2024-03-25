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
    [SerializeField] Button continueButton;

    private void Start()
    {
       
        Manager.Game.titleOption += OptionClose;
    }
    private void Update()
    {
        bool exist = Manager.Data.ExistData();
        continueButton.gameObject.SetActive(exist);

    }
    public void StartGameFunc()
    {
        Manager.Sound.PlaySFX(selectButton);
        Manager.Data.NewData();
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
        public void ContinueButton()
    {
        Manager.Sound.PlaySFX(selectButton);
        Manager.Data.LoadData();
        Manager.Scene.LoadScene(Manager.Data.GameData.sceneName);
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
