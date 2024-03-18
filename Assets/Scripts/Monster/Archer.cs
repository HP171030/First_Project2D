using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class Archer : Monster
{
    [SerializeField] AudioClip arrowReady;
    [SerializeField] AudioClip arrowShot;
    [SerializeField] Transform arrowPos;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject arrow;
    Coroutine targetting;
    Coroutine shotting;

    protected override void Start()
    {
        base.Start();
        lineRenderer.positionCount = 2;

        lineRenderer.enabled = false;
    }
    protected override IEnumerator AttackPlayer()
    {
        base.AttackPlayer();
        if ( !atkDelayOn )
        {
            atkDelayOn = true;
            animator.SetTrigger("ArrowReady");
            lineRenderer.enabled = true;
            targetting = StartCoroutine(Aimming());
            Manager.Sound.PlaySFX(arrowReady);


            animator.SetBool("Move", false);
            
            yield return new WaitForSeconds(monsterData.atkDelay);
            atkDelayOn = false;
        }


    }
    private IEnumerator Aimming()
    {

        float time = 0;
        while ( time <= 2 )
        {

            time += Time.deltaTime;
            player = Physics2D.OverlapCircle(transform.position, 100, playerLayer);
            atkDir = ( player.transform.position - transform.position ).normalized;

            if ( flipXed )
            {
                lineRenderer.transform.localScale = new Vector3(-1, 1, 1);

            }
            else
            {
                lineRenderer.transform.localScale = new Vector3(1, 1, 1);

            }
            lineRenderer.SetPosition(0, arrowPos.transform.localPosition);
            lineRenderer.SetPosition(1, atkDir * 20);

            yield return null;
        }

        lineRenderer.enabled = false;
        
       shotting = StartCoroutine(ArrowShot());
        ChangeState(MonsterState.Idle);


    }
    public IEnumerator ArrowShot()
    {
        GameObject newArrow = Instantiate(arrow, transform.position, quaternion.identity);
        ArrowHit arrowScript = newArrow.GetComponent<ArrowHit>();
        arrowScript.SetArcher(this);
        Manager.Sound.PlaySFX(arrowShot);
        newArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, atkDir);

        float elapsedTime = 0f;
        float dur = 2f;
        Vector2 endPos = (Vector2)newArrow.transform.position + atkDir * 10;

        while ( elapsedTime < dur )
        {
            
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / dur;
            Debug.Log(t);   
                newArrow.transform.position = Vector2.Lerp(newArrow.transform.position, endPos, t);
                yield return null;

        }

       
        Destroy(newArrow);


    }
    public void StopArrowShot()
    {
        if ( shotting != null )
        {
            StopCoroutine(shotting);
            shotting = null;
        }
    }


}



