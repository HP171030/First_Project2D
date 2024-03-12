using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Monster : PooledObject, Idamagable
{
    
    [SerializeField] string Name;
    [SerializeField] int atk;
    [SerializeField] int def;
    [SerializeField] int hp;
    bool dead;
    Rigidbody2D rb;
   Animator animator;
    FOV fov;
    [SerializeField] float Range;
    [SerializeField] Collider2D [] colliders;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Vector3 playerPos;
    [SerializeField] float speed;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Vector2 moveDir;
    [SerializeField] public bool MoveOn;
    [SerializeField] bool CRSwitch;
    [SerializeField] Collider2D thisCollider;
    [SerializeField] float attackRange;
    [SerializeField] Vector2 atkDir;
    [SerializeField] float atkDelay;
    [SerializeField] bool atkDelayOn;
    [SerializeField] ObjectPool pool;

    [Header("Sound Clip")]
    [SerializeField] AudioClip soundPlayerDamaged;
    [SerializeField] AudioClip soundMonsterDamaged;
    [SerializeField] AudioClip soundAttack;
    [SerializeField] AudioClip soundMonsterDead;

    private void Start()
    {
        animator = GetComponent<Animator>();
        fov = GetComponent<FOV>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        curState = MonsterState.Idle;
        colliders = new Collider2D [20];
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
    }
    public void ChasePattern()
    {
        Targeting();
        Moving();
    }

    public void Targeting()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, Range, colliders, playerLayer);

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
            transform.Translate(moveDir * speed/100);
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
        Debug.Log(curState);
        switch ( curState )
        {
            case MonsterState.Idle:
                int size = Physics2D.OverlapCircleNonAlloc(transform.position, Range, colliders, playerLayer);
               
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
                break;

            case MonsterState.Chase:
                ChasePlayer();
                Collider2D player  = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
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
           Collider2D player =  Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
            atkDelayOn = true;
            Manager.Sound.PlaySFX(soundAttack);
            animator.Play("AttackMimic");
            while ( t < 1f )
            {

                transform.position = Vector2.Lerp(prePos, targetPos, t);
                t += Time.deltaTime / duration;
                yield return null;
            }
            if ( player != null )
            {
                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                spriteRenderer.material.color = Color.red;
                Manager.Sound.PlaySFX(soundPlayerDamaged);
                yield return new WaitForSeconds(0.5f);
                Manager.Game.HpEvent -= atk;
                spriteRenderer.material.color = Color.white;
            }
           
            ChangeState(MonsterState.Idle);
            yield return new WaitForSeconds(atkDelay);
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
            hp -= damage;
            if ( hp <= 0 )
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
        Manager.Sound.PlaySFX(soundMonsterDead);
        yield return new WaitForSeconds(1f);
        pool.ReturnPool(this);
        
       

    }
    public IEnumerator ColorMon()
    {
        spriteRenderer.material.color = Color.red;
        Debug.Log("Color");
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.material.color = Color.white;
    }
}
