using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName ="Item/Consumable/Health")]
public class ItemHealEffect : ItemEffect
{
    
    public int healing = 10;
    public static event UnityAction hpConsumeEvent;
   
    public override bool eft()
    {
        if(Manager.Game.HpEvent == Manager.Game.MaxHpEvent)
        {
            
            return false;
        }
        else if ( Manager.Game.HpEvent < Manager.Game.MaxHpEvent )
        {
        Manager.Game.HpEvent += healing;
            hpConsumeEvent?.Invoke();
        
            
            
        }
        return true;
    }
    
}
