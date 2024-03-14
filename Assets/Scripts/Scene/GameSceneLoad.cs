using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameSceneLoad : BaseScene
{
  
  [SerializeField] Animator animator;
  [SerializeField] GameObject player;
    [SerializeField] ParticleSystem particlePrefab;
    [SerializeField] GameObject monsterMimic;
    [SerializeField] Transform mimicRegen1;

    [SerializeField] Image dialogOff;
    [SerializeField] UIManager uiManager;

    
    
    public override IEnumerator LoadingRoutine()
    {
       
        Debug.Log("GameSceneLoad");     //게임 씬에 따른 내용 미리 구성(맵,풀드오브젝트..)
        uiManager = FindObjectOfType<UIManager>();
        if(uiManager != null )
        {
            uiManager.playerInput = player.GetComponent<PlayerInput>();
        }
       
        int ranSize = Random.Range(1, 4);
        
        for(int i = 0; i < ranSize; i++ )
        {
            Vector2 newPosition = mimicRegen1.position + Random.insideUnitSphere * 3f;
            Instantiate(monsterMimic, newPosition, Quaternion.identity);

        }
        
        dialogOff.gameObject.SetActive(true);
        dialogOff.enabled = false;
        Manager.Game.titleOff = false;
        yield return null;
    }

    public override IEnumerator OnStartScene()
    { 
        
        
        yield return new WaitForSeconds(0.3f);
        animator.gameObject.SetActive(true);
        Instantiate(particlePrefab,player.transform.position,Quaternion.identity);
        player.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        animator.gameObject.SetActive(false);
       

        
        

    }


}
