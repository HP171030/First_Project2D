using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] public GameObject player;
    [SerializeField] Camera mainCam;
    [SerializeField] public Image hitdam;
    [SerializeField] public CinemachineVirtualCamera cine;
    [SerializeField] public PooledObject damageUI;

    public GameObject inPlayer;

    private int curGold = 0;

    private int playerHP = 150;
    private int playerMP = 150;
   [SerializeField] private int playerMaxHP = 150;
   [SerializeField] private int playerMaxMP = 150;

    public event UnityAction<int> playerHPevent;
    public event UnityAction<int> playerMPevent;
    public event UnityAction<float> brightUpdate;
    public event UnityAction<int> GoldUpdate;

    public UnityAction titleOption;

    public Toggle fullScreenMode;


    public bool time;
    public bool titleOff = true;
    public bool title;
    public bool onInvenTory = false;


    [SerializeField] PauseUI pauseUI;
    [SerializeField] public DeadUi deadui;

    private float brightnessVol;

    public UnityEvent DieEvent;


    public void CineInCam()
    {
        cine = FindObjectOfType<CinemachineVirtualCamera>();
     
    }
    public void HerePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void OnOption( InputValue value )
    {
        if(Manager.inven.inventoryUI.gameObject.activeSelf||Manager.Quest.questUI.gameObject.activeSelf)
        {
            Manager.inven.inventoryUI.gameObject.SetActive(false);
            Manager.Quest.questUI.gameObject.SetActive(false);
            Manager.inven.invenClose = true;
            Manager.Quest.questClose = true;
        }
        else
        {
 if ( !titleOff && value.isPressed )
            {
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
           
        }
       
       

    }



    public int GoldEvent { get { return curGold; } set { curGold = value; GoldUpdate?.Invoke(value); } }
    public int HpEvent { get { return playerHP; } set { playerHP = Mathf.Clamp(value, 0, playerMaxHP); playerHPevent?.Invoke(value); } }
    public int MpEvent { get { return playerMP; } set { playerMP = Mathf.Clamp(value, 0, playerMaxMP); playerMPevent?.Invoke(value); } }
    public int MaxHpEvent { get { return playerMaxHP; } set { playerMaxHP = value; } }
    public int MaxMpEvent { get { return playerMaxMP; } set { playerMaxMP = value; } }

    public float BrightnessVol {
        get { return brightnessVol; } set { brightnessVol = value; brightUpdate?.Invoke(value); } }
    private Coroutine mamaRegeneration;
    private void Start()
    {
      
        if ( hitdam != null )
        {
            hitdam.gameObject.SetActive(false);
        }
        StartManaRegeneration();

    }

    private void StartManaRegeneration()
    {
        if ( mamaRegeneration == null )
        {
            mamaRegeneration = StartCoroutine(ManaRegenerationCoroutine());
        }
    }

    // 마나 회복 코루틴
    private IEnumerator ManaRegenerationCoroutine()
    {
        while ( true )
        {
            // 0.2초마다 1씩 마나 증가
            yield return new WaitForSeconds(0.2f);
            MpEvent += 1;
        }
    }
    public void ScenePool()
    {
        Manager.Pool.CreatePool(damageUI, 7, 10);
    }


    public void ShakeCam()
    {
        if ( cine != null )
        {
            CinemachineBasicMultiChannelPerlin perlin = cine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if ( perlin != null )
            {
                perlin.m_AmplitudeGain = 2;
                perlin.m_FrequencyGain = 2;
                StartCoroutine(DampenShake(perlin));
               
            }
            else
            {
                Debug.LogError("CinemachineBasicMultiChannelPerlin not found.");
            }
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera not assigned.");
        }

       
        hitdam.gameObject.SetActive(true);
        hitdam.DOFade(0, 1f).OnComplete(() =>
        {
            hitdam.gameObject.SetActive(false);
            hitdam.DOFade(1, 0);
        });
    }
    private IEnumerator DampenShake( CinemachineBasicMultiChannelPerlin perlin )
    {
        yield return new WaitForSeconds(0.5f);
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }
}
