using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class FireBoss : Monster
{
    [SerializeField] FireEnergy fireEnergy;
    [SerializeField] string [] bossString;
    [SerializeField] CinemachineVirtualCamera cineCam;
    [SerializeField] AudioClip startBossBGM;
    [SerializeField] float kickRange = 2f;
    [SerializeField] Transform skillPos;
    [SerializeField] Animator skillAnim;
    [SerializeField] GameObject eff;
    int fireEnergyCount = 5;

    Collider2D inRangePlayer;
    bool isStart = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,monsterData.attackRange);
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
                cineCam.gameObject.SetActive( false );
                isStart = true;
                Debug.Log(isStart);
                
                return;
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
        int phase1Pattern = Random.Range(0, 3);
        Debug.Log(phase1Pattern);
        switch ( phase1Pattern )
        {
            case 0:
                if ( !atkDelayOn )
                {
                    #region 발차기1
                    Vector2 prePos = transform.position;
                    Vector2 targetPos = ( Vector2 )transform.position + atkDir * 7f;
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
                            // 벽과 충돌한 경우, 멈춰버리기
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

                    yield return new WaitForSeconds(1f);
                    #region 발차기2
                    prePos = transform.position;
                    player = Physics2D.OverlapCircle(transform.position, monsterData.range, playerLayer);
                    if ( player != null )
                    {
                        onBossAtk = true;
                        atkDir = ( player.transform.position - transform.position ).normalized;

                        if ( atkDir.x < 0 )
                        {
                            transform.localScale = new Vector3(-1, localY, 0f);

                        }
                        else if ( atkDir.x > 0 )
                        {
                            transform.localScale = new Vector3(1, localY, 0f);

                        }
                    }



                    targetPos = ( Vector2 )transform.position + atkDir * 7f;
                    t = 0;
                    duration = 0.5f;

                    atkDelayOn = true;
                    Manager.Sound.PlaySFX(monsterData.soundAttack);
                    animator.Play("FireBossAttack2");

                    while ( t < 1f )
                    {
                        transform.position = Vector2.Lerp(prePos, targetPos, t);
                        t += Time.deltaTime / duration;
                        inRangePlayer = Physics2D.OverlapCircle(transform.position, kickRange, playerLayer);
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
                    yield return new WaitForSeconds(monsterData.atkDelay);
                    atkDelayOn = false;

                }
                break;
            case 1:
                if ( !atkDelayOn )
                {
                    eff.SetActive(true);
                    atkDelayOn = true;
                    animator.Play("FireBossStomp");
                    Debug.Log("Skill");
                    onBossAtk = false;
                    animator.SetBool("Move", false);
                    yield return new WaitForSeconds(1f);
                    ChangeState(MonsterState.Idle);
                    
                    yield return new WaitForSeconds(monsterData.atkDelay);
                    atkDelayOn = false;
                    eff.SetActive(false);
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

                }

    }
   

    public void Stomp()
    {
        player = Physics2D.OverlapCircle(transform.position, monsterData.range, playerLayer);

        skillPos.position = player.transform.position;
        skillAnim.Play("Stomp");
    }
    public IEnumerator FireEnergy()
    {
        for(int i = 0; i < fireEnergyCount; i++)
        {
            animator.Play("FireBossFireball");
          FireEnergy ins=  Instantiate(fireEnergy,transform.position, Quaternion.identity);
            player = Physics2D.OverlapCircle(transform.position, monsterData.range, playerLayer);
            if(player != null)
            {
               
                atkDir = ( player.transform.position - transform.position ).normalized;
                ins.rb.velocity = atkDir * 10f;
             
            }
            else
            {
                break;
            }
        yield return new WaitForSeconds(0.5f);
        }
    }




}
