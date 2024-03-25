using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneLoad : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        Manager.UICanvas.gameObject.SetActive(false);
        Destroy(FindObjectOfType<PlayerControll>().gameObject);
        Manager.Game.HpEvent = 100;
        Manager.Game.MpEvent = 100;
        Manager.Game.GoldEvent = 0;
        Debug.Log("titleLoad");
        yield return null;
    }

   
}
