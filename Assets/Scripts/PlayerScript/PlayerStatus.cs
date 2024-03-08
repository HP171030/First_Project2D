using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine.Events;
public class PlayerStatus : MonoBehaviour
{
   [SerializeField] TMP_Text Text;
  [SerializeField]  Image HpGauge;
     [SerializeField]   Image MPGauge;
    [SerializeField] int playerHP;
        [SerializeField]int playerMP;
    int maxHpValue;
    int maxMpValue;
    [SerializeField] UnityEvent Die;


    private void Start()
    {
        playerHP = Manager.Game.HpEvent;
        playerMP = Manager.Game.MpEvent;
        StatusHPUpdate(playerHP);
        StatusHPUpdate(playerMP);
    }
    private void OnEnable()
    {
        Manager.Game.playerHPevent += StatusHPUpdate;
        Manager.Game.playerMPevent += StatusHPUpdate;
        maxHpValue = Manager.Game.MaxHpEvent;
        maxMpValue = Manager.Game.MaxMpEvent;

 
    }
    private void OnDisable()
    {
        Manager.Game.playerHPevent -= StatusMPUpdate;
        Manager.Game.playerMPevent -= StatusMPUpdate;
    }

    private void StatusHPUpdate(int curValue)
    {
        Text.text = $"{curValue}/{maxHpValue}".ToString();
        HpGauge.fillAmount =(float) curValue / maxHpValue;
        Debug.Log(Manager.Game.HpEvent);
        if ( Manager.Game.HpEvent <= 0 )
        {
            Text.text = $"0/{maxHpValue}".ToString();
            Die.Invoke();
           
        }
    }
    private void StatusMPUpdate( int curValue)
    {
        Text.text = $"{curValue}/{maxMpValue}".ToString();
        MPGauge.fillAmount =(float) curValue / maxMpValue;
    }
}
