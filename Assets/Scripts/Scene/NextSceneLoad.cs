using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class NextSceneLoad : BaseScene
{
    [SerializeField] AudioClip fireBGM; 
   [SerializeField] Transform startPos;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField]GameObject player;
    [SerializeField] GameObject playerPrefab;   

    public override IEnumerator LoadingRoutine()
    {
        
        Manager.UICanvas.gameObject.SetActive(true);
        Debug.Log(player);
        player = GameObject.FindGameObjectWithTag("Player");

        if ( player != null )
        {
            GameObject [] players = GameObject.FindGameObjectsWithTag("Player");

            if ( players.Length == 1 )
            {
                player.transform.position = startPos.position;
            }
            else if ( players.Length == 0 )
            {
                player = Instantiate(playerPrefab, Manager.Data.GameData.playerPos, Quaternion.identity);
                Debug.Log(player);
            }
        }
        else
        {
            player = Instantiate(playerPrefab, Manager.Data.GameData.playerPos, Quaternion.identity);
        }


        virtualCamera.Follow = player.transform;
     
        Manager.Sound.PlayBGM(fireBGM);
        Manager.UICanvas.dialogue.gameObject.SetActive(true);
        Manager.UICanvas.dialogue.enabled = false;
        Manager.Game.titleOff = false;
        Manager.UICanvas.gameObject.SetActive(true);
        Manager.Game.CineInCam();
        Manager.Game.ScenePool();
        yield return null;
    }
    public override void SceneLoad()
    {
        Debug.Log("Loading File");

        Manager.Game.MaxHpEvent = Manager.Data.GameData.maxHp;
        Manager.Game.MaxMpEvent = Manager.Data.GameData.maxMp;
        Debug.Log($"LoadingHP = {Manager.Data.GameData.Hp}");
        Manager.Game.HpEvent = Manager.Data.GameData.Hp;
        Manager.Game.MpEvent = Manager.Data.GameData.Mp;
        Manager.Game.GoldEvent = Manager.Data.GameData.gold;
       
       
    }


}
