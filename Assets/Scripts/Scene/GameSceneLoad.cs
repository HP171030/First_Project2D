using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class GameSceneLoad : BaseScene
{
  [SerializeField] CinemachineVirtualCamera cineCam;
  [SerializeField] Animator animator;
  [SerializeField] GameObject player;
    [SerializeField] ParticleSystem particlePrefab;
    
    
    public override IEnumerator LoadingRoutine()
    {
       
        Debug.Log("GameSceneLoad");     //���� ���� ���� ���� �̸� ����(��,Ǯ�������Ʈ..)
        yield return null;
    }

    public override IEnumerator OnStartScene()
    {
        
        
        Debug.Log("Summon");
        
        
        cineCam.Follow = player.transform;
        yield return new WaitForSeconds(0.3f);
        animator.gameObject.SetActive(true);
        Instantiate(particlePrefab,player.transform.position,Quaternion.identity);
        player.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        animator.gameObject.SetActive(false);

        
        

    }

}
