using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneLoad : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        Debug.Log("titleLoad");
        yield return null;
    }

   
}
