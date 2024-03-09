using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] AudioClip selectButton;
    public void StartGameFunc()
    {
        Manager.Sound.PlaySFX(selectButton);
        Manager.Scene.LoadScene("GameScene");
        Manager.Sound.StopBGM();
    }

    public void Option()
    {
        Manager.Sound.PlaySFX(selectButton);
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


}
