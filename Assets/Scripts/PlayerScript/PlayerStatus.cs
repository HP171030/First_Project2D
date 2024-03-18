using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
public class PlayerStatus : MonoBehaviour
{
   [SerializeField] TMP_Text Text;
    [SerializeField] TMP_Text goldText;
  [SerializeField]  Image HpGauge;
     [SerializeField]   Image MPGauge;
    [SerializeField] int playerHP;
        [SerializeField]int playerMP;
    int maxHpValue;
    int maxMpValue;
    [SerializeField] UnityEvent Die;
    [SerializeField] int curGold;
    [SerializeField] Ease ease;

    private void Start()
    {
        curGold = Manager.Game.GoldEvent;
        playerHP = Manager.Game.HpEvent;
        playerMP = Manager.Game.MpEvent;
        StatusHPUpdate(playerHP);
        StatusHPUpdate(playerMP);
        GoldUpdate(curGold);
        
    }
    private void OnEnable()
    {
        Manager.Game.GoldUpdate += GoldUpdate;
        Manager.Game.playerHPevent += StatusHPUpdate;
        Manager.Game.playerMPevent += StatusMPUpdate;
        maxHpValue = Manager.Game.MaxHpEvent;
        maxMpValue = Manager.Game.MaxMpEvent;

 
    }
    private void OnDisable()
    {
        Manager.Game.playerHPevent -= StatusHPUpdate;
        Manager.Game.playerMPevent -= StatusMPUpdate;
    }

    public void GoldUpdate(int value )
    {
        goldText.text = ( "GOLD : " + Manager.Game.GoldEvent ).ToString();
        goldText.rectTransform.DOShakePosition(duration: 0.5f, strength: new Vector3(0, 10f, 0f), vibrato: 40, randomness: 150);


    }

    private void StatusHPUpdate(int curValue)
    {
        
        
        
        Text.text = $"{Manager.Game.HpEvent}/{maxHpValue}".ToString();
        HpGauge.fillAmount =(float)Manager.Game.HpEvent / maxHpValue;
        

            if ( Manager.Game.HpEvent <= 0 )
        {
            Text.text = $"0/{maxHpValue}".ToString();
            Die.Invoke();
           
        }
        if ( Manager.Game.HpEvent > Manager.Game.MaxHpEvent )
        {
            Text.text = $"{Manager.Game.HpEvent}/{maxHpValue}".ToString();
            Debug.Log("Max");

        }
    }
    private void StatusMPUpdate( int curValue)
    {
        Text.text = $"{Manager.Game.MpEvent}/{maxMpValue}".ToString();
        MPGauge.fillAmount =(float)Manager.Game.MpEvent / maxMpValue;
    }
}
