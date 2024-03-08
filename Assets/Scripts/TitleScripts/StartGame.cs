using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public void StartGameFunc()
    {
        Manager.Scene.LoadScene("GameScene");
    }

    public void Option()
    {

    }
    public void Credit()
    {

    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }


}
