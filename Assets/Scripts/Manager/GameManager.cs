using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    private int playerHP = 100;
    private int playerMP = 100;
    private int playerMaxHP = 100;
    private int playerMaxMP = 100;

    public event UnityAction<int> playerHPevent;
  public event UnityAction<int> playerMPevent;


    private void Start()
    {
       
    }
    public int HpEvent {  get { return playerHP; }set { playerHP = Mathf.Clamp(value, 0, playerMaxMP); playerHPevent?.Invoke(value); } }
    public int MpEvent { get { return playerMP; } set { playerMP = Mathf.Clamp(value, 0, playerMaxMP); playerMPevent?.Invoke(value); } }
    public int MaxHpEvent { get { return playerMaxHP; } set { playerMaxHP = value; } }
    public int MaxMpEvent { get { return playerMaxMP; } set { playerMaxMP = value; } }
}
