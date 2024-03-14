using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR.Haptics;

public class Monster : MonoBehaviour, Idamagable
{
    
   Animator animator;
    Collider2D [] colliders;
  
    Vector3 playerPos;
    SpriteRenderer spriteRenderer;
     Vector2 moveDir;
     public bool MoveOn;
     bool CRSwitch;
     Collider2D thisCollider;
     Vector2 atkDir;
     bool atkDelayOn;
    Rigidbody2D rb;
    [SerializeField] Animator damagedEffect;
    [SerializeField] Transform damagedEffectPos;


    [SerializeField] LayerMask playerLayer;
    [SerializeField] PooledObject monsterPool;
    public MonsterData monsterData;

    public float thisMonsterHP;

    public GameObject dropItem;
    private void Start()
    {
        ChangeState(MonsterState.Idle);
        thisMonsterHP = monsterData.hp;
        colliders = new Collider2D [20];
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        thisCollider.enabled = true;
        MoveOn = false;
        
    }

    public void ChasePattern()
    {
        Targeting();
        Moving();
    }

    public void Targeting()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, monsterData.range, colliders, playerLayer);

        if ( size > 0 && !CRSwitch )
        {
            for ( int i = 0; i < size; i++ )
            {
                if ( playerLayer.Contain(colliders [i].gameObject.layer) )
                {
                    playerPos = colliders [i].transform.position;
                    moveDir = ( playerPos - transform.position ).normalized;
                    MoveOn = true;
                }
            }
        }
        else
        {
            ChangeState(MonsterState.Idle);
        }
   
        if ( moveDir.x < 0 )
        {
            spriteRenderer.flipX = true;
        }
        else if ( moveDir.x > 0 )
        {
            spriteRenderer.flipX = false;
        }
       
        StartCoroutine(MoveMonster());
        
    }

    public IEnumerator MoveMonster()
    {
        if ( !CRSwitch )
        {
            CRSwitch = true;
            animator.Play("Move");
            yield return new WaitForSeconds(2f);
            CRSwitch = false;

        }
    }

    public void Moving()
    {
        if ( MoveOn )
        {
            transform.Translate(moveDir * monsterData.speed/100);
        }
    }
    public void MoveSwitch()
    {
        MoveOn = true;
    }
    public void MoveSwitchOff()
    {
        MoveOn = false;
    }
    public enum MonsterState {Idle,Chase,Attack,Dead }

   public MonsterState curState;

 
    private void Update()
    {
        
        switch ( curState )
        {
            case MonsterState.Idle:
                int size = Physics2D.OverlapCircleNonAlloc(transform.position, monsterData.range, colliders, playerLayer);
               
                if ( size > 0 )
                {
                    for ( int i = 0; i < size; i++ )
                    {
                        if ( playerLayer.Contain(colliders [i].gameObject.layer) &&!atkDelayOn)
                        {
                         
                            ChangeState(MonsterState.Chase); break;
                        }
                    }
                }
                rb.velocity = Vector2.zero;
                
                break;

            case MonsterState.Chase:
                ChasePlayer();
                Collider2D player  = Physics2D.OverlapCircle(transform.position, monsterData.attackRange, playerLayer);
                if( player != null )
                {
                    atkDir =( player.transform.position - transform.position).normalized;
                    ChangeState(MonsterState.Attack);
                    MoveOn = false; 
                    break;
                }
                break;

            case MonsterState.Attack:
                
                StartCoroutine(AttackPlayer());
                break;

            case MonsterState.Dead:
                
                thisCollider.enabled = false;
                
                break;
        }
    }
    public IEnumerator AttackPlayer()
    {
        if ( !atkDelayOn )
        {
           
        Vector2 prePos = transform.position;
            Vector2 targetPos = ( Vector2 )transform.position + atkDir * 2f;                
            float t = 0;
            float duration = 0.5f;
           Collider2D player =  Physics2D.OverlapCircle(transform.position, monsterData.attackRange, playerLayer);
            atkDelayOn = true;
            Manager.Sound.PlaySFX(monsterData.soundAttack);
            animator.Play("AttackMimic");
            
            while ( t < 1f )
            {

                transform.position = Vector2.Lerp(prePos, targetPos, t);
                t += Time.deltaTime / duration;


                if ( player != null )
                {
                    SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                    spriteRenderer.material.color = Color.red;
                    

                }
                yield return null;

            }
            Manager.Sound.PlaySFX(monsterData.soundPlayerDamaged);
            if ( player != null )
                {
                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                spriteRenderer.material.color = Color.white;
                    Manager.Game.HpEvent -= monsterData.atk;
                }
            
           
            ChangeState(MonsterState.Idle);
            yield return new WaitForSeconds(monsterData.atkDelay);
            atkDelayOn = false;
            ChangeState(MonsterState.Chase);
        }
    }
    private void ChasePlayer()
    {
        ChasePattern();
        
    }

    public void ChangeState(MonsterState nextState)
    {
        curState = nextState;
        Debug.Log(curState);
    }
    public void TakeDamage( int damage )
    {
        if ( curState != MonsterState.Dead ) {
            thisMonsterHP -= damage;


            damagedEffectPos.position = transform.position + Random.insideUnitSphere * 1f;
            Animator Effector = damagedEffect.GetComponent<Animator>();
            Debug.Log(Effector.name);
            Effector.SetTrigger("Hit");
            
            if ( thisMonsterHP <= 0 )
            {
                ChangeState(MonsterState.Dead);
                animator.Play("Dead");
                StartCoroutine(ThisDestroy());

            }
            else
            {
                StartCoroutine(ColorMon());

                if (!MoveOn&&curState !=MonsterState.Attack )
                {
                    animator.Play("Damaged");
                    
                }

            }
        }
    }

    public IEnumerator ThisDestroy()
    {
        MoveOn = false;
        Manager.Sound.PlaySFX(monsterData.soundMonsterDead);
        yield return new WaitForSeconds(1f);
        Manager.Quest.HandleMonsterDied(monsterData.id);
        int Ran = Random.Range(0, 2);
        if(Ran > 0 )
        {
        GameObject dropIns = Instantiate(dropItem, transform.position, Quaternion.identity);
        dropIns.GetComponent<Fielditem>().SetItem(monsterData.dropItem);
            Debug.Log("Drop");
        }
        else
        {
            Debug.Log("NoDrop");
        }
        Destroy(gameObject);
       

    }
    public IEnumerator ColorMon()
    {
        spriteRenderer.material.color = Color.red;
        Debug.Log("Color");
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.material.color = Color.white;
    }
}
