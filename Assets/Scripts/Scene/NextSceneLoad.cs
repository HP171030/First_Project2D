using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneLoad : BaseScene
{
    [SerializeField] AudioClip fireBGM; 
   [SerializeField] Transform startPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    public override IEnumerator LoadingRoutine()
    {
        Manager.UICanvas.gameObject.SetActive(true);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = startPos.position;
        virtualCamera.Follow = player.transform;
        Debug.Log("Last");
        Manager.Sound.PlayBGM(fireBGM);
        Manager.Game.CineInCam();
        yield return null;
    }

   
}
