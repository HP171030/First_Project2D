
using System.Collections;

using Unity.Mathematics;

using UnityEngine;


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
    protected override void Update()
    {
        if ( moveDir.x < 0 && !onBossAtk )
        {

            flipXed = true;
            transform.localScale = new Vector3(-localX, localY, 1);



        }
        else if ( moveDir.x > 0 && !onBossAtk )
        {

            flipXed = false;
            transform.localScale = new Vector3(localX, localY, 1);

        }
        switch ( curState )
        {
            case MonsterState.Idle:



                IdleStates();
                break;

            case MonsterState.Chase:
                ChasePattern();
                player = Physics2D.OverlapCircle(transform.position, monsterData.attackRange, playerLayer);
                if ( player != null )
                {
                    atkDir = ( player.transform.position - transform.position ).normalized;
                    ChangeState(MonsterState.Attack);
                    MoveOn = false;
                    break;
                }
                break;

            case MonsterState.Attack:

                StartCoroutine(AttackPlayer());
                break;

            case MonsterState.Dead:

                DeadState();

                break;
        }
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

        float isTime = 0f;
        float dur = 1f;
        Vector2 startPos = newArrow.transform.position; 
        Vector2 endPos = (Vector2)newArrow.transform.position + atkDir * 20;

        while ( isTime < dur )
        {
            
            isTime += Time.deltaTime;
            float t = isTime / dur;
            
                newArrow.transform.position = Vector2.Lerp(startPos, endPos, t);
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



