using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{



    public Transform playerPos;
    
    private int playerHP = 100;
    private int playerMP = 100;
    private int playerMaxHP = 100;
    private int playerMaxMP = 100;
    
    public event UnityAction<int> playerHPevent;
     public event UnityAction<int> playerMPevent;
    public event UnityAction<float> brightUpdate;

    public UnityAction titleOption;

    public Toggle fullScreenMode;


    bool time;
    public bool titleOff = true;
    public bool title;


    [SerializeField]PauseUI pauseUI;

    private float brightnessVol;

   
    public void OnOption( InputValue value )
    {
        if ( !titleOff&&value.isPressed )
            if ( !time )
            {
                Manager.UI.ShowPopUpUI(pauseUI);
                time = true;
            }
            else
            {

                Manager.UI.ClearPopUpUI();
                time = false;
            }

    }


    
    public int HpEvent {  get { return playerHP; }set { playerHP = Mathf.Clamp(value, 0, playerMaxMP); playerHPevent?.Invoke(value); } }
    public int MpEvent { get { return playerMP; } set { playerMP = Mathf.Clamp(value, 0, playerMaxMP); playerMPevent?.Invoke(value); } }
    public int MaxHpEvent { get { return playerMaxHP; } set { playerMaxHP = value; } }
    public int MaxMpEvent { get { return playerMaxMP; } set { playerMaxMP = value; } }

    public float BrightnessVol {
        get { return brightnessVol; } set { brightnessVol = value; brightUpdate?.Invoke(value); } }

    private void Start()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
