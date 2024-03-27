using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireBoss : Monster
{
    [SerializeField] FireEnergy fireEnergy;
    [SerializeField] PooledObject fireEnergyPooled;
    [SerializeField] PooledObject stompPooled;
    [SerializeField] PooledObject summonEffect;
    [SerializeField] string [] bossString;
    [SerializeField] CinemachineVirtualCamera cineCam;
    [SerializeField] AudioClip startBossBGM;
    [SerializeField] AudioClip fireBall;
    [SerializeField] float kickRange = 2f;
    [SerializeField] AudioClip phase2BGM;
    [SerializeField] Animator skillAnim;
    [SerializeField] GameObject eff;
    [SerializeField] GameObject phase2;
    [SerializeField] GameObject phase2_1;
    [SerializeField] ParticleSystem fireEffect;
    [SerializeField] AudioClip fire;
    [SerializeField] AudioClip Phase2Sound;
    [SerializeField] GameObject fireArcher;

    int fireEnergyCount = 5;
    int kickShotRange = 5;
    Collider2D inRangePlayer;
    bool isStart = false;
    bool startPhase2 = false;
    bool phase2Pattern = false;


    protected override void Start()
    {
        base.Start();
        Manager.Pool.CreatePool(fireEnergyPooled, 20, 20);
        Manager.Pool.CreatePool(stompPooled, 10, 10);
        Manager.Pool.CreatePool(summonEffect, 10, 10);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterData.attackRange);
    }
    public void animEnd()
    {

        Manager.Game.HerePlayer();
        PlayerInput input = Manager.Game.player.GetComponent<PlayerInput>();
        Manager.UICanvas.dialogue.enabled = true;
        Manager.UICanvas.text.enabled = true;

        Queue<string> strings = new Queue<string>(bossString.Length);
        for ( int i = 0; i < bossString.Length; i++ )
        {

            strings.Enqueue(bossString [i]);
        }

        StartCoroutine(WaitSpace());
        Manager.UICanvas.text.text = strings.Dequeue();
        DoTweenText.DoText(Manager.UICanvas.text, 0.2f);
        IEnumerator WaitSpace()
        {
            while ( true )
            {
                yield return null;
                if ( Input.GetKeyDown(KeyCode.Space) )
                {
                    Next();
                    yield break;
                }
            }
        }
        void Next()
        {
            if ( strings.Count > 0 )
            {
                if ( Input.GetKeyDown(KeyCode.Space) )
                {
                    Manager.UICanvas.text.text = strings.Dequeue();
                    DoTweenText.DoText(Manager.UICanvas.text, 0.2f);

                    StartCoroutine(WaitSpace());
                }
            }
            else
            {

                Manager.UICanvas.dialogue.enabled = false;
                Manager.UICanvas.text.enabled = false;
                input.enabled = true;
                Manager.Sound.PlayBGM(startBossBGM);

                cineCam.Priority = 1;
                cineCam.gameObject.SetActive(false);
                isStart = true;
                Debug.Log(isStart);

                return;
            }
        }

    }
    protected override void DeadState()
    {
        if ( phase2Pattern )
        {
            base.DeadState();
        }
        if (!atkDelayOn&& !startPhase2&&!phase2Pattern )
            
        {startPhase2 = true;
            animator.SetTrigger("Phase1End");
        

        }
        
    }
    public void Phase2BGMOn()
    {
        Manager.Sound.PlayBGM(phase2BGM);
    }
    public void SecondPhaseStart()
    {
        thisMonsterMaxHp = 50;
        thisMonsterHP = 50;
      
        fireEnergyCount = 20;
        curState = MonsterState.Idle;
        startPhase2 = false;
        phase2Pattern = true;
        phase2.SetActive(true);
        phase2_1.SetActive(true);
        kickRange++;
        kickShotRange += 5;
        Manager.Game.ShakeCam();
        fireEffect.gameObject.SetActive(true);
       
    }
    protected override void Moving()
    {
        if ( MoveOn )
        {

            if(!phase2Pattern)
            transform.Translate(moveDir * monsterData.speed / 100);
            if ( phase2Pattern )
            {
               transform.Translate(moveDir * monsterData.speed / 10);
            }
        }
    }

    protected override void Targeting()
    {
        base.Targeting();
    }
    protected override void IdleStates()
    {
        if ( isStart )
        {

            base.IdleStates();

        }

    }
    protected override IEnumerator AttackPlayer()
    {
        int pattern = 0;

        if ( !startPhase2 )
        {
            if ( !phase2Pattern && !atkDelayOn )
            {

                pattern = Random.Range(0, 3);
                Debug.Log("phase1");
            }
            else
            {
                pattern = Random.Range(0, 4);
                Debug.Log(pattern);
            }

                switch ( pattern )
                {
                    case 0:
                        if ( !atkDelayOn )
                        {
                            #region 발차기1
                            Vector2 prePos = transform.position;
                            Vector2 targetPos = ( Vector2 )transform.position + atkDir * kickShotRange;
                            float t = 0;
                            float duration = 0.5f;
                            atkDelayOn = true;
                            Manager.Sound.PlaySFX(monsterData.soundAttack);
                            animator.Play("FireBossAttack1");

                            while ( t < 1f )
                            {
                                transform.position = Vector2.Lerp(prePos, targetPos, t);
                                t += Time.deltaTime / duration;
                                inRangePlayer = Physics2D.OverlapCircle(transform.position, kickRange, playerLayer);
                                Collider2D hitWall = Physics2D.OverlapCircle(transform.position, kickRange, Obstacle);
                                if ( hitWall != null )
                                {
                                    Debug.Log("Wall");
                                    break;
                                }
                                yield return null;

                            }

                            if ( inRangePlayer != null )
                            {
                                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                                Manager.Game.HpEvent -= monsterData.atk;
                                Manager.Game.ShakeCam();
                                spriteRenderer.material.color = Color.red;
                                spriteRenderer.material.DOColor(Color.white, 1f);
                                Manager.Sound.PlaySFX(monsterData.soundPlayerDamaged);
                            }
                            #endregion
                            if ( !phase2Pattern )
                                yield return new WaitForSeconds(1f);
                            else if ( phase2Pattern )
                                yield return new WaitForSeconds(0.1f);
                            #region 발차기2


                            prePos = transform.position;
                            player = Physics2D.OverlapCircle(transform.position, monsterData.range, playerLayer);
                            if ( player != null )
                            {
                                onBossAtk = true;
                                atkDir = ( player.transform.position - transform.position ).normalized;

                                if ( atkDir.x < 0 )
                                {
                                    spriteRenderer.flipX = true;

                                }
                                else if ( atkDir.x > 0 )
                                {
                                    spriteRenderer.flipX = false;

                                }
                                onBossAtk = false;
                            }



                            targetPos = ( Vector2 )transform.position + atkDir * kickShotRange;
                            t = 0;
                            if ( !phase2Pattern )
                            {
                                duration = 0.5f;
                            }
                            else if ( phase2Pattern )
                                duration = 0.3f;

                            atkDelayOn = true;
                            Manager.Sound.PlaySFX(monsterData.soundAttack);
                            animator.Play("FireBossAttack2");

                            while ( t < 1f )
                            {
                                transform.position = Vector2.Lerp(prePos, targetPos, t);
                                t += Time.deltaTime / duration;
                                inRangePlayer = Physics2D.OverlapCircle(transform.position, kickRange, playerLayer);
                                Collider2D hitWall = Physics2D.OverlapCircle(transform.position, kickRange, Obstacle);
                                if ( hitWall != null )
                                {
                                    Debug.Log("Wall");
                                    break;
                                }
                                yield return null;

                            }

                            if ( inRangePlayer != null )
                            {
                                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                                Manager.Game.HpEvent -= monsterData.atk;
                                Manager.Game.ShakeCam();
                                spriteRenderer.material.color = Color.red;
                                spriteRenderer.material.DOColor(Color.white, 1f);
                                Manager.Sound.PlaySFX(monsterData.soundPlayerDamaged);
                            }
                            #endregion


                            ChangeState(MonsterState.Idle);
                            onBossAtk = false;
                            animator.SetBool("Move", false);
                            if ( !phase2Pattern )
                                yield return new WaitForSeconds(monsterData.atkDelay);
                            else if ( phase2Pattern )
                                yield return new WaitForSeconds(1f);
                            atkDelayOn = false;


                        }
                        break;
                    case 1:
                        if ( !atkDelayOn )
                        {
                            if ( !phase2Pattern )
                            {
                                atkDelayOn = true;
                                animator.Play("FireBossStomp");
                                onBossAtk = false;
                                animator.SetBool("Move", false);
                                yield return new WaitForSeconds(1f);

                                ChangeState(MonsterState.Idle);
                                yield return new WaitForSeconds(monsterData.atkDelay);
                                atkDelayOn = false;

                            }
                            else if ( phase2Pattern )
                            {

                                onBossAtk = false;
                                atkDelayOn = true;
                                animator.SetBool("Move", false);
                                for ( int i = 0; i < 5; i++ )
                                {
                                    Debug.Log("stomp");
                                    animator.Play("FireBossStomp");
                                    yield return new WaitForSeconds(0.7f);

                                }
                                ChangeState(MonsterState.Idle);
                                yield return new WaitForSeconds(monsterData.atkDelay);
                                atkDelayOn = false;

                            }
                        }

                        break;
                    case 2:
                        if ( !atkDelayOn )
                        {

                            atkDelayOn = true;

                            StartCoroutine(FireEnergy());
                            animator.SetBool("Move", false);
                            yield return new WaitForSeconds(3f);
                            ChangeState(MonsterState.Idle);

                            yield return new WaitForSeconds(monsterData.atkDelay);
                            atkDelayOn = false;


                        }

                        break;
                    case 3:
                        if ( !atkDelayOn )
                        {


                            atkDelayOn = true;
                            animator.Play("Summon");
                            onBossAtk = false;
                            animator.SetBool("Move", false);
                            for ( int i = 0; i < 3; i++ )
                            {
                                Vector2 randomOffset = Random.insideUnitCircle.normalized * 10f;

                                Manager.Pool.GetPool(summonEffect,(Vector2)transform.position + randomOffset, Quaternion.identity);
                                Instantiate(fireArcher, ( Vector2 )transform.position + randomOffset, Quaternion.identity);


                            }

                            ChangeState(MonsterState.Idle);
                            yield return new WaitForSeconds(monsterData.atkDelay);
                            atkDelayOn = false;
                        }




                        break;
                }
            

           

        }


    }


    public void Stomp()
    {
        Manager.Sound.PlaySFX(fire);
        player = Physics2D.OverlapCircle(transform.position, monsterData.range, playerLayer);
        if ( !phase2Pattern )
        {
        Vector2 skillPos = player.transform.position;
            PooledObject InsEff= Manager.Pool.GetPool(stompPooled, skillPos, Quaternion.identity);
        Animator skillAnim = InsEff.GetComponent<Animator>();
        skillAnim.Play("Stomp");
        }
        else if (phase2Pattern )
            {
                if ( player != null )
                {
                    Vector2 playerPos = player.transform.position;
                    for ( int i = 0; i < 9; i++ )
                    {
                    Vector2 randomOffset = Random.insideUnitCircle.normalized * 3f;
                        Vector2 skillPos = playerPos + randomOffset;
                       PooledObject InsEff= Manager.Pool.GetPool(stompPooled, skillPos, Quaternion.identity);
        Animator skillAnim = InsEff.GetComponent<Animator>();
        skillAnim.Play("Stomp");
                    }
                }
            }
    }
    public IEnumerator FireEnergy()
    {


        if ( !phase2Pattern )
        {
            for ( int i = 0; i < fireEnergyCount; i++ )
            {
                Manager.Sound.PlaySFX(fireBall);
                animator.Play("FireBossFireball");
                PooledObject InsEff = Manager.Pool.GetPool(fireEnergyPooled, transform.position, Quaternion.identity);
                player = Physics2D.OverlapCircle(transform.position, monsterData.range, playerLayer);
                if ( player != null )
                {

                    atkDir = ( player.transform.position - transform.position ).normalized;
                    Rigidbody2D rb = InsEff.GetComponent<Rigidbody2D>();
                    rb.velocity = atkDir * 10f;

                }

                yield return new WaitForSeconds(0.5f);
            }

        }

        else if ( phase2Pattern )
        {
     
            for ( int i = 0; i < fireEnergyCount; i++ )
            {
                Manager.Sound.PlaySFX(fireBall);
                animator.Play("FireBossFireball");
                PooledObject InsEff = Manager.Pool.GetPool(fireEnergyPooled, transform.position, Quaternion.identity);
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                Rigidbody2D rb = InsEff.GetComponent<Rigidbody2D>();
                rb.velocity = randomDirection * 10f;
                
                yield return new WaitForSeconds(0.1f);
                player = Physics2D.OverlapCircle(transform.position, monsterData.range, playerLayer);
                if ( player != null )
                {

                    atkDir = ( player.transform.position - transform.position ).normalized;
                    InsEff.transform.DOMove((Vector2)player.transform.position + atkDir * 5f, 2f).SetEase(Ease.OutBack);
                }
            }

            yield return new WaitForSeconds(1f);
          
        }




    }
}
