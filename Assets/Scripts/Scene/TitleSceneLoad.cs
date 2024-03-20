using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneLoad : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        Manager.UICanvas.gameObject.SetActive(false);
        Destroy(FindObjectOfType<PlayerControll>().gameObject);
        Debug.Log("titleLoad");
        yield return null;
    }

   
}
