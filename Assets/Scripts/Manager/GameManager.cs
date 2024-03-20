using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] public GameObject player;
    [SerializeField] Camera mainCam;
    [SerializeField] public Image hitdam;

    public GameObject inPlayer;

    private int curGold = 0;

    private int playerHP = 100;
    private int playerMP = 100;
    private int playerMaxHP = 100;
    private int playerMaxMP = 100;

    public event UnityAction<int> playerHPevent;
    public event UnityAction<int> playerMPevent;
    public event UnityAction<float> brightUpdate;
    public event UnityAction<int> GoldUpdate;

    public UnityAction titleOption;

    public Toggle fullScreenMode;


    bool time;
    public bool titleOff = true;
    public bool title;
    public bool onInvenTory = false;


    [SerializeField] PauseUI pauseUI;

    private float brightnessVol;

    public UnityEvent DieEvent;
    
    public void HerePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void OnOption( InputValue value )
    {
        if ( !titleOff && value.isPressed )
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



    public int GoldEvent { get { return curGold; } set { curGold = value; GoldUpdate?.Invoke(value); } }
    public int HpEvent { get { return playerHP; } set { playerHP = Mathf.Clamp(value, 0, playerMaxMP); playerHPevent?.Invoke(value); } }
    public int MpEvent { get { return playerMP; } set { playerMP = Mathf.Clamp(value, 0, playerMaxMP); playerMPevent?.Invoke(value); } }
    public int MaxHpEvent { get { return playerMaxHP; } set { playerMaxHP = value; } }
    public int MaxMpEvent { get { return playerMaxMP; } set { playerMaxMP = value; } }

    public float BrightnessVol {
        get { return brightnessVol; } set { brightnessVol = value; brightUpdate?.Invoke(value); } }

    private void Start()
    {

        if ( hitdam != null )
        {
            hitdam.gameObject.SetActive(false);
        }

    }
    



    public void ShakeCam()
    {
 
        hitdam.gameObject.SetActive(true);
        hitdam.DOFade(0, 1f).OnComplete(() =>
        {
            hitdam.gameObject.SetActive(false);
            hitdam.DOFade(1, 0);
        });

    }
  
}
